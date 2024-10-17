
SELECT	o.[TimeStamp] [TimeStamp],
        etp.[Name] [EntityType],
        es.[Name] [SourceSystemName],
        ssei.[SourceSystemEntityId],
		m.[DestinationSystemEntityId],
		et.[Outcome]
	FROM [dbo].[EntityTransaction] et
        INNER JOIN [dbo].[Operation] o ON et.[Id] = o.[Id]
        INNER JOIN [dbo].[IdentityEntityTransaction] iet ON et.[Id] = iet.[Id]
        INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei ON iet.[SourceSystemEntityIdentityId] = ssei.[Id]
        INNER JOIN [dbo].[EntityType] etp ON ssei.[EntityTypeId] = etp.[Id]
        INNER JOIN [dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
        INNER JOIN [dbo].[OperationSourceSystemEntity] osse ON o.[Id] = osse.[Id]
        INNER JOIN [dbo].[SerializedEntity] se ON osse.[SerializedEntityId] = se.[Id]
		LEFT JOIN [dbo].[Mapping] m on m.SourceSystemEntityIdentityId = iet.[SourceSystemEntityIdentityId]
    WHERE et.[Outcome] = 9 
	and o.TimeStamp > '2023-08-20T13:26:56.9586967-06:00'
	order by o.[TimeStamp] desc
