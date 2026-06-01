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

    public bool IsSignedIn => LearnerId is not null;

    public void SignIn(int learnerId, string displayName)
    {
        LearnerId = learnerId;
        DisplayName = displayName;
    }

    public void SignOut()
    {
        LearnerId = null;
        DisplayName = null;
    }
}
