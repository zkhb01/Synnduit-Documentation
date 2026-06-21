using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

public sealed record RemediationResult(string Heading, string Body, IReadOnlyList<string> MisconceptionNotes, bool IsLive);

/// <summary>
/// Adaptive re-teaching for a failed concept. The core slice ships <see cref="StubRemediationService"/>
/// (deterministic, built from the concept's authored remediationAngle + per-distractor misconceptions);
/// a later ClaudeRemediationService implementing this same interface calls the Anthropic API.
/// </summary>
public interface IRemediationService
{
    Task<RemediationResult> RemediateAsync(Concept concept, Lesson? lesson, IReadOnlyList<Item> wrongItems,
        IReadOnlyDictionary<string, ItemResponse> responses, CancellationToken ct = default);
}

/// <summary>
/// No-LLM remediation: surfaces the authored remediation angle and the specific misconception text
/// for each wrong multiple-choice answer. Proves the data flow that Claude will later enrich.
/// </summary>
public sealed class StubRemediationService : IRemediationService
{
    public Task<RemediationResult> RemediateAsync(Concept concept, Lesson? lesson, IReadOnlyList<Item> wrongItems,
        IReadOnlyDictionary<string, ItemResponse> responses, CancellationToken ct = default)
    {
        var notes = new List<string>();
        foreach (var item in wrongItems)
        {
            if (item.Misconceptions is null) continue;
            var resp = responses.GetValueOrDefault(item.Id);
            var chosen = resp?.SelectedOptionIds ?? new List<string>();
            foreach (var id in chosen)
                if (item.Misconceptions.TryGetValue(id, out var why))
                    notes.Add($"“{OptionText(item, id)}” — {why}");
        }

        var body = concept.RemediationAngle
                   ?? "Re-read the lesson, focusing on the points you missed, then try fresh questions.";

        var heading = $"Let's revisit {concept.Id} — {concept.Name}";
        return Task.FromResult(new RemediationResult(heading, body, notes, IsLive: false));
    }

    private static string OptionText(Item item, string id) =>
        item.Options?.FirstOrDefault(o => o.Id == id)?.Text ?? id;
}
