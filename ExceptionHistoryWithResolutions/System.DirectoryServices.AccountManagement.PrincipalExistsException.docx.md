# System.DirectoryServices.AccountManagement.PrincipalExistsException

> Extracted from `System.DirectoryServices.AccountManagement.PrincipalExistsException.docx`

System.DirectoryServices.AccountManagement.PrincipalExistsException: The object already exists. ---> System.DirectoryServices.DirectoryServicesCOMException: The object already exists

at System.DirectoryServices.DirectoryEntry.CommitChanges()

at System.DirectoryServices.AccountManagement.SDSUtils.ApplyChangesToDirectory(Principal p, StoreCtx storeCtx, GroupMembershipUpdater updateGroupMembership, NetCred credentials, AuthenticationTypes authTypes)

- End of inner exception stack trace ---
at System.DirectoryServices.AccountManagement.SDSUtils.ApplyChangesToDirectory(Principal p, StoreCtx storeCtx, GroupMembershipUpdater updateGroupMembership, NetCred credentials, AuthenticationTypes authTypes)

at System.DirectoryServices.AccountManagement.SDSUtils.InsertPrincipal(Principal p, StoreCtx storeCtx, GroupMembershipUpdater updateGroupMembership, NetCred credentials, AuthenticationTypes authTypes, Boolean needToSetPassword)

at System.DirectoryServices.AccountManagement.ADStoreCtx.Insert(Principal p)

at System.DirectoryServices.AccountManagement.Principal.Save()

at Cssd.IT.ActiveDirectoryIntegration.ActiveDirectory.Sinks.UserSinkAndCacheFeed.Create(User user)

in C:\IT_Portal\Dev\Source\ActiveDirectoryIntegration\Cssd.IT.ActiveDirectoryIntegration\ActiveDirectory\Sinks\UserSinkAndCacheFeed.cs:line 53

at Synnduit.SinkGateway`1.<>c__DisplayClass8_0.<Create>b__0() in D:\TEMP\Synnduit\src\Synnduit\Gateway.cs:line 112

at Synnduit.SinkGateway`1.Execute[T](Func`1 method) in D:\TEMP\Synnduit\src\Synnduit\Gateway.cs:line 162

Tired to see if user existed somewhere in TestAD

Used the employeeId, Sur


|   | jessica.vargasaroni |
| --- | --- |

name, Partial lastname: ‘Vargas’, 'Aroni’, nothing found.

In production the account exists.

Looked up Exception class for error:

https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.accountmanagement.principalexistsexception?view=netframework-4.8

Stuck wondering why it finds an existing entry and I can’t find it using the tool.
