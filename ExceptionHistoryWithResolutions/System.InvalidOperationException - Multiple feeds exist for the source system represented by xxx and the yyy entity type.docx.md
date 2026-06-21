# System.InvalidOperationException - Multiple feeds exist for the source system represented by xxx and the yyy entity type

> Extracted from `System.InvalidOperationException - Multiple feeds exist for the source system represented by xxx and the yyy entity type.docx`


### System.InvalidOperationException - Multiple feeds exist for the source system represented by xxx and the yyy entity type

C:\Repos\Sync-HRAD-SOM\CSSD>PsTest.cmd

System.InvalidOperationException: The execution of a deployment step resulted in an exception; see inner exception for details.

- > System.InvalidOperationException: Multiple feeds exist for the source system represented by 'Cssd.IT.PortalIntegration.PowerSchool.PowerSchool' and the 'Cssd.IT.PortalIntegration.SchoolObjectModel.SchoolHour' entity type.
at Synnduit.Deployment.AssetAssembler.AddFeed(HashSet`1 feeds, Feed feed)

at Synnduit.Deployment.AssetAssembler.ProcessNonGenericFeedAsset(HashSet`1 feeds, CombinedAsset`1 feedAsset, IDictionary`2 externalSystems, EntityTypeSuite entityTypes)

at Synnduit.Deployment.AssetAssembler.GetFeeds(IDictionary`2 externalSystems, EntityTypeSuite entityTypes)

at Synnduit.Deployment.AssetAssembler.AssembleAssets()

at Synnduit.Deployment.AssetsDeploymentStep.Execute(IDeploymentContext context)

at Synnduit.Deployment.DeploymentExecutor.ExecuteDeploymentSteps(IBootstrapper bootstrapper, DeploymentContext context)

- End of inner exception stack trace ---
at Synnduit.Deployment.DeploymentExecutor.ExecuteDeploymentSteps(IBootstrapper bootstrapper, DeploymentContext context)

at Synnduit.Deployment.DeploymentExecutor.Deploy()

at Program.<Main>$(String[] args) in C:\Repos\Synnduit\src\Synnduit\Program.cs:line 17

C:\Repos\Sync-HRAD-SOM\CSSD>


### Root Cause:

namespace Cssd.IT.PortalIntegration.PowerSchool.Feeds

{

[Feed(SourceSystem = typeof(PowerSchool))]

public class SchoolHourFeed : SchoolHourFeedBase

{

[ImportingConstructor]

public SchoolHourFeed(IPPSDbContextFactory ppsdbContextFactory)

: base(ppsdbContextFactory, null)

{ }

namespace Cssd.IT.PortalIntegration.PowerSchool.Feeds

{

[Feed(SourceSystem = typeof(PowerSchool))]

public class SchoolHourRegularFeed : SchoolHourFeedBase

{

[ImportingConstructor]

public SchoolHourRegularFeed(IPPSDbContextFactory ppsdbContextFactory)

: base(ppsdbContextFactory, false)

{ }

}

}

Fix: 
namespace Cssd.IT.PortalIntegration.PowerSchool.Feeds

{

[Feed(SourceSystem = typeof(PowerSchoolRegular))]

public class SchoolHourRegularFeed : SchoolHourFeedBase

{

[ImportingConstructor]

public SchoolHourRegularFeed(IPPSDbContextFactory ppsdbContextFactory)

: base(ppsdbContextFactory, false)

{ }

}

}
