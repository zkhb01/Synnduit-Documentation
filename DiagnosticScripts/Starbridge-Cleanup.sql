--UPDATE m
--       SET m.[State] = 'R'
--       FROM [dbo].[Mapping] m
--              INNER JOIN [dbo].[EntityType] et ON m.[EntityTypeId] = et.[Id]
--              INNER JOIN [dbo].[ExternalSystem] ds ON et.[DestinationSystemId] = ds.[Id]
--       WHERE ds.[Name] = 'SharePoint OnLine' AND m.[State] <> 'R'

--select * from entityType

select *  -- select distinct m.id
from [dbo].[Mapping] m
        join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
		join MappingEntityTransaction met on met.MappingId = m.Id
		join EntityTransaction et on et.Id = met.Id
		join EntityType ety on ety.Id =  ssei.EntityTypeId
		join Operation o on o.Id = et.Id
		--where et.Outcome = 6 and ety.Id = '90559B7B-53E6-4F59-B4B7-F3EE25795E36'  order by [Timestamp] desc   --and o.[TimeStamp] > '2022-09-07T10:00:00.3435333-06:00'
		--where m.Id = 'DD7878BE-ABD9-43FD-8025-A3C852904B76'
		--where m.Id in ('919452E7-8B4F-4612-9708-EC29C368239A','4F0D080E-2CC5-4689-B3AE-F97127D06CCC'
	   --,'29CD217B-7527-4550-9864-BA0009E381F9','68A42030-9B03-4FB1-94F6-A5D09007B40C'
	   --,'B956C3C3-7D35-45C6-BAFB-56566D12D15E','2042D24B-15F1-4801-A3F9-F80909DA62C2')
    --where m.DestinationSystemEntityId like 'ad596b91-654a-4ed1-a9a2-a407dda174c6' -- '8efd0d1c-bff9-42b4-a166-b750099d86cd' --'fb9a5a15-9a9b-4c1d-9794-7c310b22b77f' --or m.DestinationSystemEntityId = 'e3585f4a-9ad0-4f21-ae9b-d427ed6084d1' order by  [Timestamp] desc
	  where  ssei.SourceSystemEntityId like '%_0025570%'
	  --or ssei.SourceSystemEntityId like  ('41557719-B088-4A89-BB1C-B741C1C335E6'))
	  --or ssei.SourceSystemEntityId like  ('0025308%')
	   --and ssei.SourceSystemId = '87A05A3A-89F1-4058-9B17-0EE388304E99'
	   --and ety.Name like 'Student' -- order by [Timestamp] desc
	   and m.State <> 3
	   order by [Timestamp] desc 



--select * from ExternalSystem s where s.Id in ('DA958D35-0EE8-4B25-91EE-B0990B021098','2CA6A1DE-3317-472A-AEA0-D29CB3B4ECDD')

	  -- select *
       --from [dbo].[Mapping] m
       --       inner join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
		--	  join MappingEntityTransaction met on met.MappingId = m.Id
		--join EntityTransaction et on et.Id = met.Id
		--join EntityType ety on ety.Id =  ssei.EntityTypeId
		--join Operation o on o.Id = et.Id
		--where m.Id = 'DD7878BE-ABD9-43FD-8025-A3C852904B76'
		--where et.Outcome = 6 and ety.Name = 'StaffJob'
       --where m.DestinationSystemEntityId like 'https://calgarycatholicschools.sharepoint.com/teams/lists,23f0570d-2fe5-44b4-ae1d-e828cb2caf5d,%'
       --where DestinationSystemEntityId like '%49B9C97A-CED6-4822-9CB3-0E47D748D6D0%'
	   --where ssei.[SourceSystemEntityId] LIKE '%,0b404b39-6125-4c2a-b186-70db136a1faa,9f886135-3e37-41e0-af81-486fa6af5869,7bfb21f2-3aee-42c1-9862-556e76e93a6c' -- all sites, Pages list, sudstf role
       --where ssei.[SourceSystemEntityId] LIKE 'a4446cbe-2be2-48e7-9986-c90288d8cd76,0b404b39-6125-4c2a-b186-70db136a1faa,9f886135-3e37-41e0-af81-486fa6af5869,7bfb21f2-3aee-42c1-9862-556e76e93a6c' -- IMAT site, Pages list, sudstf role
       --where ssei.[SourceSystemEntityId] LIKE '%4a357dda-b322-4deb-bcac-39f57b88c0df%' -- IMAT site, Pages list
	   --                                      b0d6117e-7ac5-43ef-8710-68aac556f0d2_90f270b2-2b18-43d7-8609-becf6ea05ea4_5cd40323f30142909e40672e70dc0027

	  -- order by SourceSystemEntityId
/*
DECLARE @RC int
DECLARE @mappingId uniqueidentifier
DECLARE @operationId uniqueidentifier
DECLARE @timeStamp datetimeoffset(7)
DECLARE @state int

set @mappingId = 'AE2882C0-5375-44F8-BC9E-BB52FE9932A8'
set @operationId = newid();
set @timeStamp = getdate();
-- TODO: Set parameter values here.

EXECUTE @RC = [dbo].[SetMappingState] 
   @mappingId
  ,@operationId
  ,@timeStamp
  ,'3'
GO
*/



-- Skipped = 1,
-- Rejected = 2,
-- NoChangesDetected = 3
-- NoChangesMerged = 4
-- ChangesDetectedAndMerged = 5
-- NotFoundInDestinationSystem = 6
-- DuplicateDetectedChangesMerged = 7
-- DuplicateDetectedNoChangesMerged = 8
-- ReferredForManualDeduplication = 9
-- NewEntityCreated = 10
-- ExceptionThrown = 11
  
  /*
      public enum EntityTransactionOutcome
    {
        /// <summary>
        /// The entity was skipped, as an existing mapping was found for it, and the
        /// settings specified that entities with existing mappings should be skipped.
        /// </summary>
        Skipped = 1,

        /// <summary>
        /// The entity was rejected by a preprocessor operation; this means it can't be
        /// processed at this point; this does NOT, however, indicate an error, and another
        /// attempt to process the entity should be made later. (E.g., another entity that
        /// the entity being processed depends on may not yet exist in the destination
        /// system.)
        /// </summary>
        Rejected = 2,

        /// <summary>
        /// An existing mapping was found for the entity, and no changes were detected
        /// between the previous and current versions of the entity; as a result, no action
        /// was taken.
        /// </summary>
        NoChangesDetected = 3,

        /// <summary>
        /// An existing mapping was found for the entity, and changes were detected between
        /// the previous and current versions of the entity; however, the merge resulted in
        /// no changes being propagated into the destination system entity; consequently,
        /// no need existed to update the destination system entity; the current version of
        /// the (source system) entity on record was updated, and no further action was
        /// taken.
        /// </summary>
        NoChangesMerged = 4,

        /// <summary>
        /// An existing mapping was found for the entity, and changes were detected between
        /// the previous and current versions of the entity; the changes were merged into
        /// the destination system entity.
        /// </summary>
        ChangesDetectedAndMerged = 5,

        /// <summary>
        /// An existing mapping was found for the entity, and changes were detected between
        /// the previous and current versions of the entity; however, the entity could not
        /// be found in the destination system; the current version of the entity on record
        /// was updated, and no futher action was taken.
        /// </summary>
        NotFoundInDestinationSystem = 6,

        /// <summary>
        /// No existing mapping for the entity was found; it was determined that the entity
        /// represents a duplicate of an existing entity in the destination sytem; a
        /// mapping was created, and the entity was merged into the one in the destination
        /// system.
        /// </summary>
        DuplicateDetectedChangesMerged = 7,

        /// <summary>
        /// No existing mapping for the entity was found; it was determined that the entity
        /// represents a duplicate of an existing entity in the destination sytem; a
        /// mapping was created, and no pending value changes were detected during the
        /// attempt to merge the entity into the one in the destination system.
        /// </summary>
        DuplicateDetectedNoChangesMerged = 8,

        /// <summary>
        /// No existing mapping for the entity was found; it was determined that the entity
        /// MAY represent an existing entity in the destination system (one or more
        /// candidate dupes were found), and the entity was referred for manual
        /// deduplication.
        /// </summary>
        ReferredForManualDeduplication = 9,

        /// <summary>
        /// No existing mapping for the entity was found; it was determined that the entity
        /// DOES NOT represent an existing entity in the destination system; a new entity
        /// was created in the destination system.
        /// </summary>
        NewEntityCreated = 10,

        /// <summary>
        /// An exception was thrown while the entity was being processed.
        /// </summary>
        ExceptionThrown = 11
    }
}

  */

