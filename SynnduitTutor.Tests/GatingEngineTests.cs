using SynnduitTutor.Data;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

public class GatingEngineTests
{
    private readonly CurriculumStore _store = TestSupport.RealStore();

    private static ConceptMastery Mastered(string id) => new() { ConceptId = id, Mastered = true, BestScore = 1.0 };

    [Fact]
    public void Entry_concept_with_no_prereqs_is_available()
    {
        var gating = new GatingEngine();
        var l11 = _store.Graph.FindConcept("L1.1")!;
        var status = gating.StatusFor(l11, new Dictionary<string, ConceptMastery>());
        Assert.Equal(ConceptStatus.Available, status);
    }

    [Fact]
    public void Concept_is_locked_until_all_prereqs_done_then_available()
    {
        var gating = new GatingEngine();
        var l16 = _store.Graph.FindConcept("L1.6")!;        // prereqs: L1.4, L1.5
        Assert.Equal(new[] { "L1.4", "L1.5" }, l16.Prereqs.ToArray());

        // Nothing mastered → locked
        var none = new Dictionary<string, ConceptMastery>();
        Assert.Equal(ConceptStatus.Locked, gating.StatusFor(l16, none));

        // Only one prereq mastered → still locked
        var partial = new Dictionary<string, ConceptMastery> { ["L1.4"] = Mastered("L1.4") };
        Assert.Equal(ConceptStatus.Locked, gating.StatusFor(l16, partial));

        // Both prereqs done → available
        var both = new Dictionary<string, ConceptMastery>
        {
            ["L1.4"] = Mastered("L1.4"),
            ["L1.5"] = Mastered("L1.5"),
        };
        Assert.Equal(ConceptStatus.Available, gating.StatusFor(l16, both));
    }

    [Fact]
    public void Skipped_placement_counts_as_done_for_prereqs()
    {
        var gating = new GatingEngine();
        var l0g2 = _store.Graph.FindConcept("L0.G2")!;       // prereqs: L0.G1, L0.O1
        var mastery = new Dictionary<string, ConceptMastery>
        {
            ["L0.G1"] = new() { ConceptId = "L0.G1", SkippedByPlacement = true },
            ["L0.O1"] = new() { ConceptId = "L0.O1", SkippedByPlacement = true },
        };
        Assert.Equal(ConceptStatus.Available, gating.StatusFor(l0g2, mastery));
    }

    [Fact]
    public void Overview_reports_level_completion_and_gating()
    {
        var gating = new GatingEngine();

        // Master every L1 concept → L1 complete.
        var mastery = _store.Graph.FindLevel("L1")!.Concepts
            .ToDictionary(c => c.Id, c => Mastered(c.Id));

        var overview = gating.BuildOverview(_store.Graph, mastery);
        var l1 = overview.Single(v => v.Level.Id == "L1");
        Assert.True(l1.Complete);

        // L2 (future) is gated by L0; with L0 untouched, L2 stays locked.
        var l2 = overview.SingleOrDefault(v => v.Level.Id == "L2");
        if (l2 is not null)
            Assert.False(l2.Unlocked);
    }
}
