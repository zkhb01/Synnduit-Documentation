using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>A learner's answer to one item.</summary>
public sealed class ItemResponse
{
    public List<string> SelectedOptionIds { get; set; } = new();   // mc/multi
    public List<string> OrderedOptionIds { get; set; } = new();    // order
    public bool? SelfAssessedCorrect { get; set; }                  // model-scored (short/scenario/classify/match)
}

public sealed record ItemResult(Item Item, bool Correct, bool WasModelScored);

public sealed record QuizResult(
    IReadOnlyList<ItemResult> Items,
    double Score,
    bool Passed,
    IReadOnlyList<Item> WrongItems);

/// <summary>
/// Deterministic auto-scoring for mc / multi / order. Model-scored items (short, scenario,
/// classify, match) are self-assessed in this core slice; Claude will grade them later.
/// </summary>
public sealed class ScoringService
{
    public bool ScoreItem(Item item, ItemResponse response)
    {
        if (!item.IsAutoScored)
            return response.SelfAssessedCorrect ?? false;

        var correct = item.Correct ?? new List<string>();
        return item.Type switch
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

        var score = results.Count == 0 ? 0 : (double)results.Count(r => r.Correct) / results.Count;
        var wrong = results.Where(r => !r.Correct).Select(r => r.Item).ToList();
        return new QuizResult(results, score, score >= threshold, wrong);
    }
}
