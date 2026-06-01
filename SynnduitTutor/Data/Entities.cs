namespace SynnduitTutor.Data;

/// <summary>
/// A learner. In the core slice this stands in for the Entra-authenticated identity; later the
/// <see cref="ExternalId"/> becomes the Entra object id / UPN.
/// </summary>
public sealed class Learner
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string ExternalId { get; set; } = "";   // stub now; Entra oid/UPN later
    public DateTime CreatedUtc { get; set; }
    public List<ConceptMastery> Mastery { get; set; } = new();
}

/// <summary>Per-learner, per-concept mastery state — the heart of gating.</summary>
public sealed class ConceptMastery
{
    public int Id { get; set; }
    public int LearnerId { get; set; }
    public Learner? Learner { get; set; }

    public string ConceptId { get; set; } = "";

    /// <summary>Best score (0..1) the learner has achieved on this concept's items.</summary>
    public double BestScore { get; set; }
    public bool Mastered { get; set; }

    /// <summary>For L0 placement: the concept was passed on first contact and is skipped.</summary>
    public bool SkippedByPlacement { get; set; }

    public int Attempts { get; set; }
    public int RemediationCycles { get; set; }
    public bool EscalatedToMentor { get; set; }

    /// <summary>Item ids already served, so re-assessment can pull fresh items.</summary>
    public string SeenItemIdsCsv { get; set; } = "";
    public DateTime? LastAttemptUtc { get; set; }
}
