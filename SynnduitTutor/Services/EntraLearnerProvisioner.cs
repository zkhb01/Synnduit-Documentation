using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace SynnduitTutor.Services;

/// <summary>
/// On circuit open, maps the Entra-authenticated principal to a <c>Learner</c> row (keyed by the
/// Entra object id) and signs the per-circuit <see cref="LearnerSession"/> in. The rest of the app
/// already keys off <see cref="LearnerSession"/>, so nothing downstream changes. Registered only
/// when Entra is configured (see <see cref="EntraAuth"/>).
/// </summary>
public sealed class EntraLearnerProvisioner : CircuitHandler
{
    private readonly AuthenticationStateProvider _auth;
    private readonly MasteryService _mastery;
    private readonly LearnerSession _session;

    public EntraLearnerProvisioner(
        AuthenticationStateProvider auth, MasteryService mastery, LearnerSession session)
    {
        _auth = auth;
        _mastery = mastery;
        _session = session;
    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken ct)
    {
        if (_session.IsSignedIn) return;

        var user = (await _auth.GetAuthenticationStateAsync()).User;
        if (user.Identity?.IsAuthenticated != true) return;

        // Entra emits the stable user id as "oid" (short) or the full URI claim type.
        var oid = user.FindFirst("oid")?.Value
                  ?? user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        if (string.IsNullOrWhiteSpace(oid)) return;

        var name = user.Identity?.Name
                   ?? user.FindFirst("name")?.Value
                   ?? user.FindFirst("preferred_username")?.Value
                   ?? "Learner";

        var learner = await _mastery.GetOrCreateLearnerAsync(name, "entra:" + oid);
        _session.SignIn(learner.Id, learner.DisplayName);
    }
}
