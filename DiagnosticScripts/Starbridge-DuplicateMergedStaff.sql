       select o.[TimeStamp], et.[Outcome], ety.[Name] [Entity], ssei.SourceSystemEntityId, s.EmployeeId, s.FirstName, s.LastName	
       from [dbo].[Mapping] m	
        join [dbo].[SourceSystemEntityIdentity] ssei on m.[SourceSystemEntityIdentityId] = ssei.[Id]	
              join [dbo].[MappingEntityTransaction] met on met.[MappingId] = m.[Id]	
              join [dbo].[EntityTransaction] et on et.[Id] = met.[Id]	
              join [dbo].[EntityType] ety on ety.[Id] =  ssei.[EntityTypeId]	
              join [dbo].[Operation] o on o.[Id] = et.[Id]	
              join [SPSQL01].[SchoolObjectModel].[dbo].[Staff] s on s.[Id] = ssei.[SourceSystemEntityId]	
              where et.[Outcome] in (7,8)	
              and ety.[Name] = 'User'	
              and o.[TimeStamp] > CURRENT_TIMESTAMP - 25	