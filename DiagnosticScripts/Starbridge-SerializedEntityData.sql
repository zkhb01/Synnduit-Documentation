select se.[Data],ssei.SourceSystemEntityId, * 
from [dbo].[SourceSystemEntityIdentity] ssei 
join [dbo].[Mapping] m on m.SourceSystemEntityIdentityId = ssei.Id
join MappingEntityTransaction met on met.MappingId = m.Id
join EntityTransaction et on et.Id = met.Id
join [dbo].[OperationSourceSystemEntity] oss on oss.Id = met.Id
join [dbo].[SerializedEntity] se on se.Id = oss.SerializedEntityId
join EntityType ety on ety.Id =  ssei.EntityTypeId
join Operation o on o.Id = et.Id
where  ssei.SourceSystemEntityId like '1066973_Email' 
order by o.TimeStamp desc