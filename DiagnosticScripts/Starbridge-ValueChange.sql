-- run to see the value changes made
	SELECT [DestinationSystem]
      ,[EntityType]
      ,[DestinationSystemEntityId]
      ,[SourceSystem]
      ,[SourceSystemEntityId]
      ,[TimeStamp]
      ,[ValueName]
      ,[PreviousValue]
      ,[NewValue] -- select *
  FROM [dbo].[EntityValueChange]
  WHERE [TimeStamp] >  '2025-03-12T14:07:29.3313552-06:00'   
  
  --AND EntityType like 'Staff'
  --AND [TimeStamp] < '2023-03-30T23:19:49.4045983-06:00' --and '2020-03-25T18:13:37.1337707-06:00'
  --and ValueName = 'JobId'
 --and SourceSystemEntityId like '%0025570%' --or SourceSystemEntityId like 'LDC1051'
 --and DestinationSystemEntityId like '95B95264-3017-494A-952F-F1A0BACA82FC'
 --and PreviousValue is null
  --and charindex('''',NewValue)  = 0
  --and NewValue = '2'
  --and Lower(NewValue) = 'kume.ajayi'
  --and ValueName = 'ManagerId'
  --and substring(NewValue,1,10) !=  substring(PreviousValue,1,10) -- use for comparing just the data part of a timestamp
  ORDER BY [TimeStamp] DESC


  

