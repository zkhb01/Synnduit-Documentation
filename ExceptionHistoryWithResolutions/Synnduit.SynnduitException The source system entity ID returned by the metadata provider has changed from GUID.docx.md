# Synnduit.SynnduitException The source system entity ID returned by the metadata provider has changed from GUID

> Extracted from `Synnduit.SynnduitException The source system entity ID returned by the metadata provider has changed from GUID.docx`

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: The source system entity ID returned by the metadata provider has changed from '00000000-0000-0000-0000-000000000000' to 'e3a063f2-4bfe-453d-8bd2-b19843d837bc'.

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

Root Cause – Had a 1-ro-1 relationship between Student and EnglishLearner entities.

The EnglishLearner entity refereced Student and applied the SourceSystemIdentidier & DuplicationKey annotations to the resolved reference field StudentId.

As this is not resolved right away the field had the empty guid., but after resolution it had the Student’s Id.

Fix: Move the 2 annotation to the NotNapped field StudentNumber.

public class EnglishLearner : Entity

{

[ForeignKey(nameof(Student))]

[EntityProperty]

[ReferenceIdentifier(typeof(Student), nameof(StudentNumber))]

[SourceSystemIdentifier]

[DuplicationKey]

public Guid StudentId { get; set; }

[NotMapped]

public int StudentNumber { get; set; }

public Student Student { get; set; }

to

public class EnglishLearner : Entity

{

[ForeignKey(nameof(Student))]

[EntityProperty]

[ReferenceIdentifier(typeof(Student), nameof(StudentNumber))]

public Guid StudentId { get; set; }

[NotMapped]

[SourceSystemIdentifier]

[DuplicationKey]

public int StudentNumber { get; set; }

public Student Student { get; set; }
