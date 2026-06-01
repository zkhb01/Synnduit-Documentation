using Bunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using SynnduitTutor.Components.Pages;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

/// <summary>
/// Renders the interactive pages the way a signed-in learner would see them. These catch
/// render-time crashes that route/status checks miss (e.g. binding to a missing dictionary key,
/// or a redirect firing during prerender) because curl can't drive a Blazor circuit.
/// </summary>
public class PageRenderTests : IDisposable
{
    private readonly BunitContext _ctx = new();
    private readonly TestSupport.TempDbFactory _db = new();
    private int _learnerId;

    public PageRenderTests()
    {
        var store = TestSupport.RealStore();
        var session = new LearnerSession();

        _ctx.Services.AddSingleton(store);
        _ctx.Services.AddSingleton<IDbContextFactory<Data.TutorDbContext>>(_db);
        _ctx.Services.AddSingleton(new MasteryService(_db, store));
        _ctx.Services.AddSingleton(new GatingEngine(store));
        _ctx.Services.AddSingleton<ScoringService>();
        _ctx.Services.AddSingleton<IRemediationService, StubRemediationService>();
        _ctx.Services.AddSingleton(session);

        // Sign in a real learner so the auth-guarded pages render their content.
        var mastery = new MasteryService(_db, store);
        var learner = mastery.GetOrCreateLearnerAsync("Render Tester", "stub:render").GetAwaiter().GetResult();
        _learnerId = learner.Id;
        session.SignIn(learner.Id, learner.DisplayName);
    }

    [Fact]
    public void Dashboard_renders_the_concept_path()
    {
        var cut = _ctx.Render<Dashboard>();
        Assert.Contains("L1.1", cut.Markup);
        Assert.Contains("What Synnduit is", cut.Markup);
    }

    [Fact]
    public void Lesson_renders_markdown_body()
    {
        var cut = _ctx.Render<Lesson>(p => p.Add(x => x.ConceptId, "L1.1"));
        Assert.Contains("ETL meets version control", cut.Markup);   // rendered from the .md body
        Assert.Contains("Check my understanding", cut.Markup);
    }

    [Fact]
    public void Quiz_renders_all_item_types_including_model_items()
    {
        // L1.1 includes a model-scored "short" item (L1.1.q2) — the case that previously threw
        // KeyNotFoundException on the textarea bind.
        var cut = _ctx.Render<Quiz>(p => p.Add(x => x.ConceptId, "L1.1"));
        Assert.Contains("Submit answers", cut.Markup);
        Assert.Contains("self-graded", cut.Markup);   // the model item rendered its self-grade UI
    }

    [Fact]
    public void Quiz_submit_scores_and_shows_results()
    {
        var cut = _ctx.Render<Quiz>(p => p.Add(x => x.ConceptId, "L1.1"));

        // Answer the auto-scored mc questions by selecting their first radio; submit.
        // (We're proving the submit path renders results without throwing, not getting 100%.)
        cut.Find("button.btn-primary").Click();

        Assert.Contains("correct", cut.Markup);        // results summary ("X of N correct")
        Assert.Contains("My path", cut.Markup);        // back-link in the results view
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _db.Dispose();
    }
}
