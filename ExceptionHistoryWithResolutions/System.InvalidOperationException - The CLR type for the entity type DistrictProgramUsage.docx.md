# System.InvalidOperationException - The CLR type for the entity type DistrictProgramUsage

> Extracted from `System.InvalidOperationException - The CLR type for the entity type DistrictProgramUsage.docx`

System.InvalidOperationException  The CLR type for the entity type DistrictProgramUsage

C:\IT_Portal\Prod\Source\PortalIntegration-StarBridge\bin>export

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: The CLR type for the entity type 'DistrictProgramUsage' ('Cssd.IT.PortalIntegration.SchoolObjectModel.DistrictProgramUsage, Cssd.IT.PortalIntegration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null') could not be loaded.

at Synnduit.Runner.EntityType.ExtractEntityType(IEntityType entityType) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 670

at Synnduit.Runner.EntityType.GetEntityType(IEntityType entityType) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 650

at Synnduit.Runner.EntityType..ctor(IEntityType entityType, IExternalSystem destinationSystem, IEnumerable`1 sharedIdentifierSourceSystems) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 607

at Synnduit.Runner.ContextFactory.<>c__DisplayClass9_1.<CreateEntityTypes>b__6(IEntityType entityType) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 387

at System.Linq.Enumerable.WhereSelectArrayIterator`2.MoveNext()

at System.Linq.Buffer`1..ctor(IEnumerable`1 source)

at System.Linq.Enumerable.ToArray[TSource](IEnumerable`1 source)

at Synnduit.Runner.ContextFactory.CreateEntityTypes(IExternalSystem sourceSystem, IEnumerable`1 externalSystems) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 384

at Synnduit.Runner.ContextFactory.CreateContext(IBootstrapper bootstrapper, SegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IDictionary`2 runData) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 312

at Synnduit.Runner.SetupContext(SegmentConfiguration segmentConfiguration, Int32 segmentIndex, Int32 segmentCount, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 138

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 100

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, SegmentConfiguration segmentConfiguration) in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 82

at Synnduit.Runner.Run() in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 61

- End of inner exception stack trace ---
at Synnduit.Runner.Run() in D:\TEMP\Synnduit\src\Synnduit.Core\Runner.cs:line 66

at Synnduit.Program.Main(String[] args) in D:\TEMP\Synnduit\src\Synnduit.Console\Program.cs:line 13

C:\IT_Portal\Prod\Source\PortalIntegration-StarBridge\bin>

Cleard references in the c# code and restored fresh prod databases and applied the appropriate publish to the SOM db.

Worked fine after that.

As for debugging the issue with Holy Child (0130) we used SQLProfiler to ensure  we got the same results as expected from the view SchoolContact

Copied the query out and got the same results.

We then checked to see what Starbridge showed for the destination side. (if it already had them)

select *

from [dbo].[Mapping] m

inner join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]

where ssei.[SourceSystemEntityId] LIKE '9C8FF681-EFAD-4714-9860-188B097EFC53_%'

The results did show all the expected entries:

2360ADB8-36BA-490E-A0A5-166D01C52247	F8E2CC42-A499-440C-BBFF-3246DC6EC116	9200410	C292966F-A7E0-4CBF-B217-F601B3CE7EBD	1	1	F8E2CC42-A499-440C-BBFF-3246DC6EC116	C597A671-B4A4-4D72-972A-EAD98F7B87F1	DA958D35-0EE8-4B25-91EE-B0990B021098	9c8ff681-efad-4714-9860-188b097efc53_859d5511-6703-45f8-b6fc-4304dc7a0d84	C292966F-A7E0-4CBF-B217-F601B3CE7EBD

40255817-7306-49EF-BEEA-DFDA204F93AD	4F708B44-31EC-4890-AEF5-ACCDD2527BF0	9200411	00597B9B-C9ED-4C68-92CF-B3C3EFD4CD7F	1	1	4F708B44-31EC-4890-AEF5-ACCDD2527BF0	C597A671-B4A4-4D72-972A-EAD98F7B87F1	DA958D35-0EE8-4B25-91EE-B0990B021098	9c8ff681-efad-4714-9860-188b097efc53_63836848-fbd8-4e64-b234-f665679cc76b	00597B9B-C9ED-4C68-92CF-B3C3EFD4CD7F

D2409908-1100-4A4C-B3B5-65F84064B476	2106AFED-8A9F-4117-91C8-C7ECCC75BD87	9200416	E063716C-A78C-49DB-890B-4D6D186B673D	1	1	2106AFED-8A9F-4117-91C8-C7ECCC75BD87	C597A671-B4A4-4D72-972A-EAD98F7B87F1	DA958D35-0EE8-4B25-91EE-B0990B021098	9c8ff681-efad-4714-9860-188b097efc53_ab642a2a-2e22-44e8-a62a-5f51e361288e	E063716C-A78C-49DB-890B-4D6D186B673D

So the assumption that is that someone may have deleted them from ODL and starbridge was applying “last change wins” rule.

we decided to update the Starbridge mapped to show as not active. This would then take the 3 feed view records and assume they were new.

update [dbo].[Mapping]

set [State] = 3

where [Id] IN (

'2360ADB8-36BA-490E-A0A5-166D01C52247',

'40255817-7306-49EF-BEEA-DFDA204F93AD',

'D2409908-1100-4A4C-B3B5-65F84064B476')

Root cause of this issue was the fact that the SchoolContact view used the Active.ActualSchool which included schools that were under construction.

Now have an Active.EducationSchol view that the SchoolContact uses.

There was a sequence of events relating to the schools facility status change. It went from CONST to EDUC and back to CONST and then back to EDUC.

The CONST to EDUC created the Group and the 2 Contacts (no principal yet)

The EDUC to CONST change caused it to be deleted from the ODL. With cascading delete it also took the GroupContacts

The final status change to EDUC created a new school but Starbridge still had the old Destination GroupContacts so it assumed they were still in ODL

If we have the updated view they would have been dropped from starbridge and recreated when the status changes.

I added a new table “CssdSocialMedia” and connected a json file migrator to it.

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>import

System.InvalidOperationException: The execution of a deployment step resulted in an exception; see inner exception for details. ---> System.InvalidOperationException: The type 'Cssd.IT.PortalIntegration.SchoolObjectModel.CssdSocialMedia' does not represent an external system.

Root cause:

Missing         public virtual IDbSet<CssdSocialMedia> CssdSocialMedia { get; set; }

namespace Cssd.IT.PortalIntegration.SchoolObjectModel

{

[Table("dbo.CssdSocialMedia")]

[EntityType(

"A38A50AB-8956-44DE-8398-590817B88D6C",

typeof(SchoolObjectModel))]  //  had name of class ‘CssdSocialMedia’

public class CssdSocialMedia : Entity
