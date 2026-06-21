# System.InvalidOperationException - A circular manager reference has been detected 2

> Extracted from `System.InvalidOperationException - A circular manager reference has been detected 2.docx`

System.InvalidOperationException: A circular manager reference has been detected

Segment 1 out of 8:

Start Time: 2024-09-06T09:38:46.4250877-06:00

Type: Migration

Source system: School Object Model

Destination system: Active Directory

Entity type: User

Loading entity identifier mappings ... 64,816 mappings loaded.

Caching destination system entities ... 11,222 entities cached.

Indexing the entity cache ... 2 indices created.

Loading source system entities ... {"AdId":{"Identifier":"2e9bddd4-cff0-4c9f-a286-9f181d19f700"},"Guid":null,"AccountExpirationDate":null,"C":"CA","Co":"Canada","Company":null,"Department":null,"Description":"Disabled by SDM 2023-09-28 00:00:00 | Custodial Services - Department Consultant","DisplayName":"Bobby Cormier","Division":"NA","EmailAddress":"bobby.cormier@cssd.ab.ca","EmployeeId":"101553755","EmployeeType":"CNTRCTR","Enabled":false,"ExtensionAttribute1":null,"ExtensionAttribute2":null,"ExtensionAttribute3":null,"ExtensionAttribute4":null,"ExtensionAttribute5":"SDM","ExtensionAttribute6":null,"ExtensionAttribute7":null,"ExtensionAttribute8":null,"ExtensionAttribute9":null,"ExtensionAttribute10":null,"ExtensionAttribute11":"2023-09-28 00:00:00","ExtensionAttribute12":"2023-09-28 13:04:40","ExtensionAttribute13":null,"ExtensionAttribute14":null,"ExtensionAttribute15":null,"GivenName":"Bobby","Initials":"n","L":"Calgary","LastLogon":null,"MailNickname":"bobby.cormier","ManagerId":null,"ManagerStaffId":{"Identifier":"3eda53ba-5884-401a-ae74-89d638240496"},"MiddleName":"nomiddle","MsExchHideFromAddressLists":true,"MsExchUsageLocation":"CA","Name":"Bobby Cormier","OtherMailbox":"bobby.cormier@learn.cssd.ab.ca","OuPath":"user accounts/sectionu15/096uacenter/096usupport/096usupport-cnmusers","PhysicalDeliveryOfficeName":"Catholic School Centre","PostalCode":"T2P 4T9","SamAccountName":"bobby.cormier","St":"AB","StreetAddress":"1000 - 5 Avenue SW","Surname":"Cormier","TargetAddress":"smtp:bobby.cormier@calgarycatholicschools.mail.onmicrosoft.com","TelephoneNumber":null,"Title":null,"UserPrincipalName":"bobby.cormier@cssd.ab.ca","WhenChanged":null,"WhenCreated":null,"UserAccountControl":0}

Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details.

- > System.InvalidOperationException: A circular manager reference has been detected. ManagerId: , EmployeeId: 101553755
at Cssd.IT.ActiveDirectoryIntegration.ActiveDirectory.Extensions.ToOrderedEntityCollection(IEnumerable`1 users) in C:\Repos\Sync-DDH-AD\src\Cssd.IT.ActiveDirectoryIntegration\ActiveDirectory\Extensions.cs:line 31

at Cssd.IT.ActiveDirectoryIntegration.SchoolObjectModel.Feeds.UserFeed.LoadEntities() in C:\Repos\Sync-DDH-AD\src\Cssd.IT.ActiveDirectoryIntegration\SchoolObjectModel\Feeds\UserFeed.cs:line 23

at Synnduit.MigrationSegmentRunner`1.LoadEntities()

at Synnduit.MigrationSegmentRunner`1.Run()

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration, IBootstrapper bootstrapper, ISafeRepository safeRepository)

at Synnduit.Runner.RunSegment(Int32 segmentIndex, Int32 segmentCount, IRunConfiguration runConfiguration, ISegmentConfiguration segmentConfiguration)

at Synnduit.Runner.Run()

- End of inner exception stack trace ---
at Synnduit.Runner.Run()

at Program.<Main>$(String[] args) in D:\Repos\Synnduit\src\Synnduit\Program.cs:line 20

Cause: The staff identified above "EmployeeId":"101553755" was an expired contractor with a more expired Manager. This manager had just reached the 1-year old mark before the disabled record is physically deleted from AD.  Because he now had no manager he was not getting picked up on the Required Manger test and causing the exception.

Resolution:

The record for the contractor Bobby Cormier was days away from hitting the 1 year of being disabled, so we updated his DeactivatedDate value to the current date. This then filtered him out of the iFeed, causing him to be deleted and not checked for having a valid manager.
