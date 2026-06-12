# Synnduit — A Teammate-Friendly Introduction

> A short onboarding doc for new teammates joining the CSSD integration team.
> Read this first, then dive into the deeper docs in `Documentation/`.

---

## 1. What is Synnduit?

**Synnduit is an open-source data synchronization engine built in .NET.** It keeps data in sync between a *source* system (where data originates — HR, PowerSchool, JSON files, ServiceNow, etc.) and a *destination* system (where it needs to land — at CSSD that's the **DDH / District Data Hub**), continuously and over the long term.

Its tagline is **"ETL meets version control"** — Synnduit doesn't just copy data, it tracks every change down to individual field values, and it remembers the link between source and destination records so the same record is updated next time, not duplicated.

- **License:** Apache 2.0 (open source)
- **Source / project home:** https://github.com/Synnduit
- **Support contact:** Jon Suda — jon@synnduit.com

---

## 2. The mental model — Synnduit is an *engine*

You don't change the engine. You plug things into it:

1. **You implement a Source connector** — code that reads a "data snapshot" from a system (Oracle, AD, an API, a JSON file).
2. **You implement a Destination connector** — code that knows how to create / update / delete records in the target system.
3. **Synnduit does the hard part** — it compares the new snapshot to what it saw last time, figures out what changed (the *delta*), and applies only those changes to the destination.

```
    ┌──────────┐   snapshot    ┌────────────┐   delta    ┌──────────────┐
    │  SOURCE  │ ────────────▶ │  Synnduit  │ ─────────▶ │ DESTINATION  │
    │ (HR, PS) │               │   engine   │            │    (DDH)     │
    └──────────┘               └─────┬──────┘            └──────────────┘
                                     │
                                     ▼
                            ┌─────────────────┐
                            │   StarBridge    │  ← mappings, change history,
                            │ (SQL bookkeeping)│   transactions, exceptions
                            └─────────────────┘
```

---

## 3. Vocabulary you'll hear every day

| Term | Meaning |
|---|---|
| **Migrator / Run** | A scheduled job that syncs a set of entities from sources to a destination. Runs via Windows Task Scheduler. |
| **Segment** | One step inside a Run — either a `Migration` (push deltas) or a `GarbageCollection` (clean up records the source no longer has). |
| **StarBridge** | The SQL database where Synnduit stores mappings, transactions, value changes, deltas, and exceptions. One StarBridge DB per Run. |
| **iFeed** | A class implementing `IFeed<TEntity>` that produces the source-side data snapshot. Tagged with `[Feed(SourceSystem = typeof(...))]`. |
| **iSink** | A class implementing `ISink<TEntity>` that writes to the destination — `New` / `Create` / `Get` / `Update` / `Delete`. Tagged with `[Sink(DestinationSystem = typeof(...))]`. The destination-side counterpart of an iFeed. |
| **iCacheFeed** | A class implementing `ICacheFeed<TEntity>` that lets Synnduit re-read the destination's current state for diffing. Often combined with the iSink in one class (see `SinkAndCacheFeed.cs`). |
| **POCO** | The C# class describing one entity type (e.g. `Staff`, `Student`, `School`). |
| **Mapping** | The persisted link between a source record's ID and the destination record's ID. |
| **Transaction outcome** | What happened to one record in one run — e.g. `NewEntityCreated`, `ChangesDetectedAndMerged`, `NoChangesDetected`, `Rejected`, `ExceptionThrown` (11 outcomes total — see `Documentation/Synnduit Console.docx`). |
| **DDH** | District Data Hub — CSSD's destination database, the "single source of truth". |

---

## 4. Key features worth knowing

- **Delta detection** — only changed records are processed, never full reloads.
- **Change history** — every value change is logged in StarBridge, down to the field.
- **Duplicate detection & merging** — a `[DuplicationKey]` attribute on the POCO lets Synnduit match a "new" source record to an existing destination record and merge them.
- **Multi-source entities** — one destination entity (e.g. `Staff`) can be fed by *several* source systems via `[SharedSourceSystemIdentifiers]`.
- **Threshold protection** — abort the run if deletions, orphans, or exceptions exceed a configured % (protects against a bad source feed wiping out the destination).
- **Resumable** — if the server crashes mid-run, no data loss; the next run picks up where it left off.
- **Garbage collection** — configurable behavior for what to do with destination records whose source has disappeared.
- **Dashboard** — a separate Synnduit Dashboard sits on top of any StarBridge DB to visualize sync history.

---

## 5. A real CSSD example: `Sync-SRC-DDH`

The repo at `C:\Users\<you>\source\repos\Sync-SRC-DDH` is a working example — it implements the entity mappings that flow from our source systems into the DDH.

### 5.1 What it integrates

| StarBridge Database  | Source Systems |
|---|---|
| `StarBridge-Hrad`    | HRAD (PeopleSoft HR), JSON, ServiceNow |
| `StarBridge-School`  | PowerSchool (School API) |
| `StarBridge-Stdnt`   | PowerSchool (Student API) |

Each StarBridge DB owns its own subset of entities and its own bookkeeping.

### 5.2 Repo layout

```
Sync-SRC-DDH/
├── src/Cssd.IT.PortalIntegration/   ← the .NET library Synnduit loads
│   ├── HumanResources/              ← HRAD (PeopleSoft) source connector + feeds
│   ├── PowerSchool/                 ← PowerSchool source connector + feeds
│   ├── Json/                        ← JSON-file source connector + feeds
│   ├── ServiceNow/                  ← ServiceNow source connector + feeds
│   └── SchoolObjectModel/           ← The DESTINATION model (DDH entities)
│       ├── Staff.cs, School.cs, Student.cs, Job.cs, ...
│       └── Feeds/                   ← Destination-side sink/cache feeds
├── CSSD/                            ← Deployment + operational folder
│   ├── appsettings.json             ← Run definitions, segments, thresholds, connection strings
│   ├── HradToDdh.cmd                ← Launches the HRAD → DDH run
│   ├── PsSchoolToDdh.cmd            ← Launches the PowerSchool School → DDH run
│   ├── PsStdntToDdh.cmd             ← Launches the PowerSchool Student → DDH run
│   └── UpdateAzureStaffWithLatestUserName.ps1   ← Post-run script (HRAD only)
├── JsonFiles/                       ← Static JSON resources used as a source
└── OracleSqlFiles/                  ← SQL scripts for the Oracle source DBs
```

### 5.3 How a run is launched

A run is just a CLI invocation. From `CSSD\HradToDdh.cmd`:

```cmd
@echo on
set PATH=C:\IT_Portal\Synnduit\0.9.3\
Synnduit HradToDdh f6dc7836-5362-4a77-adbb-12a241310591
if %errorlevel% neq 0 (
    echo Synnduit failed with code %errorlevel%
    exit /b %errorlevel%
)
powershell.exe -File "UpdateAzureStaffWithLatestUserName.ps1"
```

The shape is always: **`Synnduit <RunName> <UserSecretGuid>`** — the GUID is a **UserSecret identifier**: Synnduit uses it to locate the environment-specific UserSecret JSON file (connection strings and other secrets) for the run, so the same run can execute in different environments (Prod, Local, etc.) against the appropriate set of secrets.

### 5.4 How a Run is wired together (`appsettings.json`)

A `Run` is a named, ordered list of `Segments`. Order matters: parents before children when migrating in, and reverse order when garbage-collecting out (so foreign keys stay valid).

```jsonc
"Synnduit": {
  "ExceptionHandling": {
    "SegmentAbortThreshold": 30,                           // abort segment after 30 exceptions
    "RunAbortThreshold": 101,
    "OrphanMappingPercentageAbortThreshold": 0.05,         // abort if >5% of mappings orphan
    "GarbageCollectionPercentageAbortThreshold": 0.05      // abort if >5% would be deleted
  },
  "Runs": [
    {
      "Name": "PsStdntToDdh",
      "SourceSystem": "Power School",
      "Segments": [
        { "Type": "Migration",         "EntityType": "School" },
        { "Type": "Migration",         "EntityType": "Student" },
        { "Type": "Migration",         "EntityType": "StudentAddress" },
        // ...migrate parents before children...
        { "Type": "GarbageCollection", "EntityType": "StudentAddress" },
        { "Type": "GarbageCollection", "EntityType": "Student" },
        { "Type": "GarbageCollection", "EntityType": "School" }
      ]
    }
  ]
}
```

### 5.5 Anatomy of an entity (POCO)

From `SchoolObjectModel/Staff.cs`:

```csharp
[EntityType("2853506F-8A34-4296-A71A-E175A0EC6EA6", typeof(SchoolObjectModel))]
[SharedSourceSystemIdentifiers(                  // Staff can be fed by multiple sources
    typeof(SchoolObjectModel),
    typeof(PowerSchool.PowerSchool),
    typeof(HumanResources.HumanResources),
    typeof(Json.Json))]
public class Staff : Entity
{
    [SourceSystemIdentifier]                     // ID in the source
    public string Code { get; set; }

    [EntityProperty]
    [DuplicationKey]                             // EmployeeId is what we dedupe on
    public string EmployeeId { get; set; }

    [EntityProperty] public string FirstName { get; set; }
    [EntityProperty] public string LastName  { get; set; }
    // ...
}
```

The attributes are the contract:
- `[EntityType(<guid>, <destinationSystem>)]` — globally identifies the entity.
- `[SourceSystemIdentifier]` / `[DestinationSystemIdentifier]` — tell Synnduit which property is the source-side ID and which is the destination-side ID, so it can store the mapping.
- `[EntityProperty]` — fields Synnduit should track for change detection.
- `[DuplicationKey]` — used during dedup to match a "new" source record to an existing destination record.

### 5.6 Anatomy of a Feed (the source connector)

From `HumanResources/Feeds/StaffFeed.cs`:

```csharp
[Feed(SourceSystem = typeof(HumanResources))]
public class StaffFeed : IFeed<Staff>
{
    public IEntityCollection<Staff> LoadEntities()
    {
        using var context = this.hradFactory.CreateHRAD();
        return context.CCS_HR_AD_SYNC
            .Where(Util.ActiveStaffWithManager)
            .Select(s => new Staff { EmployeeId = s.EMPLID, FirstName = s.FIRST_NAME, ... })
            .ToList();
    }
}
```

That's it. You query your source however you want, return a collection of POCOs, and Synnduit handles the diff against last run, the mapping, dedup, change logging, error thresholds, and garbage collection.

---

## 6. Where to look first when something goes wrong

1. **The console output** — re-run the `.cmd` manually from a command prompt to see live progress.
2. **StarBridge tables** — `EntityTransaction` (per-record outcomes), `EntityValueChange` (field-level diffs), `EntityException` (errors).
3. **Diagnostic SQL** — see `DiagnosticScripts/` in this repo (`Starbridge-Cleanup.sql`, `Starbridge-Deletions.sql`, `Starbridge-ExceptionRejections.sql`, `Starbridge-SerializedEntityData.sql`, `Starbridge-ValueChange.sql`).
4. **Resolved-exception history** — `ExceptionHistoryWithResolutions/` documents past issues and their fixes.
5. **The Synnduit Dashboard** — point it at the StarBridge DB for a visual history of runs.

---

## 7. Suggested reading order for new teammates

1. This README — the 10-minute overview.
2. `Documentation/Synnduit Nov 2017.pptx` — the original CSSD overview deck.
3. `Documentation/Features of a sync program.docx` — the feature checklist.
4. `Documentation/Synnduit Console.docx` — the 11 transaction outcomes in plain English.
5. `Documentation/Synnduit Deduplication sequence.docx` — how an insert decides "new vs. duplicate".
6. `Documentation/Synnduit Requirement Poco Properties are Nullable.docx` — a real gotcha you'll hit when adding new entities.
7. `Documentation/Synnduit Dashboard.docx` — point the dashboard at a StarBridge DB.
8. Open the `Sync-SRC-DDH` solution and walk through one full feed: `StaffFeed.cs` → `Staff.cs` → `appsettings.json` segment list.

---

## 8. Where things live (CSSD environment)

| Thing | Path |
|---|---|
| Synnduit binary | `C:\IT_Portal\Synnduit\0.9.3\` |
| Synnduit core source (open-source) | `C:\Repos\Synnduit.Core\src\Synnduit.Core\` |
| Synnduit Dashboard | `C:\Repos\SynnduitDashboard\` |
| Example integration repo | `C:\Users\<you>\source\repos\Sync-SRC-DDH\` |
| This documentation repo | `C:\Users\<you>\source\repos\SynnduitDocumentation\` |
