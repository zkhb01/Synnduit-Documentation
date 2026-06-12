using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>Verdict on a free-text (model-scored) answer.</summary>
/// <param name="Correct">Whether the answer covers the rubric well enough to count as correct.</param>
/// <param name="Score">0–1 coverage of the rubric (informational; mastery uses <paramref name="Correct"/>).</param>
/// <param name="Feedback">One or two sentences addressed to the learner.</param>
/// <param name="Met">Rubric points the answer covered.</param>
/// <param name="Missed">Rubric points the answer missed.</param>
/// <param name="IsLive">True when graded by Claude; false for the self-assessment stub / a failed grade.</param>
public sealed record GradeResult(
    bool Correct, double Score, string Feedback,
    IReadOnlyList<string> Met, IReadOnlyList<string> Missed, bool IsLive);

/// <summary>
/// Grades free-text answers to short/scenario/classify/match items against their rubric. The core
/// slice ships <see cref="StubAnswerGrader"/> (the learner self-assesses); <see cref="ClaudeAnswerGrader"/>
/// grades the typed answer with the Anthropic API when a key is configured.
/// </summary>
public interface IAnswerGrader
{
    /// <summary>True when the grader actually evaluates the answer (Claude). False = self-assessment mode.</summary>
    bool IsLive { get; }

    Task<GradeResult> GradeAsync(Item item, string answer, CancellationToken ct = default);
}

/// <summary>
/// No-LLM grader: the app keeps the self-assessment radios and never calls this. Implemented
/// defensively so the contract holds regardless of caller.
/// </summary>
public sealed class StubAnswerGrader : IAnswerGrader
{
    public bool IsLive => false;

    public Task<GradeResult> GradeAsync(Item item, string answer, CancellationToken ct = default) =>
        Task.FromResult(new GradeResult(false, 0, "Self-grading mode.",
            Array.Empty<string>(), Array.Empty<string>(), IsLive: false));
}
