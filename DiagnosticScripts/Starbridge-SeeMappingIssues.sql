 
SELECT [EntityTypeId], [SourceSystemId], [SourceSystemEntityId], COUNT(*)
  FROM [dbo].[EntityMapping]
  WHERE [State] <> 3
  GROUP BY [EntityTypeId], [SourceSystemId], [SourceSystemEntityId]
  HAVING COUNT(*) > 1


SELECT et.[Name] AS [EntityType],
              es.[Name] AS [SourceSystem],
              ssei.[SourceSystemEntityId],
              COUNT(*) AS [Count]
       FROM [dbo].[Mapping] m
              INNER JOIN [dbo].[SourceSystemEntityIdentity] ssei
                     ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
              INNER JOIN [dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
              INNER JOIN [dbo].[EntityType] et ON ssei.[EntityTypeId] = et.[Id]
       WHERE m.[State] <> 3
       GROUP BY et.[Name], es.[Name], ssei.[SourceSystemEntityId]
       HAVING COUNT(*) > 1
       ORDER BY COUNT(*) DESC


select MAX(Timestamp) tm
from Operation