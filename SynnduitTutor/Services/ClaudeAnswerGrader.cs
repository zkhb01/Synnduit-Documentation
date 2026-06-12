using System.Text;
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>
/// Grades a learner's free-text answer against an item's rubric using the Anthropic API
/// (Claude Opus 4.8, adaptive thinking, structured output). On any API failure or refusal it
/// returns a non-live "couldn't grade" verdict (marked incorrect, with a clear message) so the
/// learner can retry — a quiz attempt is never silently passed or hard-failed by an outage.
/// </summary>
public sealed class ClaudeAnswerGrader : IAnswerGrader
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    private readonly AnthropicClient _client;
    private readonly ILogger<ClaudeAnswerGrader> _log;

    public ClaudeAnswerGrader(AnthropicClient client, ILogger<ClaudeAnswerGrader> log)
    {
        _client = client;
        _log = log;
    }

    public bool IsLive => true;

    public async Task<GradeResult> GradeAsync(Item item, string answer, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(answer))
            return new GradeResult(false, 0, "No answer was entered.",
                Array.Empty<string>(), Array.Empty<string>(), IsLive: true);

        try
        {
            var parameters = new MessageCreateParams
            {
                Model = Model.ClaudeOpus4_8,
                MaxTokens = 2000,
                Thinking = new ThinkingConfigAdaptive(),
                System = SystemPrompt,
                OutputConfig = new OutputConfig { Effort = Effort.Medium, Format = GradeSchema() },
                Messages = [new() { Role = Role.User, Content = BuildPrompt(item, answer) }],
            };

            var response = await _client.Messages.Create(parameters, cancellationToken: ct);

            if (response.StopReason == "refusal")
            {
                _log.LogWarning("Claude refused to grade item {ItemId}.", item.Id);
                return Unavailable();
            }

            var json = response.Content.Select(b => b.Value).OfType<TextBlock>()
                .Select(t => t.Text).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));
            if (json is null) throw new InvalidOperationException("No text content from grader.");

            var p = JsonSerializer.Deserialize<GradePayload>(json, JsonOpts)
                    ?? throw new InvalidOperationException("Could not parse grade JSON.");

            var score = Math.Clamp(p.Score, 0, 1);
            return new GradeResult(
                p.Correct, score,
                string.IsNullOrWhiteSpace(p.Feedback) ? (p.Correct ? "Looks good." : "Not quite yet.") : p.Feedback!,
                p.Met ?? new List<string>(), p.Missed ?? new List<string>(), IsLive: true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "Claude grading failed for item {ItemId}.", item.Id);
            return Unavailable();
        }
    }

    private static GradeResult Unavailable() =>
        new(false, 0, "Automatic grading was unavailable — this answer wasn't scored. Please try again.",
            Array.Empty<string>(), Array.Empty<string>(), IsLive: false);

    private const string SystemPrompt =
        """
        You grade a learner's free-text answer to a question from the Synnduit data-synchronization
        course. Synnduit is "ETL meets version control": a long-running engine that syncs a source
        system to a destination (at CSSD, the DDH), tracking every field change and remembering
        source↔destination mappings.

        Grade ONLY against the supplied rubric and reference answer. Reward answers that convey the
        essential ideas even if the wording differs from the sample; do not require exact phrasing or
        penalize extra correct detail. Mark "correct": true when the answer covers the rubric's
        essential points (roughly the substance of the reference answer). Be fair but rigorous — a
        vague or off-target answer is not correct.

        Return only the structured fields: correct (boolean), score (0–1 rubric coverage), a short
        feedback sentence addressed to the learner ("you…"), and which rubric points were met vs missed.
        """;

    private static string BuildPrompt(Item item, string answer)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Question: {item.Stem}");
        sb.AppendLine();
        if (item.Rubric is { Count: > 0 })
        {
            sb.AppendLine("Rubric — each point should be addressed:");
            foreach (var r in item.Rubric) sb.AppendLine($"- {r}");
            sb.AppendLine();
        }
        if (!string.IsNullOrWhiteSpace(item.SampleAnswer))
        {
            sb.AppendLine($"Reference answer: {item.SampleAnswer}");
            sb.AppendLine();
        }
        // For classify/match items the answer key lives in the options + correct list.
        if (item.Options is { Count: > 0 } && item.Correct is { Count: > 0 })
        {
            sb.AppendLine("Answer key:");
            foreach (var id in item.Correct)
            {
                var text = item.Options.FirstOrDefault(o => o.Id == id)?.Text ?? id;
                sb.AppendLine($"- {text}");
            }
            sb.AppendLine();
        }
        sb.AppendLine($"Learner's answer: {answer}");
        return sb.ToString();
    }

    private static JsonOutputFormat GradeSchema() => new()
    {
        Schema = new Dictionary<string, JsonElement>
        {
            ["type"] = JsonSerializer.SerializeToElement("object"),
            ["properties"] = JsonSerializer.SerializeToElement(new
            {
                correct = new { type = "boolean", description = "True if the answer covers the rubric's essential points." },
                score = new { type = "number", description = "0–1 fraction of rubric points covered." },
                feedback = new { type = "string", description = "One or two sentences addressed to the learner." },
                met = new { type = "array", items = new { type = "string" }, description = "Rubric points the answer covered." },
                missed = new { type = "array", items = new { type = "string" }, description = "Rubric points the answer missed." },
            }),
            ["required"] = JsonSerializer.SerializeToElement(new[] { "correct", "score", "feedback", "met", "missed" }),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false),
        },
    };

    private sealed record GradePayload(
        bool Correct, double Score, string? Feedback, List<string>? Met, List<string>? Missed);
}
