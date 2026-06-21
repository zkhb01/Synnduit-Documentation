using System.Text.Json.Serialization;

namespace SynnduitTutor.Models;

/// <summary>
/// Mirrors Curriculum/concept-graph.json. Fields are intentionally permissive: L1 concepts
/// carry <see cref="Concept.Objective"/>, while L0 concepts use the generic/Synnduit split and
/// <see cref="Concept.SkipOnPass"/>. Unused fields stay null.
/// </summary>
public sealed class ConceptGraph
{
    public string? Version { get; set; }
    public string? Generated { get; set; }
    public MasteryModel MasteryModel { get; set; } = new();
    public List<string> PathOrder { get; set; } = new();
    public List<Level> Levels { get; set; } = new();

    /// <summary>All concepts across every level, flattened.</summary>
    [JsonIgnore]
    public IEnumerable<Concept> AllConcepts => Levels.SelectMany(l => l.Concepts);

    public Concept? FindConcept(string id) => AllConcepts.FirstOrDefault(c => c.Id == id);
    public Level? FindLevel(string id) => Levels.FirstOrDefault(l => l.Id == id);
    public Level? LevelOf(Concept c) => Levels.FirstOrDefault(l => l.Concepts.Contains(c));
}

public sealed class MasteryModel
{
    public double DefaultThreshold { get; set; } = 0.8;
    public bool ReassessmentUsesFreshItems { get; set; } = true;
    public int RemediationCycleLimitBeforeEscalation { get; set; } = 2;
    public string? EscalationTarget { get; set; }
    public string? ContentStrategy { get; set; }
}

public sealed class Level
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int Order { get; set; }
    public string? Type { get; set; }       // conceptual | placement
    public string? Gate { get; set; }
    public string? GatesLevel { get; set; }
    public string? SourcingRule { get; set; }
    public List<Concept> Concepts { get; set; } = new();

    /// <summary>L0 placement levels skip a passed concept instead of remediating.</summary>
    [JsonIgnore]
    public bool IsPlacement => string.Equals(Type, "placement", StringComparison.OrdinalIgnoreCase);
}

public sealed class Concept
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public List<string> Prereqs { get; set; } = new();
    public string ItemPoolId { get; set; } = "";
    public double MasteryThreshold { get; set; } = 0.8;
    public string? RemediationAngle { get; set; }

    // L1-style
    public string? Objective { get; set; }
    public List<string>? Sources { get; set; }

    // L0-style (generic skill curated externally + authored Synnduit slice)
    public string? GenericObjective { get; set; }
    public string? SynnduitObjective { get; set; }
    public string? ExternalSource { get; set; }
    public string? AuthoredSource { get; set; }
    public bool SkipOnPass { get; set; }
    public bool SynnduitCritical { get; set; }
    public string? AuthorPriority { get; set; }

    /// <summary>Best single-line objective regardless of level shape.</summary>
    [JsonIgnore]
    public string DisplayObjective => Objective ?? SynnduitObjective ?? GenericObjective ?? Name;
}
