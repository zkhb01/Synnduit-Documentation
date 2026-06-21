# System.InvalidOperationException  - The feed represented by type xxx could not be found

> Extracted from `System.InvalidOperationException  - The feed represented by type xxx could not be found.docx`

System.InvalidOperationException  - The feed represented by type xxx could not be found

Segment 1 out of 4:

Start Time: 2023-07-12T09:16:42.3380398-06:00

Type: Migration

Source system: Power School

Destination system: School Object Model

Entity type: SchoolDay

Loading entity identifier mappings ... 335,397 mappings loaded.

Caching destination system entities ... 188,907 entities cached.

Indexing the entity cache ... 1 index created.

Loading source system entities ... Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details.

- > System.InvalidOperationException: The feed represented by type 'Cssd.IT.PortalIntegration.PowerSchool.Feeds.SchoolDayFeed' could not be found; it may not exist or the type name may be ambiguous; see inner exception for details.
- > System.InvalidOperationException: Sequence contains no matching element <== indicated not found as opposed to ambiguous
at System.Linq.ThrowHelper.ThrowNoMatchException()

at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source, Func`2 predicate)

at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat)

- End of inner exception stack trace ---
at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat)

at Synnduit.ServiceProvider`1.GetFeed()

at System.Lazy`1.ViaFactory(LazyThreadSafetyMode mode)

at System.Lazy`1.ExecutionAndPublication(LazyHelper executionAndPublication, Boolean useDefaultConstructor)

at System.Lazy`1.CreateValue()

at Synnduit.ServiceProvider`1.get_Feed()

at Synnduit.MigrationSegmentRunner`1.LoadEntities()

at Synnduit.MigrationSegmentRunner`1.Run()

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration)

at Synnduit.Runner.Run()

- End of inner exception stack trace ---
at Synnduit.Runner.Run()

at Program.<Main>$(String[] args) in C:\Repos\Synnduit\src\Synnduit\Program.cs:line 20

Root cause:

Code used to use an interface but was changed to use a new concrete class during attempts to set up generic contexts for Oracle.

namespace Cssd.IT.PortalIntegration.PowerSchool.Feeds

{

[Feed(SourceSystem = typeof(PowerSchool))]

public class SchoolDayFeed : SchoolDayFeedBase

{

[ImportingConstructor]

public SchoolDayFeed(PowerSchoolContextFactory powerSchoolContextFactory)

: base(powerSchoolContextFactory, false)

{ }

}

}

public abstract class SchoolDayFeedBase : IFeed<SchoolDay>

{

private readonly IPowerSchoolContextFactory powerSchoolContextFactory;

private readonly string oracleQuery;

protected SchoolDayFeedBase(PowerSchoolContextFactory powerSchoolContextFactory, bool? yearRoundTrack)

Fix:

Code was corrected to use the interface again and that fixed the issue.
