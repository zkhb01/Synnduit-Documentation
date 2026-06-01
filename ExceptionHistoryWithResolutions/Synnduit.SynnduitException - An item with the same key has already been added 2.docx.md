# Synnduit.SynnduitException - An item with the same key has already been added 2

> Extracted from `Synnduit.SynnduitException - An item with the same key has already been added 2.docx`

Loading entity identifier mappings ... Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details.

- > System.InvalidOperationException: An IInitializable instance threw an exception; see inner exception for details.
- > System.ArgumentException: An item with the same key has already been added. Key: Synnduit.Mappings.SourceSystemEntityIdentity
at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)

at System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)

at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](TSource[] source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)

at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)

at Synnduit.Mappings.MappingDataRepository.Initializer.Initialize(IInitializationContext context)

at Synnduit.InvocableInitializer.Initialize(InitializableInstanceWrapper initializableInstance, InitializationContext context)

- End of inner exception stack trace ---
at Synnduit.InvocableInitializer.Initialize(InitializableInstanceWrapper initializableInstance, InitializationContext context)

at Synnduit.InvocableInitializer.Initialize(IEventDispatcher eventDispatcher)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration)

at Synnduit.Runner.Run()

- End of inner exception stack trace ---
at Synnduit.Runner.Run()

at Program.<Main>$(String[] args) in C:\Repos\Synnduit\src\Synnduit\Program.cs:line 22

C:\Repos\Sync-HRAD-DDH\CSSD>exit /b 1

Root Cause: The source system PowerSchool SectionTeacher records imported a set of 3 sections that were linked to a Staff that had a trailing blank on their employeeId.

For SectionTeacher the poco class had the sourceSystemEntityId defind as SectionId + '_' + EmployeeId

Fixing the Ifeed query to trim the trailing blanks alone did not fix the problem.  Turned out Starbridge had an issue in that it would reuse the old sourceSystemEntityId  even though the mapping state was set to '3'.

Fix: ran this script to stri[p the trailing blank of the 3 Source SystemEntityId's

begin tran

update [dbo].[SourceSystemEntityIdentity]

set [SourceSystemEntityId] = RTRIM([SourceSystemEntityId])

where [SourceSystemEntityId] like '%_0025570 '

rollback tran

- commit tran
