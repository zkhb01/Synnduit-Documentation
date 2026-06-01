using Microsoft.EntityFrameworkCore;
using SynnduitTutor.Data;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

/// <summary>
/// Reads/writes per-learner mastery state. Uses an IDbContextFactory so each operation gets a
/// short-lived context (safe under Blazor Server's concurrency model).
/// </summary>
public sealed class MasteryService(IDbContextFactory<TutorDbContext> dbFactory, CurriculumStore store)
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

    public async Task<Dictionary<string, ConceptMastery>> GetMasteryMapAsync(int learnerId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ConceptMastery
            .Where(m => m.LearnerId == learnerId)
            .ToDictionaryAsync(m => m.ConceptId);
    }

    public async Task<ConceptMastery?> GetMasteryAsync(int learnerId, string conceptId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ConceptMastery.FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.ConceptId == conceptId);
    }

    /// <summary>
    /// Records a quiz attempt: updates best score, mastery, attempt/remediation counts, seen items,
    /// and (for L0 placement, first-attempt pass) the skip flag.
    /// </summary>
    public async Task<ConceptMastery> RecordAttemptAsync(
        int learnerId, Concept concept, QuizResult result, IEnumerable<string> servedItemIds)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var m = await db.ConceptMastery.FirstOrDefaultAsync(x => x.LearnerId == learnerId && x.ConceptId == concept.Id);
        var isFirstAttempt = m is null;
        if (m is null)
        {
            m = new ConceptMastery { LearnerId = learnerId, ConceptId = concept.Id };
            db.ConceptMastery.Add(m);
        }

        m.Attempts++;
        m.LastAttemptUtc = DateTime.UtcNow;
        if (result.Score > m.BestScore) m.BestScore = result.Score;

        var placement = store.Graph.LevelOf(concept)?.IsPlacement == true;
        var passed = result.Score >= concept.MasteryThreshold;

        if (passed)
        {
            m.Mastered = true;
            if (placement && isFirstAttempt && concept.SkipOnPass)
                m.SkippedByPlacement = true;
        }
        else if (!isFirstAttempt)
        {
            // A failed re-attempt counts as a remediation cycle; escalate past the configured limit.
            m.RemediationCycles++;
            if (m.RemediationCycles >= store.Graph.MasteryModel.RemediationCycleLimitBeforeEscalation)
                m.EscalatedToMentor = true;
        }

        var seen = new HashSet<string>(
            m.SeenItemIdsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        foreach (var id in servedItemIds) seen.Add(id);
        m.SeenItemIdsCsv = string.Join(',', seen);

        await db.SaveChangesAsync();
        return m;
    }
}
