using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>A learner's answer to one item.</summary>
public sealed class ItemResponse
{
    public List<string> SelectedOptionIds { get; set; } = new();   // mc/multi
    public List<string> OrderedOptionIds { get; set; } = new();    // order
    public bool? SelfAssessedCorrect { get; set; }                  // model-scored (short/scenario/classify/match)
    public string? FreeText { get; set; }                           // the typed answer for model-scored items
}

public sealed record ItemResult(Item Item, bool Correct, bool WasModelScored);

public sealed record QuizResult(
    IReadOnlyList<ItemResult> Items,
    double Score,
    bool Passed,
    IReadOnlyList<Item> WrongItems);

/// <summary>
/// Deterministic auto-scoring for mc / multi / order. Model-scored items (short, scenario,
/// classify, match) are <b>practice</b>: they're still scored for per-item feedback (Claude or
/// self-assessment), but they do not count toward the mastery score or the pass/fail gate — so an
/// imperfect free-text grade can never wrongly advance or block a learner. Only auto-scored items
/// drive <see cref="QuizResult.Score"/>, <see cref="QuizResult.Passed"/>, and the remediation set.
/// </summary>
public sealed class ScoringService
{
    public bool ScoreItem(Item item, ItemResponse response)
    {
        if (!item.IsAutoScored)
            return response.SelfAssessedCorrect ?? false;

        var correct = item.Correct ?? new List<string>();
        return item.EffectiveType switch
        {
            "mc" => response.SelectedOptionIds.Count == 1
                    && correct.Contains(response.SelectedOptionIds[0]),
            "multi" => correct.Count > 0
                    && new HashSet<string>(response.SelectedOptionIds).SetEquals(correct),
            "order" => response.OrderedOptionIds.SequenceEqual(correct),
            _ => false
        };
    }

    public QuizResult Score(IReadOnlyList<Item> items, IReadOnlyDictionary<string, ItemResponse> responses, double threshold)
    {
        var results = new List<ItemResult>();
        foreach (var item in items)
        {
            var resp = responses.GetValueOrDefault(item.Id) ?? new ItemResponse();
            results.Add(new ItemResult(item, ScoreItem(item, resp), !item.IsAutoScored));
        }

        // Gate on objective (auto-scored) items only; model items are practice/feedback.
        // Fallback: if a sample somehow has no auto items, score over all of them so the gate
        // is still decidable.
        var gating = results.Where(r => r.Item.IsAutoScored).ToList();
        if (gating.Count == 0) gating = results;

        var score = gating.Count == 0 ? 0 : (double)gating.Count(r => r.Correct) / gating.Count;
        var wrong = gating.Where(r => !r.Correct).Select(r => r.Item).ToList();
        return new QuizResult(results, score, score >= threshold, wrong);
    }
}
