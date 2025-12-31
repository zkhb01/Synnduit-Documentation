CREATE VIEW [dbo].[StarbridgeExceptionsRejections]
AS
SELECT o.[Id] AS [OperationId]
,m.[Id] AS [MessageId]
,o.[TimeStamp]
,m.[Type]
,CASE
WHEN m.[Type] = 1 THEN 'Rejection'
WHEN m.[Type] = 2 THEN 'Warning'
WHEN m.[Type] = 3 THEN 'Exception'
WHEN m.[Type] = 4 THEN 'Information'
ELSE 'Unknown' END [TypeName]
,m.[Text]
,ISNULL(et.[Name], etm.[Name]) [EntityType]
,ISNULL(ssei.[SourceSystemEntityId], sseim.[SourceSystemEntityId]) [SourceSystemEntityId]
FROM [dbo].[Operation] o
INNER JOIN [dbo].[OperationMessage] om ON o.[Id] = om.[OperationId]
INNER JOIN [dbo].[Message] m ON om.[MessageId] = m.[Id]
LEFT JOIN [dbo].[MappingEntityTransaction] met ON o.[Id] = met.[Id]
LEFT JOIN [dbo].[IdentityEntityTransaction] osse ON osse.[Id] = o.[Id]
LEFT JOIN [dbo].[SourceSystemEntityIdentity] ssei ON ssei.[Id] = osse.[SourceSystemEntityIdentityId]
LEFT JOIN [dbo].[EntityType] et ON et.[Id] = ssei.[EntityTypeId]
LEFT JOIN [dbo].[Mapping] m2 ON m2.[Id] = met.[MappingId]
LEFT JOIN [dbo].[SourceSystemEntityIdentity] sseim ON sseim.[Id] = m2.[SourceSystemEntityIdentityId]
LEFT JOIN [dbo].[EntityType] etm ON etm.[Id] = sseim.[EntityTypeId]