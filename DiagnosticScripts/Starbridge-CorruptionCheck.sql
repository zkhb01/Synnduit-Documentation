-- use this query to this exception:
--   Synnduit.SynnduitException - An item with the same key has already been added
--   If rows show you have 2 options.
--   1.	Restore the most recent backup. As you restore, Check backup but restoring locally and running the same query to confirm 0 rows returned. 
--   2.	Try using the Clean script to deactivate the duplicate rows (less desirable, potential loss of history)
 

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

--select MAX(Timestamp) tm from Operation

