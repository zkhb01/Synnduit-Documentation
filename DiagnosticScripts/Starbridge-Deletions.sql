-- run to get the Deletions
-- Deletion Outcome meaning
-- 1: 'Deleted';
-- 2: 'Not Found';
-- 3: 'Exception Thrown';


-- run when Console views are present
--SELECT TOP (1000) [Id]
--      ,[TimeStamp]
--      ,[Outcome]
--      ,[EntityTypeId]
--      ,[DestinationSystemEntityIdentityEntityTypeId]
--      ,[DestinationSystemEntityIdentifierId]
--      ,[DestinationSystemEntityId]
--      ,[SerializedDestinationSystemEntityId]
--  FROM [Console].[EntityDeletion] 
--    WHERE [TimeStamp] > '2020-05-11T12:32:59.5105910-06:00'

--  order by 2 desc



-- run when no Console views are present
SELECT ed.[Id],
		o.[TimeStamp],
		ed.[Outcome],
  	    et.[Name] EntityName,
		ed.[DestinationSystemEntityId] -- select *
	FROM [dbo].[EntityDeletion] ed
		INNER JOIN [dbo].[Operation] o ON ed.[Id] = o.[Id]
		LEFT OUTER JOIN [dbo].[OperationDestinationSystemEntity] odse ON o.[Id] = odse.[Id]
  JOIN dbo.EntityType et on et.Id = ed.EntityTypeId
    WHERE [TimeStamp] > '2024-07-26T00:36:43.4620504-06:00' --'2021-06-07 14:33:07 -06:00'
	--and [outcome] = 1
	and  et.[Name] like 'Student%'
	--and DestinationSystemEntityId in ('500875c1-7210-47db-a4dd-3d8f8fe43191'	)
--	and ed.[DestinationSystemEntityId] =  '500875c1-7210-47db-a4dd-3d8f8fe43191'
  --and DestinationSystemEntityId like '%014fbb74-cf73-408b-8528-a00403b75624/f84bed19-a163-4385-8935-7880b7c8c925%'
  --and ed.DestinationSystemEntityIdentifierId in (  '53AD795B-D210-4B47-A3A9-9A9548AF1ECF')

   order by [Timestamp] desc
   --014fbb74-cf73-408b-8528-a00403b75624/f84bed19-a163-4385-8935-7880b7c8c925

   -------------------

   --UPDATE m
--       SET m.[State] = 'R'
--       FROM [dbo].[Mapping] m
--              INNER JOIN [dbo].[EntityType] et ON m.[EntityTypeId] = et.[Id]
--              INNER JOIN [dbo].[ExternalSystem] ds ON et.[DestinationSystemId] = ds.[Id]
--       WHERE ds.[Name] = 'SharePoint OnLine' AND m.[State] <> 'R'



--select *  -- select distinct m.id
--from [dbo].[Mapping] m
--        join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
--		join MappingEntityTransaction met on met.MappingId = m.Id
--		join EntityTransaction et on et.Id = met.Id
--		join EntityType ety on ety.Id =  ssei.EntityTypeId
--		join Operation o on o.Id = et.Id
--		--where et.Outcome = 6 and ety.Id = '90559B7B-53E6-4F59-B4B7-F3EE25795E36'  order by [Timestamp] desc   --and o.[TimeStamp] > '2022-09-07T10:00:00.3435333-06:00'
--		--where m.Id = 'DD7878BE-ABD9-43FD-8025-A3C852904B76'
--		--where m.Id in ('919452E7-8B4F-4612-9708-EC29C368239A','4F0D080E-2CC5-4689-B3AE-F97127D06CCC'
--	   --,'29CD217B-7527-4550-9864-BA0009E381F9','68A42030-9B03-4FB1-94F6-A5D09007B40C'
--	   --,'B956C3C3-7D35-45C6-BAFB-56566D12D15E','2042D24B-15F1-4801-A3F9-F80909DA62C2')
--    --where m.DestinationSystemEntityId like '65083AC0-4193-4E92-AE1F-3AF1D0C8ED83' --or m.DestinationSystemEntityId = 'e3585f4a-9ad0-4f21-ae9b-d427ed6084d1' order by  [Timestamp] desc
--	--where o.TimeStamp > '2024-04-29'
--	  where  ssei.SourceSystemEntityId like  ('800671%')
--	   --and ssei.SourceSystemId = '87A05A3A-89F1-4058-9B17-0EE388304E99'
--	   and ety.Name = 'SectionTeacher' -- order by [Timestamp] desc
--	   order by [Timestamp] desc 

