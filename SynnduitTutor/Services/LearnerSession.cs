namespace SynnduitTutor.Services;

/// <summary>
/// Holds the current learner for the Blazor circuit. This is the stub stand-in for authentication:
/// the Home page lets you pick/create a learner and sets this; a later Entra integration will set it
/// from the authenticated principal instead. State is per-circuit (resets on a full page reload).
/// </summary>
public sealed class LearnerSession
{
    public int? LearnerId { get; private set; }
    public string? DisplayName { get; private set; }

    /// <summary>Audit-only learner: lessons are viewable everywhere, but quizzes are disabled.</summary>
    public bool IsAuditor { get; private set; }

    public bool IsSignedIn => LearnerId is not null;

    /// <summary>Raised whenever the session changes (sign in/out, audit toggle) so components that
    /// derive state from it (e.g. the dashboard's gating overview) can reload and re-render.</summary>
    public event Action? Changed;

    public void SignIn(int learnerId, string displayName, bool isAuditor = false)
    {
        LearnerId = learnerId;
        DisplayName = displayName;
        IsAuditor = isAuditor;
        Changed?.Invoke();
    }

    /// <summary>Flip audit mode for the current circuit (e.g. a reviewer toggling it from the nav).</summary>
    public void SetAuditor(bool isAuditor)
    {
        if (IsAuditor == isAuditor) return;
        IsAuditor = isAuditor;
        Changed?.Invoke();
    }

    public void SignOut()
    {
        LearnerId = null;
        DisplayName = null;
        IsAuditor = false;
        Changed?.Invoke();
    }
}
