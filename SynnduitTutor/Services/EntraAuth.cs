using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Identity.Web;

namespace SynnduitTutor.Services;

/// <summary>
/// Microsoft Entra ID (Azure AD) single sign-on, scaffolded so it activates only when an app
/// registration is configured. With no <c>AzureAd:ClientId</c> the app keeps the local stub sign-in
/// (Home page), so it still runs and tests pass with zero credentials. Fill the AzureAd section
/// (and put the client secret in user-secrets / an env var) to turn on real SSO.
/// </summary>
public static class EntraAuth
{
    /// <summary>True when an Entra app registration is configured (AzureAd:ClientId present).</summary>
    public static bool IsEntraConfigured(this IConfiguration config) =>
        !string.IsNullOrWhiteSpace(config["AzureAd:ClientId"]);

    /// <summary>Registers OpenID Connect sign-in + the learner-provisioning bridge when configured.</summary>
    public static void AddEntraAuth(this WebApplicationBuilder builder)
    {
        if (!builder.Configuration.IsEntraConfigured()) return;

        builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();

        // Bridge the authenticated principal into the app's LearnerSession at circuit open.
        builder.Services.AddScoped<CircuitHandler, EntraLearnerProvisioner>();
    }

    /// <summary>Adds the auth middleware + minimal sign-in/sign-out endpoints when configured.</summary>
    public static void UseEntraAuth(this WebApplication app)
    {
        if (!app.Configuration.IsEntraConfigured()) return;

        app.UseAuthentication();
        app.UseAuthorization();

        // Minimal endpoints — no Razor Pages UI package needed.
        app.MapGet("/auth/login", (string? returnUrl) =>
            Results.Challenge(
                new AuthenticationProperties { RedirectUri = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl },
                [OpenIdConnectDefaults.AuthenticationScheme]));

        app.MapGet("/auth/logout", () =>
            Results.SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));
    }
}
