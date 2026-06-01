# Synnduit Nov 2017

> Extracted from `Synnduit Nov 2017.pptx` (15 slides)


## Slide 1

**Synnduit @CCSD**

- Calgary Catholic School District’s Use of the Synnduit Platform

## Slide 2

**What Is Synnduit?**

- “Synnduit is an open-source enterprise integration platform designed to enable long-term application coexistence. Developed in .NET, it’s built to work with any enterprise system, and it gives business analysts and developers enormous flexibility and extremely granular control.”

## Slide 3

**A Brief History of Synnduit**

- A number of similar projects led to “patterns”
- Started building a framework with, initially, no real-world opportunity to apply it
- It’s turned out to be a great fit for the Calgary Catholic School District’s portal integration project
- A great opportunity to finish and release to open source
- No cost to CCSD – a win-win situation!

## Slide 4

**Synnduit Is Flexible**

- Does as much as possible “out-of-the-box”...
- … but treats developers as first-class citizens!
- Features a .NET framework
- All default behavior can be overridden

## Slide 5

**Synnduit Embraces Granularity**

- Integration is hard…
- … and the devil is in the details!
- Provides extraordinary visibility of integration activity…
- …down to the level of an individual value change
- Blends integration with version control…
- …when the unexpected happens, Synnduit can help!
- “ETL Meets Version Control”

## Slide 6

**Synnduit Is Easy**

- Synnduit is an engine!
- You implement connections to your external systems
  - Source Systems
  - Destination Systems
- You implement custom logic as necessary (e.g., duplicate detection)
- You provide “data snapshots”, Synnduit detects and processes the “delta”

## Slide 7

**Free and Open-Source**

- Permissive License (Apache v2.0)
- Risk free…
- …treat (virtually) as your own source code if so inclined!
- No lock-in
- Contributions welcome – everyone can benefit

## Slide 8

**Why Synnduit?**

- Continuous, long-term data migrations (integration vs. import)
- Unified, pattern-based approach to integration
- Duplicate detection, merging, change tracking, logging
- Can interface in many ways, including direct database access and APIs
- High performance, high reliability

## Slide 9

**Synnduit @CSSD: Case Studies**

- Portal Integration, SharePoint Online Integration, Reporting Integration, and more…

## Slide 10

**CCSD Integration Architecture**


## Slide 11

**Portal Integration**

- Data exists in various existing systems, including:
  - PeopleSoft (HR)
  - PowerSchool
  - SSIS (custom; has been replaced with a direct PowerSchool link)
  - Various documents and more…
- SchoolBundle is not intended to replace existing apps
- One-off migrations aren’t sufficient: Live data keeps changing
- Data changes in multiple systems: Merging is required

## Slide 12

**SharePoint (Intranet) Integration**

- Several “Team Site Templates” defined:
  - School Site
  - Department Site (HR, Document Services, Communications, etc.)
  - Functional Site (Principals, DLT, etc.)
- Actual, out-of-the-box “SharePoint templates” not usable
  - Templates keep evolving, and existing sites need to reflect these changes
- Security-related information exists in HR
  - Automatic updates of SharePoint security records saves a lot of manual labor
- Much information in SOM editable via Supplemental Data Manager

## Slide 13

**Reporting Integration**

- Security information in School Object Model too complex for use in interactive reporting
- Preprocessed and exported into a standalone “reporting” database (ETL style)
- Benefits less obvious than in the previous two cases, as “classic ETL” (i.e., replacing all data every single time) would be feasible
- Important benefits do still exist, though…
  - Improved Performance
  - “Uninterrupted” Operation
  - Version Control (History)

## Slide 14

**CCSD Integration Architecture**


## Slide 15

**Benefits from Synnduit**

- Always up-to-date data in SchoolBundle (portal, as well as intranet)
- Substantial reduction in custom code volume (~75%)
- Fewer bugs, less error prone
- History tracking & version control features
- Facilitates support & troubleshooting
- Very little maintenance required
