using SynnduitTutor.Models;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

public class DonutTests
{
    private static QuizResult Result(double s) => new(Array.Empty<ItemResult>(), s, s >= 0.8, Array.Empty<Item>());

    private static (MasteryService mastery, DonutService donuts, CurriculumStore store) Build(TestSupport.TempDbFactory db)
    {
        var store = TestSupport.RealStore();
        var opts = TestSupport.DonutOpts();
        return (new MasteryService(db, store, opts), new DonutService(db, opts), store);
    }

    [Fact]
    public async Task First_attempt_perfect_critical_concept_earns_all_bonuses()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, donuts, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");

        var critical = store.Graph.FindConcept("L0.E2")!;   // synnduitCritical
        Assert.True(critical.SynnduitCritical);

        var outcome = await mastery.RecordAttemptAsync(learner.Id, critical, Result(1.0), new[] { "L0.E2.q1" });

        // base 1 + first-attempt 1 + perfect 1 + critical 1 = 4
        Assert.Equal(4, outcome.DonutsEarned);
        var balance = await donuts.GetBalanceAsync(learner.Id);
        Assert.Equal(4, balance.Available);
    }

    [Fact]
    public async Task Late_nonperfect_noncritical_mastery_earns_base_only()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, _, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");
        var c = store.Graph.FindConcept("L1.1")!;   // not critical

        await mastery.RecordAttemptAsync(learner.Id, c, Result(0.0), new[] { "L1.1.q1" }); // fail (first)
        var outcome = await mastery.RecordAttemptAsync(learner.Id, c, Result(0.8), new[] { "L1.1.q2" }); // pass on retry, 80%

        Assert.Equal(1, outcome.DonutsEarned);  // base only — no first-attempt, no perfect, not critical
    }

    [Fact]
    public async Task Re_mastering_awards_no_additional_donuts()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, donuts, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");
        var c = store.Graph.FindConcept("L1.1")!;

        var first = await mastery.RecordAttemptAsync(learner.Id, c, Result(1.0), new[] { "L1.1.q1" });
        var again = await mastery.RecordAttemptAsync(learner.Id, c, Result(1.0), new[] { "L1.1.q2" });

        Assert.True(first.DonutsEarned > 0);
        Assert.Equal(0, again.DonutsEarned);
        var balance = await donuts.GetBalanceAsync(learner.Id);
        Assert.Equal(first.DonutsEarned, balance.Earned);
    }

    [Fact]
    public async Task Clearing_a_level_grants_the_completion_bonus_once()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, donuts, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");

        var l0 = store.Graph.FindLevel("L0")!;
        AttemptOutcome? last = null;
        foreach (var c in l0.Concepts)
            last = await mastery.RecordAttemptAsync(learner.Id, c, Result(1.0), new[] { c.Id + ".q1" });

        Assert.True(last!.LevelJustCompleted);
        Assert.Equal("L0", last.CompletedLevelId);

        // Balance includes the +3 level bonus on top of per-concept donuts.
        var balance = await donuts.GetBalanceAsync(learner.Id);
        var conceptDonuts = l0.Concepts.Count;        // at least 1 base each
        Assert.True(balance.Earned >= conceptDonuts + 3);
    }

    [Fact]
    public async Task Redeem_below_threshold_throws()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, donuts, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");

        // 0.8 first attempt → base + first-attempt = 2 donuts (below the default threshold of 6).
        await mastery.RecordAttemptAsync(learner.Id, store.Graph.FindConcept("L1.1")!, Result(0.8), new[] { "x" });
        var balance = await donuts.GetBalanceAsync(learner.Id);
        Assert.True(balance.Available < donuts.Threshold);

        await Assert.ThrowsAsync<InvalidOperationException>(() => donuts.RedeemAsync(learner.Id, "test"));
    }

    [Fact]
    public async Task Redeem_at_threshold_issues_voucher_and_zeroes_available()
    {
        using var db = new TestSupport.TempDbFactory();
        var (mastery, donuts, store) = Build(db);
        var learner = await mastery.GetOrCreateLearnerAsync("T", "stub:t");

        // Master several L1 concepts first-attempt-perfect to exceed 6 donuts.
        foreach (var id in new[] { "L1.1", "L1.2", "L1.3", "L1.4" })
            await mastery.RecordAttemptAsync(learner.Id, store.Graph.FindConcept(id)!, Result(1.0), new[] { id + ".q1" });

        var before = await donuts.GetBalanceAsync(learner.Id);
        Assert.True(before.Available >= donuts.Threshold);

        var voucher = await donuts.RedeemAsync(learner.Id, "Foundations");
        Assert.Equal(before.Available, voucher.Points);
        Assert.StartsWith("DNT-", voucher.Code);
        Assert.Equal("<Manager Name>", voucher.ManagerName);   // placeholder from default options

        var after = await donuts.GetBalanceAsync(learner.Id);
        Assert.Equal(0, after.Available);
        Assert.Equal(before.Available, after.Redeemed);
        Assert.Equal(before.Earned, after.Earned);            // earned total unchanged by redemption
    }
}
