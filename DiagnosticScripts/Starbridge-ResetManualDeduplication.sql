DECLARE @RC int
DECLARE @MappingId uniqueidentifier;
DECLARE @NotFoundEntities TABLE
(
	MappingId uniqueidentifier
)

insert into @NotFoundEntities ( MappingId )
select distinct m.Id -- select *
FROM [StarBridge-Stdnt].[dbo].[Mapping] m
              INNER JOIN [StarBridge-Stdnt].[dbo].[SourceSystemEntityIdentity] ssei
                     ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[EntityType] et ON ssei.[EntityTypeId] = et.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[DestinationSystemEntityIdentifier] dsei
                     ON m.[DestinationSystemEntityIdentifierId] = dsei.[Id]
              LEFT OUTER JOIN [DDH].[dbo].[Staff] s_on_id
                     ON dsei.[DestinationSystemEntityId] = s_on_id.[Id]
              INNER JOIN [DDH].[dbo].[Staff] s_on_employeeId
                     ON ssei.[SourceSystemEntityId] = s_on_employeeId.[EmployeeId]
       WHERE et.[Name] = 'Staff' AND m.[State] <> 3
              AND s_on_id.[Id] IS NULL




DECLARE cur CURSOR FOR SELECT MappingId FROM @NotFoundEntities
DECLARE @operationId uniqueidentifier,
@timestamp datetime;

OPEN cur
FETCH NEXT FROM cur INTO @MappingId
WHILE @@FETCH_STATUS = 0 BEGIN
  SET @operationId = NEWID();
  SET @timestamp = GETDATE();
  PRINT convert(varchar(36), @MappingId)+ ', ' + convert(varchar(36), @operationId) + ', ' + convert(varchar(36), @timestamp)
  EXECUTE @RC = [dbo].[SetMappingState]    @MappingId, @operationId, @timestamp, 3
  FETCH NEXT FROM cur INTO @MappingId
END

CLOSE cur
DEALLOCATE cur