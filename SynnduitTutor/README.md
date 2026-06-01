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
| `Services/ScoringService` | Deterministic auto-scoring; model items self-assessed for now |
| `Services/MasteryService` | Per-learner per-concept mastery state (EF Core / SQLite) |
| `Services/IRemediationService` | `StubRemediationService` now; `ClaudeRemediationService` later |
| `Services/LearnerSession` | Stub auth (current learner per circuit); Entra SSO later |
| `Components/Pages/` | `Home` (sign in), `Dashboard` (path), `Lesson`, `Quiz` |

## Next

1. **Entra SSO** — replace `LearnerSession` with `Microsoft.Identity.Web`; key `Learner.ExternalId` to the Entra object id.
2. **Live Claude remediation** — add `ClaudeRemediationService : IRemediationService` calling the Anthropic API, grounded in the lesson + the learner's wrong answers.
3. **Model-item grading** — let Claude grade short/scenario answers against the rubric instead of self-assessment.
