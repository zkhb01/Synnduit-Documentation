-- run to see Exception or Rejection messages
--Rejection Type = 1
--Warning Type = 2
--Exception Type = 3
--Information Type = 4

SELECT o.[Id] AS [OperationId],
		m.[Id] AS [MessageId],
		o.[TimeStamp],
		m.[Type],
		m.[Text],
		isnull(et.Name, etm.Name) EntityType,
		isnull(ssei.SourceSystemEntityId, sseim.SourceSystemEntityId) SourceSystemEntityId,
		etme.Outcome, eti.Outcome
	FROM [dbo].[Operation] o
		INNER JOIN [dbo].[OperationMessage] om ON o.[Id] = om.[OperationId]
		INNER JOIN [dbo].[Message] m ON om.[MessageId] = m.[Id]
		left JOIN [dbo].[MappingEntityTransaction] met ON o.[Id] = met.[Id] left join EntityTransaction etme on etme.Id = met.Id
		left join [dbo].[IdentityEntityTransaction] osse ON osse.Id = o.Id left join EntityTransaction eti on eti.Id = met.Id
		left join SourceSystemEntityIdentity ssei on ssei.ID = osse.SourceSystemEntityIdentityId
		left join EntityType et on et.Id = ssei.EntityTypeId
		left join dbo.Mapping m2 on m2.Id = met.MappingId
		left join [dbo].SourceSystemEntityIdentity sseim on sseim.Id = m2.SourceSystemEntityIdentityId
		left join EntityType etm on etm.Id = sseim.EntityTypeId
	--where o.[Timestamp] between '2023-11-21T15:59:29.1033089-07:00' and '2023-11-23T13:10:40.2173908-07:00'
	where o.[Timestamp] > '2024-09-18T12:31:53.3186828-06:00' --and o.TimeStamp < '2024-02-08T13:55:04.7483958-07:00'
	--and  ssei.SourceSystemEntityId like '%efbd917e-e8da-44d1-a057-3e1c3b44b6d5%'
	--and m.[Text]  like 'System.InvalidOperationException: The instance of entity type ''TeamSiteStaffServiceArea''%'
	--and  o.[Timestamp] > '2023-05-21T20:30:16.7455851-06:00'
	--and et.[Name] = 'StaffJob'  
	--and etm.[Name] =  'StaffJob'  
	--and m.[Text] like '%The attempt to send a user %'
	order by  o.[Timestamp] desc

Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.   
---> Microsoft.Data.SqlClient.SqlException (0x80131904): Violation of UNIQUE KEY constraint 'BK_EnglishLearner'. Cannot insert duplicate key in object 'stdnt.EnglishLearner'. The duplicate key value is (ca6c3c64-ad66-48e4-af88-9f76e78c1133).  The statement has been terminated.     at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)     at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)     at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)     at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)     at Microsoft.Data.SqlClient.SqlDataReader.TryConsumeMetaData()     at Microsoft.Data.SqlClient.SqlDataReader.get_MetaData()     at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)     at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean isAsync, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)     at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry, String method)     at Microsoft.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior)     at Microsoft.Data.SqlClient.SqlCommand.ExecuteDbDataReader(CommandBehavior behavior)     at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReader(RelationalCommandParameterObject parameterObject)     at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.Execute(IRelationalConnection connection)  ClientConnectionId:dfea13a7-2249-4eb7-96ed-67fce4cb2301  Error Number:2627,State:1,Class:14  ClientConnectionId before routing:b7e28b2e-f8a6-4d65-ae7d-fac08a2b3e22  Routing Destination:sqldb-ccsdapi.database.windows.net\b1e7a3df2a18,4815     --- End of inner exception stack trace ---     at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.Execute(IRelationalConnection connection)     at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.Execute(IRelationalConnection connection)     at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.Execute(IEnumerable`1 commandBatches, IRelationalConnection connection)     at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChanges(IList`1 entriesToSave)     at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChanges(StateManager stateManager, Boolean acceptAllChangesOnSuccess)     at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](TState state, Func`3 operation, Func`3 verifySucceeded)     at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChanges(Boolean acceptAllChangesOnSuccess)     at Microsoft.EntityFrameworkCore.DbContext.SaveChanges(Boolean acceptAllChangesOnSuccess)     at Cssd.IT.PortalIntegration.SchoolObjectModel.SinkAndCacheFeed`1.Create(TEntity entity) in C:\Repos\Sync-HRAD-DDH\src\Cssd.IT.PortalIntegration\SchoolObjectModel\SinkAndCacheFeed.cs:line 74     at Synnduit.SinkGateway`1.Execute[T](Func`1 method)


	