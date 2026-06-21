# System.InvalidOperationException - The CLR type for the entity type Asset

> Extracted from `System.InvalidOperationException - The CLR type for the entity type Asset.docx`

From: Karl Burndorfer 
Sent: Tuesday, April 26, 2022 15:55
To: Jon Suda <jon@synnduit.com>
Subject: Odd discrepancy between local vs. staging integration run

Hey Jon. I did get my local machine & DB to run the SOM-Reporting integration.

I got a prod backup loaded onto the Staging DB server, Updated the appsettings.json connection strings to point to staging DB and launched it again. Started up just fine:

C:\Repo\ArchitectureIntegration\Sync-SOM-Reporting\CSSD\bin>ExportToReporting.cmd

Segment 1 out of 12:

Start Time: 2022-04-26T14:47:15.1966392-06:00

Type: Migration

Source system: School Object Model

Destination system: Reporting

Entity type: Asset

Loading entity identifier mappings ... 149,500 mappings loaded.

Caching destination system entities ... 15,153 entities cached.

Indexing the entity cache ... 1 index created.

I then copied the local bin folder over to the staging app server.

Ran it from there just to make sure the env is good there  as well.

Got this error:

C:\IT_Portal\Starbridge-Reporting\Core\bin>ExportToReporting.cmd

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details.

- > System.InvalidOperationException: The CLR type for the entity type 'Asset' ('Cssd.IT.ReportingIntegration.Reporting.Asset, Cssd.IT.ReportingIntegration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null') could not be loaded.
at Synnduit.Runner.EntityType.ExtractEntityType(IEntityType entityType)

at Synnduit.Runner.EntityType.GetEntityType(IEntityType entityType)

at Synnduit.Runner.EntityType..ctor(IEntityType entityType, IExternalSystem destinationSystem, IEnumerable`1 sharedIdentifierSourceSystems)

at Synnduit.Runner.ContextFactory.<>c__DisplayClass10_1.<CreateEntityTypes>b__6(IEntityType entityType)

at System.Linq.Enumerable.SelectArrayIterator`2.ToArray()

at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)

at Synnduit.Runner.ContextFactory.CreateEntityTypes(IExternalSystem sourceSystem, IEnumerable`1 externalSystems)

at Synnduit.Runner.ContextFactory.CreateContext(IBootstrapper bootstrapper, ISegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IDictionary`2 runData)

at Synnduit.Runner.SetupContext(IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IBootstrapper bootstrapper, ISafeRepository safeRepository)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration)

at Synnduit.Runner.Run()

- End of inner exception stack trace ---
at Synnduit.Runner.Run()

at Program.<Main>$(String[] args) in C:\Repo\Synnduit\src\Synnduit\Program.cs:line 20

C:\IT_Portal\Starbridge-Reporting\Core\bin>

With both local and staging using the same db and code, I was perplexed on what could cause it to bomb on the first segment.

I installed dotnet-sdk-6.0.103-win-x64.exe on the staging app server, ran again, but still same error.

Was there anything else that would need to be installed on the app server?
