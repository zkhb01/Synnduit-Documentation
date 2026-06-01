# System.InvalidOperationException - No sink exists for the xxx entity type

> Extracted from `System.InvalidOperationException - No sink exists for the xxx entity type.docx`

C:\IT_Portal\Dev\Source\SomViewIntegration\bin>ExportFromSomView.cmd

System.InvalidOperationException: The execution of a deployment step resulted in an exception; see inner exception for details. ---> System.InvalidOperationException: No sink exists for the 'Cssd.IT.SomViewIntegration.SchoolObjectModel.AzureGroup' entity type.

at Synnduit.Deployment.AssetAssembler.GetConnector(Type entityType, Type externalSystem, ConnectorSuite connectors, String notFoundExceptionMessageFormat) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 723

at Synnduit.Deployment.AssetAssembler.GetEntityTypeSink(Type entityType, Type externalSystem, ConnectorSuite sinks) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 689

at Synnduit.Deployment.AssetAssembler.<>c__DisplayClass31_0.<AssembleEntityTypes>b__0(Type entityType) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 671

at System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()

at System.Linq.Buffer`1..ctor(IEnumerable`1 source)

at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)

at Synnduit.Deployment.AssetAssembler.AssembleEntityTypes(Type externalSystem, EntityTypeSuite entityTypes, ConnectorSuite sinks, ConnectorSuite cacheFeeds, IDictionary`2 externalSystemAssetsByType) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 667

at Synnduit.Deployment.AssetAssembler.AssembleExternalSystems(IDictionary`2 externalSystems, IDictionary`2 sourceSystemParameters, EntityTypeSuite entityTypes, ConnectorSuite sinks, ConnectorSuite cacheFeeds) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 644

at Synnduit.Deployment.AssetAssembler.AssembleAssets(IDictionary`2 externalSystems, IDictionary`2 sourceSystemParameters, EntityTypeSuite entityTypes, ConnectorSuite sinks, ConnectorSuite cacheFeeds, IEnumerable`1 feeds) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 622

at Synnduit.Deployment.AssetAssembler.AssembleAssets() in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetAssembler.cs:line 45

at Synnduit.Deployment.AssetsDeploymentStep.Execute(IDeploymentContext context) in D:\TEMP\Synnduit\src\Synnduit\Deployment\AssetsDeploymentStep.cs:line 33

at Synnduit.Deployment.DeploymentExecutor.ExecuteDeploymentSteps(IBootstrapper bootstrapper, DeploymentContext context) in D:\TEMP\Synnduit\src\Synnduit\Deployment\DeploymentExecutor.cs:line 51

- End of inner exception stack trace ---
at Synnduit.Deployment.DeploymentExecutor.ExecuteDeploymentSteps(IBootstrapper bootstrapper, DeploymentContext context) in D:\TEMP\Synnduit\src\Synnduit\Deployment\DeploymentExecutor.cs:line 55

at Synnduit.Deployment.DeploymentExecutor.Deploy() in D:\TEMP\Synnduit\src\Synnduit\Deployment\DeploymentExecutor.cs:line 37

at Synnduit.Program.Main(String[] args) in D:\TEMP\Synnduit\src\Synnduit.Console\Program.cs:line 12

C:\IT_Portal\Dev\Source\SomViewIntegration\bin>

Issue: missing Synnduit Sink annotation of sink cs file.

Resolution:

Provide it in the code:

namespace Cssd.IT.SomViewIntegration.SchoolObjectModel.Sinks

{

[Sink]

public class AzureGroupSink : ISink<AzureGroup>

{
