using System.Text.Json.Serialization;

namespace SynnduitTutor.Models;

/// <summary>Mirrors a Curriculum/items/&lt;poolId&gt;.items.json file.</summary>
public sealed class ItemBank
{
    public string PoolId { get; set; } = "";
    public string ConceptId { get; set; } = "";
    public string ConceptName { get; set; } = "";
    public List<Item> Items { get; set; } = new();
}

public sealed class Item
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";          // mc | multi | order | match | classify | short | scenario
    public string Difficulty { get; set; } = "";    // recall | apply | synthesis
    public string Scoring { get; set; } = "auto";    // auto | model
    public string Stem { get; set; } = "";
    public List<ItemOption>? Options { get; set; }
    public List<string>? Correct { get; set; }       // option ids; ordered for "order"; "left:right" for "match"
    public string? Explanation { get; set; }
    public Dictionary<string, string>? Misconceptions { get; set; }
    public List<string>? Rubric { get; set; }
    public string? SampleAnswer { get; set; }
    public string? Source { get; set; }

    [JsonIgnore]
    public bool IsAutoScored => string.Equals(Scoring, "auto", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// The input mechanism to render, collect, and score by — derived from the item's data rather
    /// than trusting the authored <see cref="Type"/> label alone. An auto-scored item that carries
    /// options is a choice question regardless of a narrative type tag (e.g. "scenario"): one correct
    /// answer resolves to "mc", several to "multi". Explicit choice types and ordered items pass
    /// through unchanged; everything else stays free-text (model-scored). This keeps a mislabeled item
    /// working instead of silently falling through to a textarea it can never be auto-scored from.
    /// </summary>
    [JsonIgnore]
    public string EffectiveType
    {
        get
        {
            if (Type is "mc" or "multi" or "order") return Type;
            if (IsAutoScored && Options is { Count: > 0 })
                return (Correct?.Count ?? 0) > 1 ? "multi" : "mc";
            return Type;
        }
    }

    [JsonIgnore]
    public bool IsSingleChoice => EffectiveType is "mc";

    [JsonIgnore]
    public bool IsOrdered => EffectiveType is "order";

    /// <summary>
    /// For an auto-scored item, a human-readable reason it could never be scored correctly (so the
    /// curriculum load can reject it loudly), or null when it is well-formed. Model-scored items always
    /// return null — they're graded by rubric, not option data. Inference (<see cref="EffectiveType"/>)
    /// rescues a mislabeled-but-well-formed item; this catches the ones inference cannot save.
    /// </summary>
    [JsonIgnore]
    public string? AutoScoringError
    {
        get
        {
            if (!IsAutoScored) return null;
            var et = EffectiveType;
            if (et is not ("mc" or "multi" or "order"))
                return $"auto-scored item has type '{Type}' with no options/correct to score against " +
                       "— give it a choice type (mc/multi/order) with options+correct, or set scoring to \"model\".";
            if (Options is not { Count: > 0 }) return $"auto-scored '{et}' item has no options.";
            if (Correct is not { Count: > 0 }) return $"auto-scored '{et}' item has no correct answer(s).";
            return null;
        }
    }

    /// <summary>
    /// For a model-scored item, a reason it can't give the learner meaningful feedback - it has
    /// neither a rubric nor a sample answer, so the self-check panel renders empty - or null when
    /// it's well-formed. Auto-scored items always return null here (see <see cref="AutoScoringError"/>).
    /// </summary>
    [JsonIgnore]
    public string? ModelScoringError
    {
        get
        {
            if (IsAutoScored) return null;
            var hasRubric = Rubric is { Count: > 0 };
            var hasSample = !string.IsNullOrWhiteSpace(SampleAnswer);
            if (!hasRubric && !hasSample)
                return $"model-scored '{Type}' item has neither a rubric nor a sampleAnswer - the self-check panel would be empty; add at least one, or make it an auto-scored choice item.";
            return null;
        }
    }
}

public sealed class ItemOption
{
    public string Id { get; set; } = "";
    public string Text { get; set; } = "";
}
