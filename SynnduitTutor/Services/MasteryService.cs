using Microsoft.EntityFrameworkCore;
using SynnduitTutor.Data;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>Result of recording a quiz attempt, including any donuts just earned and milestone state.</summary>
public sealed record AttemptOutcome(
    ConceptMastery Mastery,
    IReadOnlyList<DonutAward> NewAwards,
    bool LevelJustCompleted,
    string? CompletedLevelId,
    string? CompletedLevelName)
{
    public int DonutsEarned => NewAwards.Sum(a => a.Points);
}

/// <summary>
/// Reads/writes per-learner mastery state and awards donuts atomically with each attempt.
/// Uses an IDbContextFactory so each operation gets a short-lived context (safe under Blazor Server).
/// </summary>
public sealed class MasteryService(
    IDbContextFactory<TutorDbContext> dbFactory, CourseCatalog catalog, DonutOptions donuts)
{
    public async Task<List<Learner>> GetLearnersAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Learners.OrderBy(l => l.DisplayName).ToListAsync();
    }

    public async Task<Learner> GetOrCreateLearnerAsync(string displayName, string externalId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var learner = await db.Learners.FirstOrDefaultAsync(l => l.ExternalId == externalId);
        if (learner is null)
        {
            learner = new Learner { DisplayName = displayName, ExternalId = externalId, CreatedUtc = DateTime.UtcNow };
            db.Learners.Add(learner);
            await db.SaveChangesAsync();
        }
        return learner;
    }

    public async Task<Learner?> GetLearnerAsync(int learnerId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Learners.FindAsync(learnerId);
    }

    public async Task<Dictionary<string, ConceptMastery>> GetMasteryMapAsync(int learnerId, string courseId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ConceptMastery
            .Where(m => m.LearnerId == learnerId && m.CourseId == courseId)
            .ToDictionaryAsync(m => m.ConceptId);
    }

    public async Task<ConceptMastery?> GetMasteryAsync(int learnerId, string courseId, string conceptId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ConceptMastery.FirstOrDefaultAsync(
            m => m.LearnerId == learnerId && m.CourseId == courseId && m.ConceptId == conceptId);
    }

    /// <summary>
    /// Records a quiz attempt: updates best score, mastery, attempt/remediation counts, seen items,
    /// (for L0 placement) skip-on-pass, and — when a concept is mastered for the FIRST time — awards
    /// donuts (base + first-attempt / perfect-score / critical bonuses) and a level-completion bonus.
    /// </summary>
    public async Task<AttemptOutcome> RecordAttemptAsync(
        int learnerId, string courseId, Concept concept, QuizResult result, IEnumerable<string> servedItemIds)
    {
        var graph = (catalog.GetStore(courseId)
                     ?? throw new InvalidOperationException($"Unknown course '{courseId}'.")).Graph;

        await using var db = await dbFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;

        var existing = await db.ConceptMastery.FirstOrDefaultAsync(
            x => x.LearnerId == learnerId && x.CourseId == courseId && x.ConceptId == concept.Id);
        var isFirstAttempt = existing is null;
        var wasMastered = existing?.Mastered == true;

        var m = existing;
        if (m is null)
        {
            m = new ConceptMastery { LearnerId = learnerId, CourseId = courseId, ConceptId = concept.Id };
            db.ConceptMastery.Add(m);
        }

        m.Attempts++;
        m.LastAttemptUtc = now;
        if (result.Score > m.BestScore) m.BestScore = result.Score;

        var placement = graph.LevelOf(concept)?.IsPlacement == true;
        var passed = result.Score >= concept.MasteryThreshold;

        if (passed)
        {
            m.Mastered = true;
            if (placement && isFirstAttempt && concept.SkipOnPass)
                m.SkippedByPlacement = true;
        }
        else if (!isFirstAttempt)
        {
            m.RemediationCycles++;
            if (m.RemediationCycles >= graph.MasteryModel.RemediationCycleLimitBeforeEscalation)
                m.EscalatedToMentor = true;
        }

        var seen = new HashSet<string>(
            m.SeenItemIdsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        foreach (var id in servedItemIds) seen.Add(id);
        m.SeenItemIdsCsv = string.Join(',', seen);

        // ---- Donuts: only on the first time this concept is mastered ----
        var awards = new List<DonutAward>();
        var justMastered = passed && !wasMastered;
        if (justMastered)
        {
            void Add(string reason, int points)
            {
                if (points <= 0) return;
                awards.Add(new DonutAward { LearnerId = learnerId, CourseId = courseId, Reason = reason, ConceptId = concept.Id, Points = points, EarnedUtc = now });
            }
            Add("Mastered", donuts.Award.ConceptMastered);
            if (isFirstAttempt) Add("FirstAttempt", donuts.Award.FirstAttemptBonus);
            if (result.Score >= 1.0) Add("Perfect", donuts.Award.PerfectScoreBonus);
            if (concept.SynnduitCritical) Add("Critical", donuts.Award.CriticalConceptBonus);
            db.DonutAwards.AddRange(awards);
        }

        await db.SaveChangesAsync();

        // ---- Level-completion milestone ----
        var levelJustCompleted = false;
        string? completedLevelId = null, completedLevelName = null;
        if (justMastered)
        {
            var level = graph.LevelOf(concept);
            if (level is not null)
            {
                var ids = level.Concepts.Select(c => c.Id).ToList();
                var doneCount = await db.ConceptMastery.CountAsync(x =>
                    x.LearnerId == learnerId && x.CourseId == courseId && ids.Contains(x.ConceptId) && (x.Mastered || x.SkippedByPlacement));

                if (doneCount == ids.Count)
                {
                    var already = await db.DonutAwards.AnyAsync(a =>
                        a.LearnerId == learnerId && a.CourseId == courseId && a.LevelId == level.Id && a.Reason == "LevelComplete");
                    if (!already)
                    {
                        var bonus = new DonutAward
                        {
                            LearnerId = learnerId, CourseId = courseId, Reason = "LevelComplete", LevelId = level.Id,
                            Points = donuts.Award.LevelCompletionBonus, EarnedUtc = now
                        };
                        db.DonutAwards.Add(bonus);
                        await db.SaveChangesAsync();
                        if (bonus.Points > 0) awards.Add(bonus);
                        levelJustCompleted = true;
                        completedLevelId = level.Id;
                        completedLevelName = level.Name;
                    }
                }
            }
        }

        return new AttemptOutcome(m, awards, levelJustCompleted, completedLevelId, completedLevelName);
    }
}
