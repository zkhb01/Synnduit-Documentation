using SynnduitTutor.Data;
using SynnduitTutor.Models;

namespace SynnduitTutor.Services;

public enum ConceptStatus
{
    Locked,      // a prerequisite is not yet done
    Available,   // all prerequisites done, not yet mastered
    Mastered,    // met the mastery threshold
    Skipped      // L0 placement: passed on first contact
}

public sealed record ConceptView(Concept Concept, ConceptStatus Status, double BestScore, int Attempts, bool Critical);

public sealed record LevelView(Level Level, IReadOnlyList<ConceptView> Concepts, bool Complete, bool Unlocked);

/// <summary>
/// Pure gating logic: given a learner's mastery state, decide which concepts are locked /
/// available / mastered, and whether each level's gate is satisfied.
/// </summary>
public sealed class GatingEngine(CurriculumStore store)
{
    public bool IsDone(ConceptMastery? m) => m is not null && (m.Mastered || m.SkippedByPlacement);

    public ConceptStatus StatusFor(Concept concept, IReadOnlyDictionary<string, ConceptMastery> mastery)
    {
        var m = mastery.GetValueOrDefault(concept.Id);
        if (m is not null && m.SkippedByPlacement) return ConceptStatus.Skipped;
        if (m is not null && m.Mastered) return ConceptStatus.Mastered;

        var prereqsDone = concept.Prereqs.All(p => IsDone(mastery.GetValueOrDefault(p)));
        return prereqsDone ? ConceptStatus.Available : ConceptStatus.Locked;
    }

    public IReadOnlyList<LevelView> BuildOverview(IReadOnlyDictionary<string, ConceptMastery> mastery)
    {
        var graph = store.Graph;
        var views = new List<LevelView>();

        // A level is "unlocked" if it's first in path order, or every level that gates it is complete.
        // (In the current graph, L0 gates L2; L1 has no gate and is the entry level.)
        var completeById = new Dictionary<string, bool>();

        foreach (var level in graph.Levels.OrderBy(l => l.Order))
        {
            var conceptViews = level.Concepts
                .Select(c =>
                {
                    var m = mastery.GetValueOrDefault(c.Id);
                    return new ConceptView(c, StatusFor(c, mastery), m?.BestScore ?? 0, m?.Attempts ?? 0, c.SynnduitCritical);
                })
                .ToList();

            var complete = conceptViews.All(cv => cv.Status is ConceptStatus.Mastered or ConceptStatus.Skipped);
            completeById[level.Id] = complete;

            var gatedBy = graph.Levels.Where(l => l.GatesLevel == level.Id).ToList();
            var unlocked = gatedBy.Count == 0 || gatedBy.All(g => completeById.GetValueOrDefault(g.Id));

            views.Add(new LevelView(level, conceptViews, complete, unlocked));
        }

        return views;
    }
}
