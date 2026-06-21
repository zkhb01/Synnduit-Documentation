# Synnduit.SynnduitException - The specified entity type ('LookupFNMI') could not be found

> Extracted from `Synnduit.SynnduitException - The specified entity type ('LookupFNMI') could not be found.docx`

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>PowerSchool-Test.cmd

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: The specified entity type ('LookupFNMI') could not be found; it may not exist or the name may be ambiguous; see inner exception for details. ---> System.InvalidOperationException: Sequence contains no matching element

at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source, Func`2 predicate)

at Synnduit.Runner.Single[T](IEnumerable`1 collection, String name, Func`2 getName, String exceptionMessageFormat) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 203

- End of inner exception stack trace ---
at Synnduit.Runner.Single[T](IEnumerable`1 collection, String name, Func`2 getName, String exceptionMessageFormat) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 209

at Synnduit.Runner.ContextFactory.GetEntityType(String entityTypeName, IEnumerable`1 entityTypes) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 444

at Synnduit.Runner.ContextFactory.CreateContext(IBootstrapper bootstrapper, SegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IDictionary`2 runData) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 314

at Synnduit.Runner.SetupContext(SegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 138

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 100

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 82

at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 61

- End of inner exception stack trace ---
at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 66

at Synnduit.Program.Main(String[] args) in D:\Projects\Synnduit\src\Synnduit.Console\Program.cs:line 14

Root Cause:  The new migrator 'LookupFNMI' specified in the \Configuration\PortalIntegration.json file was not found in the complied program. Either the json file had it miss-spelled or the program was not successfully compiled with the code for that migrator.

Fix: In this case it was the fact that the program was not compiled after the coding for it was done.  After compiling the program that process ran through.
