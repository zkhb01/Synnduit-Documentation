# System.InvalidOperationException - The sink represented by type X could not be found

> Extracted from `System.InvalidOperationException - The sink represented by type X could not be found.docx`

Any Ideas on Why the sink can’t be found?

System.InvalidOperationException: The sink represented by type 'Cssd.IT.SharePointOnlineIntegration.SharePointOnLine.Sinks.TeamSiteListSink' could not be found; it may not exist or the type name may be ambiguous; see inner exception for details. ---> System.InvalidOperationException: Sequence contains no matching element

at System.Linq.Enumerable.Single[TSource](IEnumerable`1 source, Func`2 predicate)

at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat)

in D:\Repos\Synnduit\src\Synnduit\ServiceProvider.cs:line 220

- End of inner exception stack trace ---
at Synnduit.ServiceProvider`1.GetService[TService](IEnumerable`1 services, String typeName, String exceptionMessageFormat)

in D:\Repos\Synnduit\src\Synnduit\ServiceProvider.cs:line 228

at Synnduit.ServiceProvider`1.GetSink()

in D:\Repos\Synnduit\src\Synnduit\ServiceProvider.cs:line 194

at System.Lazy`1.CreateValue()

at System.Lazy`1.LazyInitValue()

at Synnduit.ServiceProvider`1.get_Sink()

in D:\Repos\Synnduit\src\Synnduit\ServiceProvider.cs:line 101

at Synnduit.SinkGateway`1.<New>b__7_0() in D:\Repos\Synnduit\src\Synnduit\Gateway.cs:line 98

at Synnduit.SinkGateway`1.Execute[T](Func`1 method)

in D:\Repos\Synnduit\src\Synnduit\Gateway.cs:line 162

I tried debugging coming in from Synnduit.Console but I get these errors about transaction scope while calling stored procedures:

Seems the Entity type for TeamSiteListFeed is OK, not sure about TeamSiteListSink, but it’s in the dbo.EntityType table:

The process goes into the code below and dies on that Transaction exception:

private void CreateOrUpdateAssets(

IBootstrapper bootstrapper, DeploymentContext context)

{

using(ISafeRepository safeRepository = bootstrapper.Get<ISafeRepository>())

{

using(var scope = new TransactionScope())

{

this.CreateOrUpdateExternalSystems(

context.ExternalSystems, safeRepository);

this.CreateOrUpdateEntityTypes(

context.EntityTypes, safeRepository);

this.CreateSharedSourceSystemIdentifierGroups(

context.SharedSourceSystemIdentifierGroups, safeRepository);

this.CreateOrUpdateFeeds(

context.Feeds, safeRepository);

this.CreateOrUpdateParameters(context, safeRepository);

scope.Complete();

}

}

}

Not entirly sure why we update these and the data seems the same.


|   | Name | Value | Type |
| --- | --- | --- | --- |
| ▶ | _innerException | {"CREATE DATABASE statement not allowed within multi-statement transaction."} | System.Exception {System.Data.SqlClient.SqlException} |
|   | Name | Value | Type |
|   | StackTrace | "" | string |

at System.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)

at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)

at System.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)

at System.Data.SqlClient.SqlCommand.RunExecuteNonQueryTds(String methodName, Boolean async, Int32 timeout, Boolean asyncWrite)

at System.Data.SqlClient.SqlCommand.InternalExecuteNonQuery(TaskCompletionSource`1 completion, String methodName, Boolean sendToPipe, Int32 timeout, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry)

at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()

at System.Data.Entity.Infrastructure.Interception.InternalDispatcher`1.Dispatch[TTarget,TInterceptionContext,TResult](TTarget target, Func`3 operation, TInterceptionContext interceptionContext, Action`3 executing, Action`3 executed)

at System.Data.Entity.Infrastructure.Interception.DbCommandDispatcher.NonQuery(DbCommand command, DbCommandInterceptionContext interceptionContext)

at System.Data.Entity.SqlServer.SqlProviderServices.<>c__DisplayClass1a.<CreateDatabaseFromScript>b__19(DbConnection conn)

at System.Data.Entity.SqlServer.SqlProviderServices.<>c__DisplayClass33.<UsingConnection>b__32()

at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.<>c__DisplayClass1.<Execute>b__0()

at System.Data.Entity.SqlServer.DefaultSqlExecutionStrategy.Execute[TResult](Func`1 operation)

at System.Data.Entity.SqlServer.SqlProviderServices.UsingMasterConnection(DbConnection sqlConnection, Action`1 act)

at System.Data.Entity.SqlServer.SqlProviderServices.CreateDatabaseFromScript(Nullable`1 commandTimeout, DbConnection sqlConnection, String createDatabaseScript)

at System.Data.Entity.SqlServer.SqlProviderServices.DbCreateDatabase(DbConnection connection, Nullable`1 commandTimeout, StoreItemCollection storeItemCollection)

at System.Data.Entity.Migrations.Utilities.DatabaseCreator.Create(DbConnection connection)

at System.Data.Entity.Migrations.DbMigrator.EnsureDatabaseExists(Action mustSucceedToKeepDatabase)

at System.Data.Entity.Migrations.DbMigrator.Update(String targetMigration)

at System.Data.Entity.Internal.DatabaseCreator.CreateDatabase(InternalContext internalContext, Func`3 createMigrator, ObjectContext objectContext)

at System.Data.Entity.Database.Create(DatabaseExistenceState existenceState)

at System.Data.Entity.CreateDatabaseIfNotExists`1.InitializeDatabase(TContext context)

at System.Data.Entity.Internal.InternalContext.PerformInitializationAction(Action action)

at System.Data.Entity.Internal.InternalContext.PerformDatabaseInitialization()

at System.Data.Entity.Internal.RetryAction`1.PerformAction(TInput input)

at System.Data.Entity.Internal.LazyInternalContext.InitializeDatabaseAction(Action`1 action)

at System.Data.Entity.Internal.InternalContext.ExecuteSqlCommand(TransactionalBehavior transactionalBehavior, String sql, Object[] parameters)

at Synnduit.Persistence.SqlServer.DatabaseContext.ExecuteStoredProcedure(String storedProcedureName, SqlParameter[] parameters) in D:\\TEMP\\Synnduit\\src\\Synnduit.Persistence.SqlServer\\DatabaseContext.cs:line 648

at Synnduit.Persistence.SqlServer.DatabaseContext.CreateOrUpdateExternalSystem(Guid id, String name) in D:\\TEMP\\Synnduit\\src\\Synnduit.Persistence.SqlServer\\DatabaseContext.cs:line 425

at Synnduit.Persistence.SqlServer.Repository.CreateOrUpdateExternalSystem(IExternalSystem externalSystem) in D:\\TEMP\\Synnduit\\src\\Synnduit.Persistence.SqlServer\\Repository.cs:line 212

at Synnduit.Persistence.SafeRepository.<>c__DisplayClass13_0.<CreateOrUpdateExternalSystem>b__0() in C:\\IT_Portal\\Dev\\Source\\Synnduit-master\\src\\Synnduit\\Persistence\\SafeRepository.cs:line 148

at Synnduit.Persistence.SafeRepository.Invoke(Action method, String methodName) in C:\\IT_Portal\\Dev\\Source\\Synnduit-master\\src\\Synnduit\\Persistence\\SafeRepository.cs:line 520
