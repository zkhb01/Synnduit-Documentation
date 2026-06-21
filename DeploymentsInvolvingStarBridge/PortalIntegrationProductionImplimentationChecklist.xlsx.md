# PortalIntegrationProductionImplimentationChecklist

> Extracted from `PortalIntegrationProductionImplimentationChecklist.xlsx`


## Sheet: Local

| Done | Task |
| --- | --- |
|   | 1.       Local test using copy of prod |
| x | a.       Take dev version of Code from TFS and compile. |
| x | b.      Prevent checkin by anyone to dev during this process to prevent bringing in untested code |
| x | c.       Get and log the latest changset id for the dev release. 3486 |
| x | d.      Copy latest backups for SchoolObjectModel, StarBridge, CCSD_ODL |
| x | e.      Restore to local databases |
| x | f.        Update config files to point to local database connections copied |
| x | g.       Point other database connections to Prod. |
| x | h.      Bring up the visual studio PortalIntergration-Starbridge/SchoolObjectModelDatabase project |
| x | i.         Run the Publish feature and generate the scripts. |
| x | j.        Review the generated script file and verify expected content is there. |
| x | i.      Steps may contain table alters that require them to be empty. |
| x | ii.      Add code to move data into temp tables, then truncate table, then alter table and then reload appropriate data for the altered table. |
| x | k.       Apply generated scripts to the local SchoolObjectModel database server (restored from backup of prod). |
| x | l.         Ensure scripts ran error free |
| x | m.    Check any tables where data was moved and reloaded, to ensure all data is appropriately loaded. |
| x | n.      Get the latest value for the last item imported and logged in the Starbridge EntityValueChange table |
| x | ·         SELECT top 1 [TimeStamp] FROM [Starbridge].[dbo].[EntityValueChange] ORDER BY [TimeStamp] DESC |
| x | ·         '2016-08-30 12:34:18.9569055 -06:00' |
| x | o.      Save Script file and check-in. Will be applied again to staging and prod. |
| x | p.      Run Enchilada.cmd and save cmd output |
| x | q.      Review output and verify logged changes |
| x | r.        Re-Run Enchilada.cmd and compare cmd results with previous run (no additional changes should come up.) |
| x | s.       From Sql Management Studio open the starbridge view and confirm the changes logged |
|   | SELECT [DestinationSystem] |
|   | ,[EntityType] |
|   | ,[DestinationSystemEntityId] |
|   | ,[SourceSystem] |
|   | ,[SourceSystemEntityId] |
|   | ,[TimeStamp] |
|   | ,[TransactionType] |
|   | ,[ValueName] |
|   | ,[PreviousValue] |
|   | ,[NewValue] |
|   | FROM [Starbridge].[dbo].[EntityValueChange] |
|   | WHERE [TimeStamp] > '2016-08-30 12:34:18.9569055 -06:00' |
|   | ORDER BY [TimeStamp] DESC |
| x | t.        Done local test. |


## Sheet: Staging

| Done | Task |   |   |   |   |   |   |
| --- | --- | --- | --- | --- | --- | --- | --- |
|   |   | Note on Staging test using copy of prod (speed of execution is important as Prod connections can bring in new data from HRAD and Powerschool) |   |   |   |   |   |
| x |   | Compile an x64 Release version for the Prod  ensuring release candidate is at specified change sets and no subsequent or unwanted changeset are included. |   |   |   |   |   |
| x |   | Copy existing staging app files to backup |   |   |   |   |   |
| x |   | Copy existing Prod app files to Staging - retaining config files unless there is a new version of this file as well, if so ensure staging version points to staging databases where applicable |   |   |   |   |   |
|   |   |   | C:\IT_Portal\StarBridge\CSSD |   | C:\IT_Portal\StarBridge-Staging\CSSD |   |   |
| x |   | Copy the prod json files to staging, check if PortalIntegration.json files are being updated |   |   |   |   |   |
|   |   |   | C:\JsonFiles to C:\JsonFiles-Staging |   | C:\IT_Portal\StarBridge\Configuration\PortalIntegration.json to C:\IT_Portal\StarBridge-Staging\Configuration\PortalIntegration.json |   |   |
| x |   | Get copy of Prod db restored to Staging for SchoolObjectModel, StarBridge & CCSD_ODL |   |   |   |   |   |
|   | Restore Prod Backup of SchoolObjectModel to SchoolObject-Staging, StarBridge to StarBridge-Staging and CCSD_ODL to CCSD_ODL-Staging |   |   |   |   |   |   |
| x |   | Run the app in staging - taking note of changes, can run multiple times to fully sync the integrations. Take any notes of timing , exceptions, etc. |   |   |   |   |   |
|   | Setup cmd prompt to run PortalIntegration-starbridge from the staging folder |   |   |   |   |   |   |
| x | b.      Copy files over to 10.154.2.6 (app server) to the staging folder |   |   |   |   |   |   |
|   |   |   | i. | C:\IT_Portal\StarBridge-Staging\CSSD | check that the files were copied from  \\IT_Portal\Prod\Source\PortalIntegration-StarBridge\Cssd.IT.PortalIntegration\bin\x64\Release into \\IT_Portal\Prod\Source\PortalIntegration-StarBridge\CSSD |   |   |
| x | c.       Copy over the appropriate JSON files from SourceControl to the Prod Application server Json Staging folder |   |   |   |   |   |   |
|   |   |   | if changes were made to PortalIntegration.json, copy the new version to C:\IT_Portal\StarBridge-Staging\Configuration |   |   |   |   |
| x | d.      Verify and update database connection to use staging database where prod copies will been restored. Point to prod databases otherwise. |   |   |   |   |   |   |
| -- | f.        Take a Backup of Prod SchoolObjectModel, StarBridge, CCSD_ODL Databases |   |   |   |   |   |   |
| x | h.      On the 10.154.2.5 server, update the database script file to use the staging databases. |   |   |   |   |   |   |
| x | i.         Run the database script against SchoolObjectModel-staging database |   |   |   |   |   |   |
| x | j.        Ensure scripts ran error free |   |   |   |   |   |   |
| x | k.       Check any tables where data was moved and reloaded, to ensure all data is appropriately loaded. |   |   |   |   |   |   |
| x | l.         Get the latest value for the last item imported and logged in the Starbridge EntityValueChange table |   |   |   |   |   |   |
|   | ·         SELECT top 1 [TimeStamp] FROM [StarBridge-Staging].[dbo].[EntityValueChange] ORDER BY [TimeStamp] DESC |   |   |   |   | 2019-04-01 15:16:17.4469951 -06:00 | 1st run |
|   | ·         Example result: '2016-08-30 12:34:18.9569055 -06:00' |   |   |   |   |   |   |
| x | m.    From the cmd prompt: Run Enchilada.cmd and save cmd output |   |   |   |   |   |   |
| x | n.      Re-Run Enchilada.cmd and save cmd output |   |   |   |   | 2018-11-06 16:00:00.4321260 -07:00 | 2nd run |
|   | o.      Need for speed now ends. |   |   |   |   |   |   |
|   | p.      Compare cmd results with previous run (no additional changes should come up.) |   |   |   |   |   |   |
| x | q.      Review output and verify logged changes |   |   |   |   |   |   |
|   | r.        From Sql Management Studio open the starbridge view and confirm the changes logged |   |   |   |   |   |   |
| x | SELECT [DestinationSystem] |   |   |   |   |   |   |
|   | ,[EntityType] |   |   |   |   |   |   |
|   | ,[DestinationSystemEntityId] |   |   |   |   |   |   |
|   | ,[SourceSystem] |   |   |   |   |   |   |
|   | ,[SourceSystemEntityId] |   |   |   |   |   |   |
|   | ,[TimeStamp] |   |   |   |   |   |   |
|   | ,[Outcome] |   |   |   |   |   |   |
|   | ,[ValueName] |   |   |   |   |   |   |
|   | ,[PreviousValue] |   |   |   |   |   |   |
|   | ,[NewValue] |   |   |   |   |   |   |
|   | FROM [Starbridge-Staging].[dbo].[EntityValueChange] |   |   |   |   |   |   |
|   | WHERE [TimeStamp] > '2016-08-30 12:34:18.9569055 -06:00' |   |   |   |   |   |   |
|   | ORDER BY [TimeStamp] DESC |   |   |   |   |   |   |
| x | s.       If ok with changes logged then Done staging test. |   |   |   |   |   |   |


## Sheet: Prod

| Done | Task |   |
| --- | --- | --- |
|   | 1.       Prod implementation (speed of execution is important as Prod connections can bring in new data from HRAD and Powerschool) |   |
| x | a.       Open remote sessions to App and DB servers. |   |
| x | b.      On Db server open Sql Management Studio, |   |
| x | i.      Open a New Query window and paste in a copy of the database Script file |   |
| x | ii.      ensure the database script file uses the prod SchoolObjectModel database |   |
| x | c.       On app server copy existing application files to the backup/restore folder. |   |
|   | d.      Update config file on staging folder to set database connection to use all prod databases. (ready to copy to prod) |   |
|   | e. |   |
| x | f.        Turn off the Task Scheduler for PortalIntergation. |   |
|   | g.       Take a Backup of Prod SchoolObjectModel, StarBridge, CCSD_ODL Databases |   |
| x | h.      Setup the cmd prompt to run PortalIntegration-Starbridge from the prod folder |   |
|   | i.         Run Enchilada.cmd and save cmd output |   |
|   | j.        Speed required from here… |   |
|   | k.       Get the latest value for the last item imported and logged in the Starbridge EntityValueChange table |   |
|   | ·         SELECT top 1 [TimeStamp] FROM [Starbidge].[dbo].[EntityValueChange] ORDER BY [TimeStamp] DESC |   |
|   | ·         Example result: '2019-04-01 14:06:27.8257471 -06:00' |   |
| x | l.         Copy application files over from the staging folder to the prod folder |   |
| x | l.2   Copy JsonFiles-Staging to JsonFiles folder |   |
| x | m.    On the 10.154.2.5 server: Run the database script against SchoolObjectModel database |   |
| x | n.      Ensure scripts ran error free |   |
| x | o.      Check any tables where data was moved and reloaded, to ensure all data is appropriately loaded. |   |
| x | p.      From the cmd prompt: Run Enchilada.cmd and save cmd output |   |
|   | q.      Re-Run Enchilada.cmd and save cmd output |   |
|   | r.        Need for speed now ends. |   |
| x | s.       Compare cmd results with previous run (no additional changes should come up.) |   |
| x | t.        Review output and verify logged changes |   |
| x | u.      From Sql Management Studio open the starbridge view and confirm the changes logged |   |
|   | SELECT [DestinationSystem] |   |
|   | ,[EntityType] |   |
|   | ,[DestinationSystemEntityId] |   |
|   | ,[SourceSystem] |   |
|   | ,[SourceSystemEntityId] |   |
|   | ,[TimeStamp] |   |
|   | ,[TransactionType] |   |
|   | ,[ValueName] |   |
|   | ,[PreviousValue] |   |
|   | ,[NewValue] |   |
|   | FROM [Starbridge].[dbo].[EntityValueChange] |   |
|   | WHERE [TimeStamp] > '2016-08-30 12:34:18.9569055 -06:00' |   |
|   | ORDER BY [TimeStamp] DESC |   |
|   | v.       If ok with changes logged then move onto Smoke test. |   |
|   | w.     Smoke test the prod application |   |
|   | x.       If all is NOT good |   |
|   | i.      Restore back application files and databases |   |
|   | ii.      Smoke test the old application |   |
|   | iii.      If still not good |   |
|   | 1.       Deal with it. |   |
|   | y.       Turn on the Task Scheduler for PortalIntegration. |   |
|   | z.       Merge Prod TFS from DEV cutting off at the changeset identifed |   |
|   |   | aa.   Check-in prod merge |


## Sheet: ReportingIntegration

| get latest prod release from tfs |
| --- |
| do x64 build |
| get prod db backup and restore to local for SchoolObjectModel_Reporting |
| Gen db script and run on local |
| copy prod db to staging db and run same script on staging |
| Run db script against prod db |
| Backup the old prod executabes from C:\IT_Portal\ReportingIntegration\CSSD |
| Copy compiled files to C:\IT_Portal\ReportingIntegration\CSSD |
| Disable task scheduler |
| Run admin cmd prompt against C:\IT_Portal\ReportingIntegration\bin>ExportToReporting |


## Sheet: EntityIdentifierCache

| Stop service |
| --- |
| Apply the db upgrade scripts to prod db. |
| Compiled a debug x64 version of code. |
| Copy compiled files from local build C:\IT_Portal\Prod\Source\SharePointOnlineIntegration\EntityIdentifierCacheService\bin\x64\Debug to \\spapp01\IT_Portal\SharePointOnlineIntegration\EntityIdentifierCacheService |
| Start service |


## Sheet: SPO-Mock

| Apply updated C:\IT_Portal\Dev\Source\PortalIntegration\SqlScripts\SharePointOnline Mock\ CreateMockSharePointDatabase.sql |
| --- |
| Stop EntiryIdentiferCache Service |
| Apply SOM database scripts (rename :setvar DatabaseName "SchoolObjectModel" from SchoolObjectModel-staging |
| Apply EntityIdentiferCache database scripts (rename :setvar DatabaseName "EntityIdentifierCache" from EntityIdentiferCache-Staging |
| Copy SharePointOnlineIntegration.json with updated migrators from staging to prod C:\IT_Portal\SharePointOnlineIntegration-Staging\Configuration to C:\IT_Portal\SharePointOnlineIntegration\Configuration |
| Copy CSSD latest complied files from staging to prod C:\IT_Portal\SharePointOnlineIntegration-Staging\CSSD to C:\IT_Portal\SharePointOnlineIntegration\CSSD |
| Copy EntiryIdentiferCache latest complied files from staging to prod C:\IT_Portal\SharePointOnlineIntegration-Staging\EntityIdentifierCacheService to C:\IT_Portal\SharePointOnlineIntegration\EntityIdentifierCacheService |
| Start EntiryIdentiferCache Service, and wait a bit to ensure its running ok, ie refresh list of services |
