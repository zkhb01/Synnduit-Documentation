# Synnduit Tutor

A Blazor (ASP.NET Core, .NET 9) training tool that serves the authored curriculum in
`../Curriculum/` — gating concepts by prerequisite, scoring quizzes, tracking per-learner
mastery, and offering remediation on a miss.

This is the **core engine slice**: authentication and Claude-powered remediation are stubbed
(clearly labelled in the UI) so the gating/scoring/mastery loop can be proven first.

## Run

```bash
dotnet run --project SynnduitTutor
```

Then open the printed URL (e.g. `https://localhost:7xxx`). On first run it creates a SQLite
database at `SynnduitTutor/App_Data/tutor.db` and locates `Curriculum/` automatically by
walking up from the content root.

Flow: **Sign in** (pick/create a learner — stub auth) → **My path** (concept map with
locked / available / mastered status + level gates) → open a concept's **lesson** →
**check understanding** (quiz). Passing the threshold (default 80%) masters the concept and
unlocks its dependents; a miss shows guided remediation and offers fresh questions.

## Test

```bash
dotnet test SynnduitTutor.Tests
```

Covers scoring (mc/multi/order/model), gating (prereq locking, placement-skip, level gates),
and mastery (mastery on pass, L0 first-attempt skip-on-pass, escalation after repeated fails).

## Architecture

| Piece | Responsibility |
|---|---|
| `Models/` | `ConceptGraph`, `ItemBank`, `Lesson` — shapes of the authored content |
| `Services/CurriculumStore` | Loads + caches concept-graph.json, items/*.json, lessons/*.md |
| `Services/GatingEngine` | Pure: concept status (locked/available/mastered/skipped) + level gates |
| `Services/ScoringService` | Deterministic auto-scoring (mc/multi/order) |
| `Services/IAnswerGrader` | Free-text grading: `StubAnswerGrader` (self-assess) or `ClaudeAnswerGrader` (rubric, when a key is set) |
| `Services/MasteryService` | Per-learner per-concept mastery state (EF Core / SQLite) |
| `Services/IRemediationService` | `StubRemediationService`, or `ClaudeRemediationService` when an Anthropic key is configured |
| `Services/LearnerSession` | Current learner per circuit — set by the stub Home picker, or by `EntraLearnerProvisioner` from the Entra principal when `AzureAd` is configured |
| `Services/EntraAuth` | Config-gated Entra SSO wiring (OIDC + `/auth/login` & `/auth/logout`); no-op without `AzureAd:ClientId` |
| `Components/Pages/` | `Home` (sign in), `Dashboard` (path), `Lesson`, `Quiz` |

## Next

1. **Top up thin item banks** — several L0 banks have only 4 items; aim for 5–8 so re-assessment always has fresh questions.

Done: live Claude remediation (`ClaudeRemediationService`) and rubric grading of free-text items (`ClaudeAnswerGrader`) — both activate when an Anthropic API key is configured (`Anthropic:ApiKey` or `ANTHROPIC_API_KEY`).

## Enabling Entra SSO

SSO is scaffolded and **config-gated** — it stays off (local stub sign-in) until you provide an app registration:

1. Register an app in Microsoft Entra ID. Add a **Web** redirect URI `https://<host>/signin-oidc` and a front-channel logout URL `https://<host>/signout-callback-oidc`. Create a client secret.
2. Fill `AzureAd` in `appsettings.json` (`TenantId`, `ClientId`, `Domain`); put the secret in user-secrets or an env var — `dotnet user-secrets set "AzureAd:ClientSecret" "<secret>"`.
3. Run. The Home page now shows **Sign in with Microsoft**; on callback, `EntraLearnerProvisioner` maps the principal to a `Learner` (keyed `entra:<oid>`) and signs in `LearnerSession`. Sign out via `/auth/logout`.

With `AzureAd:ClientId` empty, none of the auth middleware/endpoints are added and the stub picker remains.
