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

    [JsonIgnore]
    public bool IsSingleChoice => Type is "mc";

    [JsonIgnore]
    public bool IsOrdered => Type is "order";
}

public sealed class ItemOption
{
    public string Id { get; set; } = "";
    public string Text { get; set; } = "";
}
