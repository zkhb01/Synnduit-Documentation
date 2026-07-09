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

## Deploy (on-prem IIS)

One app serves **both** courses — `synnduit` (`Curriculum/`) and `sdm` (`Curriculum-SDM/`), configured in
the `Courses` array. The intended topology is co-hosting under the existing SDM site at **`/Tutor`**: SDM's
`Training:TutorBaseUrl` is `/Tutor` in prod and its Training menu links each course as `/Tutor/<courseId>`.

The `.csproj` links both curriculum folders in as **publish content** (`CopyToPublishDirectory`), so a file
publish carries `Curriculum/` and `Curriculum-SDM/` into the output root; the app resolves those folder names
against the content root with no path config. (Local `dotnet run` is unaffected — the content only copies on
publish, and dev still walks up to the source folders.)

**Server prerequisites (one-time):**

1. Install the **.NET 9 Hosting Bundle** (ASP.NET Core Module v2 + runtime), then `iisreset`.
2. Enable the **WebSocket Protocol** IIS feature — Blazor Server's circuit needs it (else it degrades to long-polling).

**Each deploy:**

1. Publish `SynnduitTutor` in **Release** (your usual VS file-publish profile). Both `Curriculum/` and
   `Curriculum-SDM/` appear in the publish folder automatically.
2. In the published `appsettings.json`, set the sub-path so in-app links resolve under the prefix:
   ```jsonc
   "Hosting": { "PathBase": "/Tutor" }
   ```
   Leave the `Courses` paths as the folder names (`Curriculum` / `Curriculum-SDM`) — they resolve at the
   publish root. Keep `Anthropic:ApiKey` and `AzureAd` empty here; supply them via env vars / user-secrets
   to enable live remediation / Entra SSO (the app falls back to stubs without them).
3. Copy the publish folder to the server and register it as an **IIS Application** named `Tutor` **under the
   SDM site**, with an app pool set to **No Managed Code**. `PathBase`, the IIS alias, and SDM's
   `TutorBaseUrl` must all agree on `/Tutor`.

   **IIS launches the app for you** via the `web.config` (ASP.NET Core Module) that publish drops in the
   folder — you do **not** run it yourself. `dotnet run` is for source only and won't work against publish
   output; the published entry point is the assembly `SynnduitTutor.dll` (the folder is named `Tutor`, the
   assembly isn't). To boot-check a copied publish folder from the console before wiring up IIS:
   ```powershell
   cd <publish-folder>
   dotnet .\SynnduitTutor.dll   # look for "Loaded 2 training course(s): synnduit, sdm"; Ctrl+C to stop
   ```
   A `307` redirect to https on that console run is expected (`UseHttpsRedirection`) — it means the app booted.
4. Grant the app pool identity (e.g. `IIS AppPool\Tutor`) **Modify** on `App_Data/` — the SQLite mastery
   store (`tutor.db`) is created and written there at runtime.

**Redeploys:** overwrite the app folder, but do **not** enable "remove additional files at destination" in the
publish profile — that would wipe `App_Data/tutor.db`. Content re-ships on every publish; the DB persists.

Verify by browsing `https://<sdm-host>/Tutor/synnduit` and `/Tutor/sdm` directly (both should load their concept
maps), then via the SDM **Training** menu — each course opens with the signed-in user preloaded (`?name=&id=`).

To host it as its **own** IIS site instead (own hostname), set `PathBase` to `""` and point SDM's
`Training:TutorBaseUrl` at the absolute origin.

## Architecture

| Piece | Responsibility |
|---|---|
| `Models/` | `ConceptGraph`, `ItemBank`, `Lesson` — shapes of the authored content |
| `Services/CurriculumStore` | Loads + caches concept-graph.json, items/*.json, lessons/*.md |
| `Services/GatingEngine` | Pure: concept status (locked/available/mastered/skipped) + level gates |
| `Services/ScoringService` | Deterministic auto-scoring (mc/multi/order); gates on auto items only — model items are practice (feedback, not scored) |
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
