 --public enum MappingState
 --   {
 --       /// <summary>
 --       /// The mapping is currently active.
 --       /// </summary>
 --       Active = 1,

 --       /// <summary>
 --       /// The mapping has been deactivated.
 --       /// </summary>
 --       Deactivated = 2,

 --       /// <summary>
 --       /// The mapping has been removed.
 --       /// </summary>
 --       Removed = 3
 --   }

 --query to list subset of Not-Found-In-Destination records
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
	--AND o.[TimeStamp] > '2023-01-01'
		and m.State <> 3
	order by o.[TimeStamp] desc

 select *
from Mapping m 
	 join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
Where ssei.EntityTypeId = '940E6483-B3F0-49F4-A0B6-0E42CE4F9E64' and [State] <> 3 and  ssei.SourceSystemEntityId in 
('8b82d10a-92ac-47a7-8ce4-31a916d45e2c'
)


-- script to reset them to allow Insert again

DECLARE @RC int
DECLARE @MappingId uniqueidentifier;
DECLARE @NotFoundEntities TABLE
(
	MappingId uniqueidentifier
)

INSERT INTO
	@NotFoundEntities
(
	MappingId
)
select distinct m.Id -- select *
	FROM [dbo].[EntityTransaction] et
        INNER JOIN [dbo].[Operation] o ON et.[Id] = o.[Id]
		left join MappingEntityTransaction met on met.Id = et.Id
		left join Mapping m on m.Id = met.MappingId
		left join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
        left JOIN [dbo].[EntityType] etp ON ssei.[EntityTypeId] = etp.[Id]
        left JOIN [dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
    WHERE et.[Outcome] = 6  	and m.State <> 3
	--AND o.[TimeStamp] > '2023-02-24'

		--from [dbo].[Mapping] m
  --      join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
		--join MappingEntityTransaction met on met.MappingId = m.Id
		--join EntityTransaction et on et.Id = met.Id
		--join EntityType ety on ety.Id =  ssei.EntityTypeId
		--join Operation o on o.Id = et.Id
		----where et.Outcome = 6 and ety.Id = '90559B7B-53E6-4F59-B4B7-F3EE25795E36'  order by [Timestamp] desc   --and o.[TimeStamp] > '2022-09-07T10:00:00.3435333-06:00'
		----where m.Id = 'DD7878BE-ABD9-43FD-8025-A3C852904B76'
		----where m.Id in ('919452E7-8B4F-4612-9708-EC29C368239A','4F0D080E-2CC5-4689-B3AE-F97127D06CCC'
	 --  --,'29CD217B-7527-4550-9864-BA0009E381F9','68A42030-9B03-4FB1-94F6-A5D09007B40C'
	 --  --,'B956C3C3-7D35-45C6-BAFB-56566D12D15E','2042D24B-15F1-4801-A3F9-F80909DA62C2')
  --    --where m.DestinationSystemEntityId like '8A904F6C-6FD4-4D99-9412-1BED7327E5B7%' order by  [Timestamp] desc
	 --  where ssei.SourceSystemEntityId like '39BD0DBF-36C4-43B9-B38E-A80D98591100%' 

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
--0ACB80DC-9A2B-4616-BA74-4C6743B5AF16

--select * from [dbo].[Mapping] m
----UPDATE [dbo].[Mapping] set DestinationSystemEntityId = 'EA2FA366-11D8-4953-A8D8-940FEAB3AA43' -- old value 70a5c9e0-0f7e-446d-ac65-f2f7447fe549
----SET [State] = 2
--where Id = 'A810A7B0-1A41-4B7E-B545-CDF04F0D2637'


--select * from [dbo].Mapping m
--left join [MappingStateChange] msc on m.Id = msc.MappingId
--where m.Id in (
--'BB8550F3-5F68-41BF-BA39-AB53339539C3'
--,'FA5C2C34-90DA-49DB-BE82-818B8B0E442E'
--,'F4881896-B97F-4056-AC43-EAA11215DF3F'
--,'E5395B73-333B-44BD-AFA1-AA7AB2CE2B2E'
--,'A810A7B0-1A41-4B7E-B545-CDF04F0D2637'
--)
--order by m.Id

--Declare @MappingId uniqueidentifier = '769368D0-2A5E-44E8-A6EC-8BA696571403',
--@RC integer = 0,
--@Date dateTime = getdate(),
--@Id uniqueidentifier = newid();
--EXECUTE @RC = [dbo].[SetMappingState]    @MappingId, @Id, @Date, 3
