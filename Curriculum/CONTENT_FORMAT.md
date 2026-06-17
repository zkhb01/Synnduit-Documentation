# Curriculum content format

> How the authored content is laid out and shaped, so lessons and quiz banks stay
> consistent as concepts are added. Companion to `CurriculumBlueprint.md` (design)
> and `concept-graph.json` (gating-engine data).

## Layout

```
Curriculum/
├── CurriculumBlueprint.md     ← design: concepts, prereqs, mastery model
├── concept-graph.json         ← gating-engine data (prereqs, thresholds, itemPoolId)
├── CONTENT_FORMAT.md          ← this file
├── lessons/
│   └── <conceptId>.md         ← one canonical lesson per concept (e.g. L1.1.md)
└── items/
    └── <conceptId>.items.json ← one quiz bank per concept; filename == itemPoolId
```

The `itemPoolId` in `concept-graph.json` (e.g. `L1.1.items`) maps 1:1 to a file
`items/L1.1.items.json`. The lesson for the same concept is `lessons/L1.1.md`.

## Lesson file (`lessons/<conceptId>.md`)

A reviewed, canonical lesson — the human-authored backbone. Claude re-teaches from a
*different* angle on remediation; this is the first, authoritative pass. Keep it tight
(a learner should read it in 2–4 minutes). Front-matter mirrors the concept graph so a
loader can validate the two stay in sync.

```markdown
---
conceptId: L1.1
name: What Synnduit is
level: L1
prereqs: []
objective: <copied from concept-graph.json>
sources: [README.md#1-what-is-synnduit]
---

# L1.1 — What Synnduit is

## In one line
<the single sentence a learner must be able to say>

## The idea
<canonical explanation, grounded in the cited sources, with one concrete CSSD example>

## Why it matters
<what breaks / what you can't do if you don't get this>

## Watch out for
<the common misconception this concept is most often confused with — mirrors the
 remediationAngle in the concept graph>

## You've got this when you can…
<1–3 bullet "can do" statements; the quiz bank tests exactly these>
```

### L0 lesson variant (placement diagnostic)

L0 concepts confirm generic .NET skills, so they follow the blueprint's sourcing rule —
**link out for the generic skill, author only the Synnduit-application slice.** Front-matter
carries the dual objectives + external pointer; the body curates rather than re-teaches the
generic part:

```markdown
---
conceptId: L0.G1
name: Using generic types
level: L0
prereqs: []
genericObjective: <copied from concept-graph.json>
synnduitObjective: <copied from concept-graph.json>
externalSource: Microsoft Learn - C# generics
externalUrl: https://learn.microsoft.com/dotnet/csharp/fundamentals/types/generics
externalVideo: https://www.youtube.com/watch?v=...          # optional, curated video
externalVideoPulled: 2026-06-17                            # date the link was sourced/checked
externalVideoPublished: 2024-03 (approx)                   # optional; the video's own age, approximate
authoredSource: Sync-SRC-DDH StaffFeed.cs
skipOnPass: true
synnduitCritical: false
---

# L0.G1 — Using generic types

## Generic skill (curated — don't re-teach)
<one short paragraph + the external link; "if you're solid here the placement item passes
 and you skip ahead">

📺 Video: [Title](externalVideo URL) — pulled <date> · published ~<month year>
<!-- Curated video for the generic skill. Always carry a `pulled` date so a learner can judge
     staleness; verify the link by hand (YouTube links rot). Omit the whole line if there's no
     good external video (e.g. the Synnduit-specific nullable-POCO rule in L0.E2). -->

## The Synnduit slice
<the authored teaching: how this generic skill shows up in real Synnduit code, grounded in
 authoredSource / the extracted docs>

## Placement check
<what the cursory item verifies — pass = adequate, skipped; miss = remediation + re-check>

## If this is shaky
<the remediationAngle from the concept graph>
```

`synnduitCritical: true` concepts (L0.G2, L0.D2, L0.E2) get a fuller Synnduit slice and one
extra item — they're where otherwise-strong .NET devs stumble (MEF, the nullable-POCO rule).

## Quiz bank (`items/<conceptId>.items.json`)

Machine-scorable item pool. The gating engine samples from `items[]`; re-assessment
pulls *fresh* items (so author more than the gate samples — aim 5–8 per concept).

```jsonc
{
  "poolId": "L1.1.items",
  "conceptId": "L1.1",
  "conceptName": "What Synnduit is",
  "items": [
    {
      "id": "L1.1.q1",
      "type": "mc",                 // mc | multi | classify | order | match | short | scenario
      "difficulty": "recall",       // recall | apply | synthesis
      "scoring": "auto",            // auto (objective) | model (Claude/rubric grades free text)
      "stem": "Synnduit is best described as…",
      "options": [                  // present for mc/multi/classify/order/match
        { "id": "a", "text": "a one-time data import tool" },
        { "id": "b", "text": "a continuous sync engine that tracks changes" },
        { "id": "c", "text": "a reporting dashboard" }
      ],
      "correct": ["b"],             // array of option id(s); ordered list for type "order"
      "explanation": "Why b is right — the one sentence to reinforce on review.",
      "misconceptions": {           // optional: per-wrong-option diagnosis, fed to remediation
        "a": "Confusing sync (ongoing) with import (one-time).",
        "c": "The dashboard sits on top of Synnduit; it isn't Synnduit."
      },
      "source": "README.md#1-what-is-synnduit"
    },
    {
      "id": "L1.1.q2",
      "type": "short",
      "difficulty": "apply",
      "scoring": "model",           // free text → graded by rubric (Claude in the tool)
      "stem": "Explain the phrase \"ETL meets version control\" in one or two sentences.",
      "rubric": [                   // each entry is a must-hit point; partial credit allowed
        "ETL/move-and-transform-data part",
        "version-control/change-history-and-remembered-links part"
      ],
      "sampleAnswer": "It moves data like ETL, but also records every field change and remembers the source↔destination link, like version control.",
      "source": "README.md#1-what-is-synnduit"
    }
  ]
}
```

### Item-type notes
- **mc** — exactly one correct option. **multi** — one or more; `correct` lists all.
- **classify** — `stem` lists things to bucket; model items work best, or encode as several `mc`.
- **order** — `correct` is the option ids in the required order.
- **match** — `options` are left items; encode right items + the answer key in `correct`
  as `["a:2","b:1",...]` (left id : right id).
- **short / scenario** — `scoring: "model"`; always supply `rubric` + `sampleAnswer`.

**Gating vs. practice.** `scoring: "auto"` items (mc/multi/order) are the only ones that count toward
the mastery score and the pass/fail gate. `scoring: "model"` items (short/scenario, and free-text
classify/match) are **practice**: they're shown and given per-item feedback (Claude against the
`rubric`, or learner self-check), but a right/wrong verdict never decides advancement — so an
imperfect free-text grade can't wrongly pass or block a learner. Author each concept with **at least
a few auto items** so its gate is decidable on objective scoring alone; use model items for the
open-ended reflection/application that auto items can't capture.

### Authoring rules
- Every item carries a `source` (file + anchor) — no ungrounded questions.
- Spread difficulty: at least one `recall`, one `apply`, and (for synthesis concepts
  L1.6/L1.10/L1.11) one `synthesis` item.
- Distractors must be *plausible* and map to a real misconception (put it in `misconceptions`).
- IDs are stable: `<conceptId>.q<N>`. Never renumber; append new items with the next N.
