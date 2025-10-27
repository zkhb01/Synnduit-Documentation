
DECLARE @DestinationSystemEntityIdentifierId uniqueidentifier 
DECLARE @OldDestinationEntityIdValue Uniqueidentifier = 'abc0a1ee-b854-406d-b359-c877cf38d030' -- Provide current Old Dest/ADID  
DECLARE @NewDestinationEntityIdValue Uniqueidentifier = 'ad3d65eb-559c-410e-b9de-8acbc28d3648' -- Go to Azure Entra ID and get ADID 

SELECT @DestinationSystemEntityIdentifierId = DestinationSystemEntityIdentifierId
	FROM Mapping
	WHERE DestinationSystemEntityId =  @OldDestinationEntityIdValue

SELECT * 
	FROM Mapping
	WHERE DestinationSystemEntityIdentifierId =  @DestinationSystemEntityIdentifierId

SELECT *
	FROM [dbo].[DestinationSystemEntityIdentifier]
	WHERE Id = @DestinationSystemEntityIdentifierId


--Update Script 
UPDATE [dbo].[Mapping]
	SET [DestinationSystemEntityId] = @NewDestinationEntityIdValue
	WHERE DestinationSystemEntityIdentifierId = @DestinationSystemEntityIdentifierId

UPDATE [dbo].[DestinationSystemEntityIdentifier]
	SET [DestinationSystemEntityId] = @NewDestinationEntityIdValue
	WHERE Id = @DestinationSystemEntityIdentifierId


--Result
SELECT * 
	FROM Mapping
	WHERE DestinationSystemEntityIdentifierId =  @DestinationSystemEntityIdentifierId

SELECT *
	FROM [dbo].[DestinationSystemEntityIdentifier]
	WHERE Id = @DestinationSystemEntityIdentifierId

