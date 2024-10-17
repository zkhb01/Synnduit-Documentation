
SELECT	m.Id MappingId,
        etp.[Name] [EntityType],
        es.[Name] [SourceSystemName],
        ssei.[SourceSystemEntityId],
		m.[DestinationSystemEntityId], 
		o.[TimeStamp] [TimeStamp],
		et.[Outcome] -- select *
	FROM [dbo].[EntityTransaction] et
        INNER JOIN [dbo].[Operation] o ON et.[Id] = o.[Id]
		left join MappingEntityTransaction met on met.Id = et.Id
		left join Mapping m on m.Id = met.MappingId
		left join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
        left JOIN [dbo].[EntityType] etp ON ssei.[EntityTypeId] = etp.[Id]
        left JOIN [dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
    WHERE et.[Outcome] = 6 
	AND m.[State] <> 3
	--AND o.[TimeStamp] > '2023-08-01'
	order by o.[TimeStamp] desc

--select * from Mapping m where m.Id = '9FA9D25C-640A-492F-AD7B-C8BFDC914166'