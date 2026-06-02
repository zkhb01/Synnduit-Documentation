namespace SynnduitTutor.Services;

/// <summary>Donut incentive configuration, bound from the "Donuts" section of appsettings.json.</summary>
public sealed class DonutOptions
{
    /// <summary>Placeholder until the real manager is set; printed on the voucher.</summary>
    public string ManagerName { get; set; } = "<Manager Name>";

    /// <summary>Minimum unredeemed donut points required to print a voucher.</summary>
    public int RedeemThresholdPoints { get; set; } = 6;

    public DonutAwardOptions Award { get; set; } = new();
}

public sealed class DonutAwardOptions
{
    /// <summary>Base award for mastering a concept.</summary>
    public int ConceptMastered { get; set; } = 1;

    /// <summary>Bonus for mastering on the first attempt (no remediation) — rewards getting it right.</summary>
    public int FirstAttemptBonus { get; set; } = 1;

    /// <summary>Bonus for a perfect (100%) score — rewards excellence.</summary>
    public int PerfectScoreBonus { get; set; } = 1;

    /// <summary>Bonus for mastering a Synnduit-critical concept (the genuinely hard ones).</summary>
    public int CriticalConceptBonus { get; set; } = 1;

    /// <summary>Bonus when a whole level's gate is cleared — the milestone.</summary>
    public int LevelCompletionBonus { get; set; } = 3;
}
