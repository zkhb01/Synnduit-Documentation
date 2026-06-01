# System.InvalidOperationException - A circular manager reference has been detected

> Extracted from `System.InvalidOperationException - A circular manager reference has been detected.docx`

System.InvalidOperationException: A circular manager reference has been detected

C:\IT_Portal\Starbridge-ActiveDirectory\bin>ExportToActiveDirectory.cmd

Segment 1 out of 8:

Start time: 2020-06-18T16:33:33.8439377-06:00

Type: Migration

Source system: School Object Model

Destination system: Active Directory

Entity type: User

Loading entity identifier mappings ... 51,665 mappings loaded.

Caching destination system entities ... 15,464 entities cached.

Indexing the entity cache ... 2 indices created.

Loading source system entities ... Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: A circular manager reference has been detected.

at Cssd.IT.ActiveDirectoryIntegration.ActiveDirectory.Extensions.ToOrderedEntityCollection(IEnumerable`1 users) in C:\IT_Portal\Dev\Source\ActiveDirectoryIntegration\Cssd.IT.ActiveDirectoryIntegration\ActiveDirectory\Extensions.cs:line 35

at Cssd.IT.ActiveDirectoryIntegration.SchoolObjectModel.Feeds.UserFeed.LoadEntities() in C:\IT_Portal\Dev\Source\ActiveDirectoryIntegration\Cssd.IT.ActiveDirectoryIntegration\SchoolObjectModel\Feeds\UserFeed.cs:line 15

at Synnduit.MigrationSegmentRunner`1.LoadEntities() in D:\TEMP\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 80

at Synnduit.MigrationSegmentRunner`1.Run() in D:\TEMP\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 57

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\TEMP\Synnduit\src\Synnduit\Runner.cs:line 116

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration) in D:\TEMP\Synnduit\src\Synnduit\Runner.cs:line 82

at Synnduit.Runner.Run() in D:\TEMP\Synnduit\src\Synnduit\Runner.cs:line 61

- End of inner exception stack trace ---
at Synnduit.Runner.Run() in D:\TEMP\Synnduit\src\Synnduit\Runner.cs:line 66

at Synnduit.Program.Main(String[] args) in D:\TEMP\Synnduit\src\Synnduit.Console\Program.cs:line 13

C:\IT_Portal\Starbridge-ActiveDirectory\bin>

Cause: A UserPrincipal was to be deleted after reaching the 1 year of being disabled. There were other UserPrincipals that were still referencing him as a manager.  Removing him would have caused the UserPincipals that referenced him to not have a manager. The ifeed uses an Ordered Get which detects this issue and throws the error.

Resolution:

Update the DeletedTimestamp for this staff record in SOM Staff (Entity) to not be deleted yet. Check with HR to have all existing staff be assigned another manager.

After that, he should be good to delete from AD.
