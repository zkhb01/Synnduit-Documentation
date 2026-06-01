using SynnduitTutor.Models;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

public class MasteryServiceTests
{
    private static QuizResult Pass() => new(Array.Empty<ItemResult>(), 1.0, true, Array.Empty<Item>());
    private static QuizResult Fail() => new(Array.Empty<ItemResult>(), 0.0, false, Array.Empty<Item>());

    [Fact]
    public async Task Passing_attempt_marks_concept_mastered()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = new MasteryService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var l11 = store.Graph.FindConcept("L1.1")!;

        var m = await svc.RecordAttemptAsync(learner.Id, l11, Pass(), new[] { "L1.1.q1" });

        Assert.True(m.Mastered);
        Assert.False(m.SkippedByPlacement);          // L1 is not a placement level
        Assert.Equal(1, m.Attempts);
        Assert.Contains("L1.1.q1", m.SeenItemIdsCsv);
    }

    [Fact]
    public async Task L0_placement_first_attempt_pass_is_skipped()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = new MasteryService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var l0g1 = store.Graph.FindConcept("L0.G1")!;
        Assert.True(l0g1.SkipOnPass);

        var m = await svc.RecordAttemptAsync(learner.Id, l0g1, Pass(), new[] { "L0.G1.q1" });

        Assert.True(m.Mastered);
        Assert.True(m.SkippedByPlacement);
    }

    [Fact]
    public async Task Repeated_failures_escalate_to_mentor()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = new MasteryService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var c = store.Graph.FindConcept("L1.3")!;

        await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q1" }); // first attempt
        var second = await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q2" });
        Assert.False(second.EscalatedToMentor);                                   // 1 remediation cycle
        Assert.Equal(1, second.RemediationCycles);

        var third = await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q3" });
        Assert.True(third.EscalatedToMentor);                                     // hit the limit (2)
        Assert.False(third.Mastered);
        Assert.Equal(3, third.Attempts);
    }

    [Fact]
    public async Task Best_score_is_retained_across_attempts()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = new MasteryService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var c = store.Graph.FindConcept("L1.2")!;

        await svc.RecordAttemptAsync(learner.Id, c, new(Array.Empty<ItemResult>(), 0.6, false, Array.Empty<Item>()), new[] { "L1.2.q1" });
        var m = await svc.RecordAttemptAsync(learner.Id, c, new(Array.Empty<ItemResult>(), 0.4, false, Array.Empty<Item>()), new[] { "L1.2.q2" });

        Assert.Equal(0.6, m.BestScore, 3);   // best, not latest
    }
}
