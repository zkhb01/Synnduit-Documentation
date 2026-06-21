using System.Text;
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>
/// Configuration for the live Claude remediation service. Bound from the "Anthropic" section of
/// appsettings (or the ANTHROPIC_API_KEY environment variable). When no API key is resolvable the
/// app falls back to <see cref="StubRemediationService"/> — see Program.cs.
/// </summary>
public sealed class AnthropicOptions
{
    public string? ApiKey { get; set; }

    /// <summary>Output-token ceiling for one remediation (includes adaptive-thinking tokens).</summary>
    public int MaxTokens { get; set; } = 8000;
}

/// <summary>
/// Adaptive re-teaching backed by the Anthropic API (Claude Opus 4.8). Given the failed concept's
/// objective + remediation angle + canonical lesson + the learner's specific wrong answers, it
/// re-teaches the concept from a *different* angle and diagnoses each misconception. On any API
/// failure (network, rate limit, refusal) it falls back to the deterministic
/// <see cref="StubRemediationService"/> so a quiz attempt is never blocked.
/// </summary>
public sealed class ClaudeRemediationService : IRemediationService
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    private readonly AnthropicClient _client;
    private readonly AnthropicOptions _options;
    private readonly StubRemediationService _fallback;
    private readonly ILogger<ClaudeRemediationService> _log;

    public ClaudeRemediationService(
        AnthropicClient client, AnthropicOptions options,
        StubRemediationService fallback, ILogger<ClaudeRemediationService> log)
    {
        _client = client;
        _options = options;
        _fallback = fallback;
        _log = log;
    }

    public async Task<RemediationResult> RemediateAsync(
        Concept concept, Lesson? lesson, IReadOnlyList<Item> wrongItems,
        IReadOnlyDictionary<string, ItemResponse> responses, CancellationToken ct = default)
    {
        try
        {
            var parameters = new MessageCreateParams
            {
                Model = Model.ClaudeOpus4_8,
                MaxTokens = _options.MaxTokens,
                Thinking = new ThinkingConfigAdaptive(),
                System = BuildSystemPrompt(),
                OutputConfig = new OutputConfig
                {
                    Effort = Effort.Medium,
                    Format = ResultSchema(),
                },
                Messages =
                [
                    new() { Role = Role.User, Content = BuildUserPrompt(concept, lesson, wrongItems, responses) },
                ],
            };

            var response = await _client.Messages.Create(parameters, cancellationToken: ct);

            if (response.StopReason == "refusal")
            {
                _log.LogWarning("Claude refused remediation for {ConceptId}; using stub.", concept.Id);
                return await _fallback.RemediateAsync(concept, lesson, wrongItems, responses, ct);
            }

            var json = response.Content
                .Select(b => b.Value)
                .OfType<TextBlock>()
                .Select(t => t.Text)
                .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t));

            if (json is null)
                throw new InvalidOperationException("Claude returned no text content.");

            var parsed = JsonSerializer.Deserialize<RemediationPayload>(json, JsonOpts)
                         ?? throw new InvalidOperationException("Could not parse remediation JSON.");

            var heading = string.IsNullOrWhiteSpace(parsed.Heading)
                ? $"Let's revisit {concept.Id} — {concept.Name}"
                : parsed.Heading!;
            var body = string.IsNullOrWhiteSpace(parsed.Body)
                ? (concept.RemediationAngle ?? "Re-read the lesson, focusing on the points you missed.")
                : parsed.Body!;

            return new RemediationResult(heading, body, parsed.MisconceptionNotes ?? new List<string>(), IsLive: true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _log.LogError(ex, "Claude remediation failed for {ConceptId}; falling back to stub.", concept.Id);
            return await _fallback.RemediateAsync(concept, lesson, wrongItems, responses, ct);
        }
    }

    private static string BuildSystemPrompt() =>
        """
        You are the Synnduit AI tutor, re-teaching a learner who just missed questions on a single
        concept of the Synnduit data-synchronization course. Synnduit is "ETL meets version control":
        a long-running engine that syncs a source system to a destination (at CSSD, the DDH), tracking
        every change and remembering source↔destination mappings.

        Re-teach ONLY the one concept you are given, and do it from a DIFFERENT angle than the canonical
        lesson the learner already read (use the supplied remediation angle). Be concrete, warm, and
        brief — a learner should absorb it in under two minutes. Ground everything in the supplied
        lesson and objective; do not invent Synnduit behavior that isn't supported there. Address the
        learner's specific wrong answers directly: name the misconception and correct it.

        Return only the structured fields requested: a short heading, a Markdown body (the re-teaching),
        and one short misconception note per wrong answer (empty array if none apply).
        """;

    private static string BuildUserPrompt(
        Concept concept, Lesson? lesson, IReadOnlyList<Item> wrongItems,
        IReadOnlyDictionary<string, ItemResponse> responses)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Concept to re-teach: {concept.Id} — {concept.Name}");
        sb.AppendLine();
        sb.AppendLine($"Objective: {concept.DisplayObjective}");
        if (!string.IsNullOrWhiteSpace(concept.RemediationAngle))
            sb.AppendLine($"Remediation angle (teach from here): {concept.RemediationAngle}");
        sb.AppendLine();

        if (lesson is not null && !string.IsNullOrWhiteSpace(lesson.MarkdownBody))
        {
            sb.AppendLine("## Canonical lesson the learner already read (do NOT repeat verbatim — use a fresh angle)");
            sb.AppendLine(lesson.MarkdownBody.Trim());
            sb.AppendLine();
        }

        sb.AppendLine("## Questions the learner missed");
        var n = 0;
        foreach (var item in wrongItems)
        {
            n++;
            sb.AppendLine($"{n}. {item.Stem}");
            var resp = responses.GetValueOrDefault(item.Id);
            var chosen = DescribeChosen(item, resp);
            if (!string.IsNullOrWhiteSpace(chosen)) sb.AppendLine($"   - Learner answered: {chosen}");
            var correct = DescribeCorrect(item);
            if (!string.IsNullOrWhiteSpace(correct)) sb.AppendLine($"   - Correct answer: {correct}");
            if (item.Misconceptions is not null && resp is not null)
                foreach (var id in resp.SelectedOptionIds)
                    if (item.Misconceptions.TryGetValue(id, out var why))
                        sb.AppendLine($"   - Authored note on their choice: {why}");
        }

        return sb.ToString();
    }

    private static string DescribeChosen(Item item, ItemResponse? resp)
    {
        if (resp is null) return "";
        if (resp.SelectedOptionIds.Count > 0)
            return string.Join("; ", resp.SelectedOptionIds.Select(id => OptionText(item, id)));
        if (resp.OrderedOptionIds.Count > 0)
            return string.Join(" → ", resp.OrderedOptionIds.Select(id => OptionText(item, id)));
        if (resp.SelfAssessedCorrect is not null)
            return resp.SelfAssessedCorrect.Value ? "(self-assessed correct)" : "(self-assessed not yet)";
        return "";
    }

    private static string DescribeCorrect(Item item)
    {
        if (item.Correct is not { Count: > 0 }) return "";
        return item.Type == "order"
            ? string.Join(" → ", item.Correct.Select(id => OptionText(item, id)))
            : string.Join("; ", item.Correct.Select(id => OptionText(item, id)));
    }

    private static string OptionText(Item item, string id) =>
        item.Options?.FirstOrDefault(o => o.Id == id)?.Text ?? id;

    /// <summary>JSON-schema constraint so Claude returns exactly the fields we map to RemediationResult.</summary>
    private static JsonOutputFormat ResultSchema() => new()
    {
        Schema = new Dictionary<string, JsonElement>
        {
            ["type"] = JsonSerializer.SerializeToElement("object"),
            ["properties"] = JsonSerializer.SerializeToElement(new
            {
                heading = new { type = "string", description = "Short, encouraging heading for the re-teaching." },
                body = new { type = "string", description = "Markdown re-teaching of the concept, from a fresh angle." },
                misconceptionNotes = new
                {
                    type = "array",
                    items = new { type = "string" },
                    description = "One short note per wrong answer, naming and correcting the misconception.",
                },
            }),
            ["required"] = JsonSerializer.SerializeToElement(new[] { "heading", "body", "misconceptionNotes" }),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false),
        },
    };

    private sealed record RemediationPayload(string? Heading, string? Body, List<string>? MisconceptionNotes);
}
