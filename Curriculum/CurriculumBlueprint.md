# Synnduit AI-Tutor — Curriculum Authoring Blueprint

> Authoring blueprint for the Synnduit developer course (AI tutor). Defines the
> concept graphs for **Level 1 (Foundations)** and **Level 0 (Prerequisites)**, the
> shared mastery/remediation model, and how the gating engine + Claude consume them.
>
> Companion file: `concept-graph.json` (same data, shaped for the gating engine).
>
> Source material: this repo's `README.md`, `Documentation/`, `DiagnosticScripts/`,
> `ExceptionHistoryWithResolutions/`, plus `Synnduit-Database` (StarBridge schema),
> `SynnduitDashboard`, the Synnduit engine source, and the CSSD example repos.

---

## 1. How to use this doc

- **Curriculum authors** read the concept blocks to write the canonical lessons + quiz banks.
- **The gating engine** consumes `concept-graph.json` for prerequisite ordering and mastery gating.
- **Claude (remediation)** is handed a single concept's `objective` + `remediationAngle` + `source` + the learner's wrong answer, and re-teaches only that concept.

Keep content **hybrid**: a reviewed canonical lesson + quiz bank per concept is the backbone; Claude supplies the adaptive layer (misconception diagnosis, re-teaching from a new angle, fresh practice, live Q&A).

---

## 2. Learner path

```
L1 Foundations (conceptual, open to all)
   → L0 Prerequisites (placement diagnostic; gates the coding levels)
      → L2 Coding (write feeds, sinks, POCOs)   [future]
         → L3 Operations & troubleshooting       [future]
```

L1 is deliberately code-free, so it has no prerequisites. L0 is a **placement diagnostic** that gates L2+ (where learners write code). A confident dev clears L0 quickly; gaps trigger targeted remediation.

---

## 3. Shared mastery & remediation model

- **Mastery threshold:** ≥ 80% on a concept's mapped items = mastered.
- **Level gate:** all concepts in the level mastered.
- **Re-assessment:** pulls *different* items from the same concept's pool (no repeats).
- **Remediation loop:** assess → diagnose weak concept(s) → targeted re-teach → re-assess → gate.
- **Escalation:** after 2 failed remediation cycles on one concept → route to a human mentor.
- **Placement (L0 only):** `skipOnPass` — a passed concept is marked adequate and skipped, not remediated.
- **Division of labor:** the app owns identity (Entra SSO), the concept graph, per-learner per-concept mastery state, gating, and the UI. Claude owns adaptive content (diagnosis, re-teaching, fresh items, Q&A), grounded in the knowledge base with the learner's mastery state as context.

---

## 4. Level 1 — Synnduit Foundations

**Goal:** Build the mental model and vocabulary to read what a sync does and explain how Synnduit works — no implementation skill required.

### Dependency graph

```
L1.1 What Synnduit is
   └─▶ L1.2 Source vs Destination
          └─▶ L1.3 Engine model (you plug things in)
                 ├─▶ L1.4 Delta detection (snapshot→delta)
                 │      ├─▶ L1.6 Mapping (source↔dest link) ◀─┐
                 │      ├─▶ L1.8 iFeed & iSink (connectors)    │
                 │      └─▶ L1.10 Transaction outcomes ◀───────┤ (needs L1.6)
                 ├─▶ L1.5 StarBridge (bookkeeping DB) ─────────┘
                 │      └─▶ L1.6 Mapping (also needs L1.5)
                 └─▶ L1.7 Run & Segment
                        └─▶ L1.12 Thresholds & resumability
          L1.8 ─▶ L1.9 POCO & attributes
          L1.9 + L1.6 ─▶ L1.11 Dedup & merge
```

Entry: **L1.1**. Synthesis: **L1.10, L1.11, L1.12**.

### Concepts

**L1.1 — What Synnduit is** · prereqs: none · source: README §1
Objective: State that Synnduit is a long-running data **synchronization engine** (not a one-time copy/ETL); explain "ETL meets version control."
Sample items: (MC) "Synnduit is: (a) one-time import (b) continuous sync engine that tracks changes ✓ (c) a dashboard." (Short) "What does 'ETL meets version control' mean?"
Remediation: contrast against a naive nightly copy script — show duplicates / no history, then how Synnduit's remembered links + change log fix it.

**L1.2 — Source vs. Destination** · prereqs: L1.1 · source: README §1, §3
Objective: Define source (where data originates) and destination (where it lands; at CSSD = DDH); classify real systems.
Sample items: (Classify) "Source or Destination: PowerSchool, DDH, HRAD, ServiceNow." (MC) "CSSD destination = DDH ✓."
Remediation: use the §5.1 CSSD table as concrete grounding.

**L1.3 — The engine model** · prereqs: L1.2 · source: README §2
Objective: You don't modify the engine — you implement a source connector and a destination connector; the engine computes & applies the delta.
Sample items: (MC) "To add an integration you: (a) modify Synnduit (b) implement feed + sink, engine diffs ✓." (Label) "SOURCE → ___ → DESTINATION."
Remediation: walk the §2 ASCII diagram; "engine fixed, connectors yours."

**L1.4 — Delta detection** · prereqs: L1.3 · source: README §2, §4
Objective: The connector emits a full snapshot; the engine processes only what changed since last run.
Sample items: (Scenario) "Feed returns all 5,000 staff each run — does the destination get 5,000 updates each time? Why not?"
Remediation: "snapshot in, delta out" — engine diffs your snapshot against last run's recorded state.

**L1.5 — StarBridge** · prereqs: L1.3 · source: README §2, §3
Objective: StarBridge is the SQL DB storing mappings, transactions, value changes, exceptions; one per Run.
Sample items: (MC) "Bookkeeping lives in: (a) destination tables (b) StarBridge ✓." (List) "Name two things StarBridge stores."
Remediation: the diagram's StarBridge box; relate each stored thing to a later concept.

**L1.6 — Mapping** · prereqs: L1.4, L1.5 · source: README §1, §3
Objective: A mapping is the persisted link between source ID and destination ID; it's why the same record updates, not duplicates, next run.
Sample items: (Scenario) "Run 1 creates Staff X; run 2 sees X again — why no duplicate?"
Remediation: two-run timeline — run 1 writes a mapping row; run 2 looks it up → Update not Create.

**L1.7 — Run & Segment** · prereqs: L1.3 · source: README §3, §5.4
Objective: A Run syncs a set of entities; a Segment is one step — Migration or GarbageCollection. Order matters (parents before children in; reverse for GC).
Sample items: (MC) "Which removes destination records whose source disappeared — GarbageCollection ✓." (Order) "Why migrate School before Student but GC Student before School?"
Remediation: the §5.4 segment list; foreign-key ordering analogy.

**L1.8 — iFeed & iSink** · prereqs: L1.3, L1.4 · source: README §3, §5.6
Objective: `IFeed<TEntity>` = source connector (snapshot); `ISink<TEntity>` = destination connector (New/Create/Get/Update/Delete). You write these.
Sample items: (MC) "Which reads from source — iFeed ✓." (MC) "Create/Update/Delete live on iSink ✓."
Remediation: map onto L1.3's connector boxes; show `StaffFeed.LoadEntities()` shape.

**L1.9 — POCO & attributes** · prereqs: L1.8 · source: README §3, §5.5
Objective: A POCO is the C# class for one entity type; attributes form the contract — `[EntityType]`, `[SourceSystemIdentifier]`, `[DestinationSystemIdentifier]`, `[EntityProperty]`, `[DuplicationKey]`.
Sample items: (Match) "Pair each attribute to its job." (MC) "`[EntityProperty]` = field tracked for change detection ✓."
Remediation: annotate `Staff.cs` line-by-line, one attribute at a time.

**L1.10 — Transaction outcomes** · prereqs: L1.4, L1.6 · source: README §3, §6
Objective: Every record in every run gets an outcome (NewEntityCreated, ChangesDetectedAndMerged, NoChangesDetected, Skipped, Rejected, ExceptionThrown…); outcomes are how you read a run.
Sample items: (Scenario) "Run 1 creates a record; run 2 source unchanged — outcome?" (NoChangesDetected.) (MC) "Where do per-record outcomes live? (EntityTransaction / console.)"
Remediation: tie to delta detection (no change → NoChangesDetected); show a console summary block.

**L1.11 — Dedup & merge (conceptual)** · prereqs: L1.6, L1.9 · source: README §4
Objective: A `[DuplicationKey]` lets the engine match a "new" source record to an existing destination record and merge, instead of duplicating.
Sample items: (Scenario) "New source row matches existing dest by EmployeeId — what happens?" (Matched & merged.) (MC) "Dedup driven by `[DuplicationKey]` ✓."
Remediation: connect mapping (L1.6) + DuplicationKey (L1.9) — "no mapping yet, but the key finds the twin."

**L1.12 — Thresholds & resumability** · prereqs: L1.7 · source: README §4
Objective: Safety features — abort thresholds (deletion/orphan/exception %) protect the destination from a bad feed; runs are resumable after a crash.
Sample items: (Scenario) "Source bug returns 0 rows — what stops the destination being wiped?" (GC/orphan threshold abort.)
Remediation: the §5.4 threshold settings; "guardrails against a bad snapshot."

### L1 completion gate

12–15 item assessment sampling every concept, weighted toward synthesis concepts L1.6, L1.10, L1.11. Per-concept scoring feeds remediation; advancing requires all 12 mastered.

---

## 5. Level 0 — Prerequisites (.NET substrate for Synnduit code)

**Goal:** Confirm the C#/.NET skills to *write* feeds, sinks, POCOs. Gates L2+ (coding), not L1.
**Gate policy:** diagnostic-that-auto-skips. Pass a concept's cursory item → adequate, skipped. Miss → remediation + re-check. Results persist against the Entra identity (one-time placement).
**Sourcing rule:** link external for the generic skill; author only the *Synnduit-application* slice.

### Dependency graph

```
L0.O1 Interfaces ──┬─▶ L0.G2 Implement a generic interface
                   ├─▶ L0.D2 MEF composition in Synnduit
L0.G1 Use generics ┘        ▲
                            │
L0.D1 DI / constructor injection ─┘

L0.O2 Attributes ──┬─▶ L0.D2 MEF composition (uses attributes)
                   └─▶ L0.E2 POCOs & nullable rule
L0.E1 Query with EF ──▶ L0.E2 POCOs & nullable rule
```

Independent entry: **L0.O1, L0.G1, L0.D1, L0.O2, L0.E1**. Synnduit-critical synthesis: **L0.G2, L0.D2, L0.E2**.

### Concepts

Each concept has a **generic skill** (curate external) and a **Synnduit application** (author).

**L0.G1 — Using generic types** · prereqs: none
Generic: read/use `List<T>`, `IEnumerable<T>` — what the type parameter means.
Synnduit: recognize `IEntityCollection<TEntity>` as a feed's return type.
Sources: external (MS Learn — C# generics) + `StaffFeed.cs`.
Cursory item: (MC) "`IEnumerable<Staff>` is a collection of: (a) anything (b) Staff items ✓."
Remediation: MS Learn generics → show it in `StaffFeed.LoadEntities()`.

**L0.G2 — Implementing a generic interface** *(Synnduit-critical)* · prereqs: L0.G1, L0.O1
Generic: implement `IExample<T>` — supply `T`, implement members.
Synnduit: `class StaffFeed : IFeed<Staff>`, `class TeamSiteWebPartSink : ISink<TeamSiteWebPart>` — `<T>` is your entity.
Sources: external + `Sync-DDH-SPO` sinks, `StaffFeed.cs`.
Cursory item: (Short) "In `ISink<TeamSiteWebPart>`, what does `TeamSiteWebPart` bind to, and which members must you provide?"
Remediation: walk a real sink — interface left, entity as `<T>`, the five methods as the contract.

**L0.D1 — DI & constructor injection** · prereqs: none
Generic: inversion of control; a class declares dependencies in its constructor and receives them.
Synnduit: a sink takes its service via constructor, not `new`.
Sources: external (MS Learn — dependency injection) + a sink ctor.
Cursory item: (MC) "A ctor param `ITeamSiteWebPartService service` means the class: (a) creates it (b) is handed one ✓."
Remediation: MS Learn DI basics → the sink ctor receiving the service.

**L0.D2 — MEF composition in Synnduit** *(Synnduit-critical, author-heavy)* · prereqs: L0.D1, L0.O1, L0.O2
Generic: attribute-driven composition — parts are exported and discovered, not container-registered.
Synnduit: `[Sink]`/`[Feed]` mark discoverable parts; `[Export(typeof(IService))]` + `[PartCreationPolicy(CreationPolicy.Shared)]` publish a service; `[ImportingConstructor]` marks the injected ctor.
Sources: **authored only** (no good external Synnduit-MEF source) + `TeamSiteWebPartService.cs`, the sinks.
Cursory item: (Short) "What does `[ImportingConstructor]` tell MEF, and how does `ISynchronizer` reach the service?"
Remediation: **flag even for strong DI devs** (they know `Microsoft.Extensions.DI`, not MEF). Author "the DI you know vs the MEF Synnduit uses," then annotate a real `[Export]`/`[ImportingConstructor]` pair.

**L0.O1 — Interfaces & implementation** · prereqs: none
Generic: an interface is a contract; implementing it means supplying every member.
Synnduit: `ISink<T>` / `IFeed<T>` are the contracts your connectors fulfill.
Sources: external (MS Learn — interfaces) + `ISink<T>` member list.
Cursory item: (MC) "Implementing `ISink<T>` obligates you to provide: (a) any subset (b) all members ✓."
Remediation: MS Learn interfaces → `ISink<T>` members mapped to a real sink.

**L0.O2 — Attributes (declarative metadata)** · prereqs: none
Generic: `[Attribute]` annotations attach metadata read by frameworks at runtime.
Synnduit: the POCO contract is attributes — `[EntityType]`, `[EntityProperty]`, `[SourceSystemIdentifier]`, `[DuplicationKey]`.
Sources: external (MS Learn — attributes) + `Staff.cs`, `TeamSiteWebPart.cs`.
Cursory item: (MC) "`[EntityProperty]`: (a) runs code immediately (b) tags the field for the engine ✓."
Remediation: MS Learn attributes → annotate a real POCO (ties into L1.9).

**L0.E1 — Querying a source with EF** · prereqs: none
Generic: `DbContext`, LINQ-to-Entities (`.Where().Select()`), deferred execution.
Synnduit: a feed opens a `DbContext`, queries, projects rows into POCOs.
Sources: external (MS Learn — EF Core / EF6 querying) + `StaffFeed.cs`, `Sync-DDH-SPO` SchoolObjectModel feeds.
Cursory item: (Short) "When does `context.Staff.Where(...)` actually hit the database?" (On enumeration / `.ToList()`.)
Remediation: MS Learn EF querying → the `StaffFeed` HRAD projection; call out materialize-before-complex-LINQ.

**L0.E2 — POCOs & the nullable requirement** *(Synnduit-critical gotcha)* · prereqs: L0.E1, L0.O2
Generic: a POCO is a plain class mapped to data; EF binds columns to properties.
Synnduit: **Synnduit requires POCO properties to be nullable** (per README reading list); non-nullable properties cause subtle change-detection/load problems.
Sources: `Documentation/Synnduit Requirement Poco Properties are Nullable.docx` (extract) + a POCO example.
Cursory item: (Scenario) "You add `public int Grade { get; set; }` and runs misbehave — what rule did you break?" (Properties must be nullable.)
Remediation: extract the nullable-requirement doc; show wrong (`int`) vs right (`int?`) and why the engine needs the distinction.

### L0 completion gate

8–10 item diagnostic, one+ per concept. Auto-skip on pass; route misses to remediation, then re-check. Clearing all 8 unlocks L2.

---

## 6. Authoring notes

- **Highest-value authoring targets:** `L0.D2` (MEF) and `L0.E2` (nullable POCO rule) — no good external substitute, and exactly where otherwise-strong devs stumble. Lean on curation for the rest of L0.
- **Binary extraction prerequisite:** much L1/advanced depth is locked in `.docx`/`.pptx`/`.xlsx`/`.rtf` (console outcomes, dedup sequence, feature checklist, exception resolutions, deployment checklist). Extract to text before ingesting. `README.md` and `.sql` are already text.
- **L1.8/L1.9 boundary:** these lightly touch code (interfaces, attributes) at "recognize," not "write." If L1 should be fully code-free, push them into L2 — the graph still holds.
- **Knowledge base spans the full stack:** concept (README) → engine (Synnduit source) → implementations (Sync-DDH-SPO, Sync-SRC-DDH) → persistence (Synnduit-Database / StarBridge schema) → monitoring (SynnduitDashboard) → ops/troubleshooting (DiagnosticScripts, ExceptionHistory).
- **Remaining material to provide (not author):** a sample source + disposable StarBridge for hands-on labs — the biggest gap for meaningful L2+ assessment.

---

## 7. How the gating engine + Claude consume `concept-graph.json`

- **Gating engine:** reads `levels[].concepts[]` for `prereqs`, `masteryThreshold`, `itemPoolId`, and (L0) `skipOnPass`; enforces prerequisite ordering and level gates; tracks per-learner per-concept mastery keyed to the Entra identity.
- **Claude (remediation):** on a failed concept, receives `{objective (and Synnduit objective for L0), remediationAngle, sources, learner's wrong answer}` and re-teaches only that concept from that angle, grounded in the cited sources, then emits fresh practice items.
