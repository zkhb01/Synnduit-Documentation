# Synnduit.SynnduitException The source system entity ID returned by the metadata provider has changed from

> Extracted from `Synnduit.SynnduitException The source system entity ID returned by the metadata provider has changed from.docx`

Synnduit.SynnduitException The source system entity ID returned by the metadata provider has changed from

Segment 74 out of 204:

Start time: 2020-11-04T03:19:33.9199387-07:00

Type: Migration

Source system: JSON

Destination system: School Object Model

Entity type: PlaceholderSiteGroup

Caching destination system entities ... 79 entities cached.

Indexing the entity cache ... 1 index created.

Loading source system entities ... 42 entities loaded.

Skipped:

Rejected:

No changes detected: 0 ( 20,164 )

No changes merged: 0 ( 1 )

Changes detected & merged: 0 ( 1,315 )

Not found in destination system:

Duplicate detected, changes merged:

Duplicate detected, no changes merged: 0 ( 1 )

Referred for manual deduplication:

New entity created: 0 ( 218 )

Exception thrown:

Migration progress: .00%

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: The source system entity ID returned by the metadata provider has changed from 'PSG_WEBSITEADMIN' to 'PSG_WEBSITEADMI'.

at Synnduit.SafeMetadataProvider`1.ReturnValidatedEntityId(TEntity entity, Func`2 getEntityIdMethod, String entityIdDictionaryDataKey, String entityIdNullExceptionMessage, String entityIdChangedExceptionMessageFormat) in D:\Projects\Synnduit\src\Synnduit\SafeMetadataProvider.cs:line 109

at Synnduit.SafeMetadataProvider`1.GetSourceSystemEntityId(TEntity entity) in D:\Projects\Synnduit\src\Synnduit\SafeMetadataProvider.cs:line 47

at Synnduit.MigrationSegmentRunner`1.RemoveMapping(IDictionary`2 mappings, TEntity entity) in D:\Projects\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 121

at Synnduit.MigrationSegmentRunner`1.Run() in D:\Projects\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 69

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 116

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 82

at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 61

- End of inner exception stack trace ---
at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 66

at Synnduit.Program.Main(String[] args) in D:\Projects\Synnduit\src\Synnduit.Console\Program.cs:line 14

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>

Root Cause: This was a Json import and the database column was too small for the json attribute value to be saved. There was C# level entityframework annotation specified for the max field size.

Resolution: Increased the column size as well as updating the c# class code annotation to maxsize.
