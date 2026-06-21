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

    /// <summary>The course (curriculum) this mastery row belongs to. A learner has independent
    /// progress per course, so gating/mastery is always scoped by (LearnerId, CourseId, ConceptId).</summary>
    public string CourseId { get; set; } = "";

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

/// <summary>
/// One earned-donut ledger entry (append-only). Earned the moment a concept is first mastered;
/// <see cref="VoucherId"/> stays null until the donuts are redeemed onto a printed voucher.
/// </summary>
public sealed class DonutAward
{
    public int Id { get; set; }
    public int LearnerId { get; set; }
    public Learner? Learner { get; set; }

    /// <summary>The course this award was earned in.</summary>
    public string CourseId { get; set; } = "";

    /// <summary>Mastered | FirstAttempt | Perfect | Critical | LevelComplete</summary>
    public string Reason { get; set; } = "";
    public string? ConceptId { get; set; }
    public string? LevelId { get; set; }
    public int Points { get; set; }
    public DateTime EarnedUtc { get; set; }

    /// <summary>Null = unredeemed (counts toward the next voucher); set once redeemed.</summary>
    public int? VoucherId { get; set; }
}

/// <summary>A redeemed, printable donut voucher — handed to the manager to buy the real donuts.</summary>
public sealed class Voucher
{
    public int Id { get; set; }
    public int LearnerId { get; set; }
    public Learner? Learner { get; set; }

    /// <summary>The course whose donuts this voucher redeems.</summary>
    public string CourseId { get; set; } = "";

    public string Code { get; set; } = "";
    public int Points { get; set; }
    public string Milestone { get; set; } = "";
    public string ManagerName { get; set; } = "";   // snapshot of the configured manager at issue time
    public DateTime IssuedUtc { get; set; }
}
