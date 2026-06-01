# System.InvalidOperationException - The feed represented by type XX could not be found - it may not exist or the type name may be ambiguous

> Extracted from `System.InvalidOperationException - The feed represented by type XX could not be found - it may not exist or the type name may be ambiguous.docx`

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>import

Segment 1 out of 21:

Start time: 2020-08-26T14:53:24.0414260-06:00

Type: Migration

Source system: Service Now

Destination system: School Object Model

Entity type: SupportedProgram

Loading entity identifier mappings ... 228,592 mappings loaded.

Caching destination system entities ... 13 entities cached.

Indexing the entity cache ... 1 index created.

Loading source system entities ... Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: The feed represented by type 'Cssd.IT.PortalIntegration.ServiceNow.Feeds.SupportedProgramFeed' could not be found; it may not exist or the type name may be ambiguous; see inner exception for details. ---> System.InvalidOperationException: Sequence contains no matching element

at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source, Func`2 predicate)

at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat) in D:\Projects\Synnduit\src\Synnduit\ServiceProvider.cs:line 220

- End of inner exception stack trace ---
at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat) in D:\Projects\Synnduit\src\Synnduit\ServiceProvider.cs:line 228

at Synnduit.ServiceProvider`1.GetFeed() in D:\Projects\Synnduit\src\Synnduit\ServiceProvider.cs:line 181

at System.Lazy`1.CreateValue()

at System.Lazy`1.LazyInitValue()

at Synnduit.ServiceProvider`1.get_Feed() in D:\Projects\Synnduit\src\Synnduit\ServiceProvider.cs:line 93

at Synnduit.MigrationSegmentRunner`1.LoadEntities() in D:\Projects\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 80

at Synnduit.MigrationSegmentRunner`1.Run() in D:\Projects\Synnduit\src\Synnduit\MigrationSegmentRunner.cs:line 57

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 116

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration) in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 82

at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 61

- End of inner exception stack trace ---
at Synnduit.Runner.Run() in D:\Projects\Synnduit\src\Synnduit\Runner.cs:line 66

at Synnduit.Program.Main(String[] args) in D:\Projects\Synnduit\src\Synnduit.Console\Program.cs:line 14

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>SupportedProgramSupportedProgram

Root Cause:

The MEF dependencies were not there, hence it could not create the object due to not being able to create one of its dependent objects.

In this case it was the invalid name for the Export typeOf. It was a copy paste issue that was not replaces with the correct value of ISupportedProgramDataAdapter

namespace Cssd.IT.PortalIntegration.ServiceNow.Feeds

{

[Export(typeof(IProgramSupportContractorDataAdapter))]

public class SupportedProgramDataAdapter : ISupportedProgramDataAdapter

{

private readonly IDataContractExtension<SupportedProgramDataContract> dataContractExtension;

[ImportingConstructor]

public SupportedProgramDataAdapter(

IDataContractExtension<SupportedProgramDataContract> dataContractExtension)

{

this.dataContractExtension = dataContractExtension;

}

public SupportedProgram SupportedProgramMapping(

SupportedProgramDataContract supportedProgramDataContract)

{

return new SupportedProgram()

{

Code = supportedProgramDataContract.Code.SafeTrim(),

Name = supportedProgramDataContract.Name.SafeTrim(),

};

}

}

}

Resolution:

Once replaces with the correct name the process worked as expected.

Then the SupportedProgramFeed class constructor would be able to fully resolve all its MEF dependencies.
