using SynnduitTutor.Models;
using SynnduitTutor.Services;
using Xunit;

namespace SynnduitTutor.Tests;

public class ScoringServiceTests
{
    private readonly ScoringService _scoring = new();

    private static Item Mc() => new()
    {
        Id = "q", Type = "mc", Scoring = "auto",
        Options = new() { new() { Id = "a" }, new() { Id = "b" }, new() { Id = "c" } },
        Correct = new() { "b" }
    };

    [Fact]
    public void Mc_correct_choice_scores_true()
    {
        var r = new ItemResponse { SelectedOptionIds = { "b" } };
        Assert.True(_scoring.ScoreItem(Mc(), r));
    }

    [Fact]
    public void Mc_wrong_choice_scores_false()
    {
        var r = new ItemResponse { SelectedOptionIds = { "a" } };
        Assert.False(_scoring.ScoreItem(Mc(), r));
    }

    [Fact]
    public void Multi_requires_exact_set()
    {
        var item = new Item { Id = "q", Type = "multi", Scoring = "auto", Correct = new() { "a", "b", "c" } };
        Assert.True(_scoring.ScoreItem(item, new ItemResponse { SelectedOptionIds = { "a", "b", "c" } }));
        Assert.False(_scoring.ScoreItem(item, new ItemResponse { SelectedOptionIds = { "a", "b" } }));        // missing one
        Assert.False(_scoring.ScoreItem(item, new ItemResponse { SelectedOptionIds = { "a", "b", "c", "d" } })); // extra
    }

    [Fact]
    public void Order_requires_exact_sequence()
    {
        var item = new Item { Id = "q", Type = "order", Scoring = "auto", Correct = new() { "b", "a", "c" } };
        Assert.True(_scoring.ScoreItem(item, new ItemResponse { OrderedOptionIds = { "b", "a", "c" } }));
        Assert.False(_scoring.ScoreItem(item, new ItemResponse { OrderedOptionIds = { "a", "b", "c" } }));
    }

    [Fact]
    public void Model_item_uses_self_assessment()
    {
        var item = new Item { Id = "q", Type = "short", Scoring = "model" };
        Assert.True(_scoring.ScoreItem(item, new ItemResponse { SelfAssessedCorrect = true }));
        Assert.False(_scoring.ScoreItem(item, new ItemResponse { SelfAssessedCorrect = false }));
        Assert.False(_scoring.ScoreItem(item, new ItemResponse { SelfAssessedCorrect = null }));
    }

    [Fact]
    public void Score_aggregates_and_applies_threshold()
    {
        var items = new List<Item>
        {
            new() { Id = "q1", Type = "mc", Scoring = "auto", Correct = new() { "b" } },
            new() { Id = "q2", Type = "mc", Scoring = "auto", Correct = new() { "b" } },
            new() { Id = "q3", Type = "mc", Scoring = "auto", Correct = new() { "b" } },
            new() { Id = "q4", Type = "mc", Scoring = "auto", Correct = new() { "b" } },
            new() { Id = "q5", Type = "mc", Scoring = "auto", Correct = new() { "b" } },
        };
        var responses = new Dictionary<string, ItemResponse>
        {
            ["q1"] = new() { SelectedOptionIds = { "b" } },
            ["q2"] = new() { SelectedOptionIds = { "b" } },
            ["q3"] = new() { SelectedOptionIds = { "b" } },
            ["q4"] = new() { SelectedOptionIds = { "b" } },
            ["q5"] = new() { SelectedOptionIds = { "a" } }, // wrong
        };

        var result = _scoring.Score(items, responses, threshold: 0.8);
        Assert.Equal(0.8, result.Score, 3);
        Assert.True(result.Passed);
        Assert.Single(result.WrongItems);
        Assert.Equal("q5", result.WrongItems[0].Id);
    }
}
