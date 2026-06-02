using SynnduitTutor.Models;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

public class MasteryServiceTests
{
    private static QuizResult Pass() => new(Array.Empty<ItemResult>(), 1.0, true, Array.Empty<Item>());
    private static QuizResult Fail() => new(Array.Empty<ItemResult>(), 0.0, false, Array.Empty<Item>());
    private static QuizResult Score(double s) => new(Array.Empty<ItemResult>(), s, s >= 0.8, Array.Empty<Item>());

    private static MasteryService NewService(TestSupport.TempDbFactory db, CurriculumStore store) =>
        new(db, store, TestSupport.DonutOpts());

    [Fact]
    public async Task Passing_attempt_marks_concept_mastered()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = NewService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var l11 = store.Graph.FindConcept("L1.1")!;

        var outcome = await svc.RecordAttemptAsync(learner.Id, l11, Pass(), new[] { "L1.1.q1" });

        Assert.True(outcome.Mastery.Mastered);
        Assert.False(outcome.Mastery.SkippedByPlacement);
        Assert.Equal(1, outcome.Mastery.Attempts);
        Assert.Contains("L1.1.q1", outcome.Mastery.SeenItemIdsCsv);
    }

    [Fact]
    public async Task L0_placement_first_attempt_pass_is_skipped()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = NewService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var l0g1 = store.Graph.FindConcept("L0.G1")!;
        Assert.True(l0g1.SkipOnPass);

        var outcome = await svc.RecordAttemptAsync(learner.Id, l0g1, Pass(), new[] { "L0.G1.q1" });

        Assert.True(outcome.Mastery.Mastered);
        Assert.True(outcome.Mastery.SkippedByPlacement);
    }

    [Fact]
    public async Task Repeated_failures_escalate_to_mentor()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = NewService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var c = store.Graph.FindConcept("L1.3")!;

        await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q1" });
        var second = await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q2" });
        Assert.False(second.Mastery.EscalatedToMentor);
        Assert.Equal(1, second.Mastery.RemediationCycles);

        var third = await svc.RecordAttemptAsync(learner.Id, c, Fail(), new[] { "L1.3.q3" });
        Assert.True(third.Mastery.EscalatedToMentor);
        Assert.False(third.Mastery.Mastered);
        Assert.Equal(3, third.Mastery.Attempts);
    }

    [Fact]
    public async Task Best_score_is_retained_across_attempts()
    {
        using var db = new TestSupport.TempDbFactory();
        var store = TestSupport.RealStore();
        var svc = NewService(db, store);

        var learner = await svc.GetOrCreateLearnerAsync("Tester", "stub:tester");
        var c = store.Graph.FindConcept("L1.2")!;

        await svc.RecordAttemptAsync(learner.Id, c, Score(0.6), new[] { "L1.2.q1" });
        var outcome = await svc.RecordAttemptAsync(learner.Id, c, Score(0.4), new[] { "L1.2.q2" });

        Assert.Equal(0.6, outcome.Mastery.BestScore, 3);
    }
}
