# Synnduit - Unable to resolve reference identifier

> Extracted from `Synnduit - Unable to resolve reference identifier.docx`

Unable to resolve reference identifier. (Property: StaffId; Source System ID: '0017738')

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>PowerSchool-Test.cmd

Segment 1 out of 2:

Start time: 2022-05-30T17:08:22.1460230-06:00

Type: Migration

Source system: Power School

Destination system: School Object Model

Entity type: Section

Loading entity identifier mappings ... 1,097,311 mappings loaded.

Caching destination system entities ... 0 entities cached.

Indexing the entity cache ... 1 index created.

Loading source system entities ... 53,243 entities loaded.

Skipped:

Rejected: 8 ( 8 )

No changes detected:

No changes merged:

Changes detected & merged:

Not found in destination system:

Duplicate detected, changes merged:

Duplicate detected, no changes merged:

Referred for manual deduplication:

New entity created:

Exception thrown:

Migration progress: .02%

^CTerminate batch job (Y/N)? y

Root Cause:

Synnduit loads all mappings, from console:  Loading entity identifier mappings … via stored Proc:

exec sp_executesql N'SELECT

[Extent1].[Id] AS [Id],

[Extent1].[EntityTypeId] AS [EntityTypeId],

[Extent1].[SourceSystemId] AS [SourceSystemId],

[Extent1].[SourceSystemEntityId] AS [SourceSystemEntityId],

[Extent1].[DestinationSystemId] AS [DestinationSystemId],

[Extent1].[DestinationSystemEntityId] AS [DestinationSystemEntityId],

[Extent1].[Origin] AS [Origin],

[Extent1].[State] AS [State],

[Extent1].[SerializedEntityHash] AS [SerializedEntityHash]

FROM [dbo].[EntityMapping] AS [Extent1]

WHERE ([Extent1].[DestinationSystemId] = @p__linq__0) AND ([Extent1].[State] IN (1, 2))

AND [Extent1].[EntityTypeId] = ''2853506f-8a34-4296-a71a-e175a0ec6ea6''

- AND [Extent1].[SourceSystemId] = ''87a05a3a-89f1-4058-9b17-0ee388304e99'' added these lines to simulate call made by program to find the entry. If not found you get the rejection msg.
AND [Extent1].[SourceSystemEntityId] = ''0017595''

',N'@p__linq__0 uniqueidentifier',@p__linq__0='DA958D35-0EE8-4B25-91EE-B0990B021098'

Call parameters:

entityTypeId	{2853506f-8a34-4296-a71a-e175a0ec6ea6}

sourceSystemId {87a05a3a-89f1-4058-9b17-0ee388304e99} (Powerschool’s id) not found but is there for 2CA6A1DE-3317-472A-AEA0-D29CB3B4ECDD (HR’s id)

sourceSystemEntityId	{0017595}	Staff employeeId not found

Fix:

The Staff Class needed the PowerSchool Source system added to it’s list.
