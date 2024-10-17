
BEGIN TRANSACTION

CREATE TABLE #EntityTypes
(
	[Id] UNIQUEIDENTIFIER NOT NULL
)

INSERT INTO #EntityTypes
	SELECT [Id] 
		FROM [dbo].[EntityType]
		WHERE [Name] IN (
	'BDCA_UserGroupRole','BDCA_User','BDCA_SchoolYears','BDCA_SchoolDays','BDCA_RoleExternalGroup','BDCA_RoleContentType','BDCA_ManyToMany','BDCA_Job','BDCA_GroupSocial','BDCA_GroupRole','BDCA_GroupHour','BDCA_Group','BDCA_GroupContact','BDCA_EmployeeManager','DataDictionaryValues','DataDictionary','BDCA_CategoryUser','BDCA_CategoryGroup'	
	)


CREATE TABLE #Operations
(
	[Id] UNIQUEIDENTIFIER NOT NULL
)

INSERT INTO #Operations
	SELECT ed.[Id]
		FROM [dbo].[EntityDeletion] ed
			INNER JOIN #EntityTypes et ON ed.[EntityTypeId] = et.[Id]

DELETE
	FROM ed
	FROM [dbo].[EntityDeletion] ed
		INNER JOIN #EntityTypes et ON ed.[EntityTypeId] = et.[Id]

DELETE
	FROM f
	FROM [dbo].[Feed] f
		INNER JOIN #EntityTypes et ON f.[EntityTypeId] = et.[Id]

DELETE
	FROM p
	FROM [dbo].[Parameter] p
		INNER JOIN #EntityTypes et ON p.[EntityTypeId] = et.[Id]

DELETE
	FROM sssi
	FROM [dbo].[SharedSourceSystemIdentifier] sssi
		INNER JOIN #EntityTypes et ON sssi.[EntityTypeId] = et.[Id]

DELETE
	FROM im
	FROM [dbo].[IdentityMessage] im
		INNER JOIN [dbo].[IdentityObject] iobj ON im.[Id] = iobj.[Id]
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			iobj.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM isse
	FROM [dbo].[IdentitySourceSystemEntity] isse
		INNER JOIN [dbo].[IdentityObject] iobj ON isse.[Id] = iobj.[Id]
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			iobj.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM iobj
	FROM [dbo].[IdentityObject] iobj
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			iobj.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM vc
	FROM [dbo].[ValueChange] vc
		INNER JOIN [dbo].[MappingEntityTransaction] met ON
			vc.[MappingEntityTransactionId] = met.[Id]
		INNER JOIN [dbo].[Mapping] m ON met.[MappingId] = m.[Id]
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			m.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

INSERT INTO #Operations
	SELECT iet.[Id]
		FROM [dbo].[IdentityEntityTransaction] iet
			INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
				iet.[SourceSystemEntityIdentityId] = ssei.[Id]
			INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

INSERT INTO #Operations
	SELECT met.[Id]
		FROM [dbo].[MappingEntityTransaction] met
			INNER JOIN [dbo].[Mapping] m ON met.[MappingId] = m.[Id]
			INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
				m.[SourceSystemEntityIdentityId] = ssei.[Id]
			INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM met
	FROM [dbo].[MappingEntityTransaction] met
		INNER JOIN #Operations o ON met.[Id] = o.[Id]

INSERT INTO #Operations
	SELECT msc.[Id]
		FROM [dbo].[MappingStateChange] msc
			INNER JOIN [dbo].[Mapping] m ON msc.[MappingId] = m.[Id]
			INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
				m.[SourceSystemEntityIdentityId] = ssei.[Id]
			INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM msc
	FROM [dbo].[MappingStateChange] msc
		INNER JOIN [dbo].[Mapping] m ON msc.[MappingId] = m.[Id]
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			m.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM m
	FROM [dbo].[Mapping] m
		INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON
			m.[SourceSystemEntityIdentityId] = ssei.[Id]
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM osse
	FROM [dbo].[OperationSourceSystemEntity] osse
		INNER JOIN #Operations o ON osse.[Id] = o.[Id]

DELETE
	FROM iet
	FROM [dbo].[IdentityEntityTransaction] iet
		INNER JOIN #Operations o ON iet.[Id] = o.[Id]

DELETE
	FROM et
	FROM [dbo].[EntityTransaction] et
		INNER JOIN #Operations o ON et.[Id] = o.[Id]

DELETE
	FROM odse
	FROM [dbo].[OperationDestinationSystemEntity] odse
		INNER JOIN #Operations o ON odse.[Id] = o.[Id]

DELETE
	FROM om
	FROM [dbo].[OperationMessage] om
		INNER JOIN #Operations o ON om.[OperationId] = o.[Id]

DELETE
	FROM o
	FROM [dbo].[Operation] o
		INNER JOIN #Operations oids ON o.[Id] = oids.[Id]

DELETE
	FROM ssei
	FROM [dbo].[SourceSystemEntityIdentity] ssei
		INNER JOIN #EntityTypes et ON ssei.[EntityTypeId] = et.[Id]

DELETE
	FROM et
	FROM [dbo].[EntityType] et
		INNER JOIN #EntityTypes etids ON et.[Id] = etids.[Id]

DROP TABLE #Operations

DROP TABLE #EntityTypes

--ROLLBACK TRANSACTION
COMMIT TRANSACTION
