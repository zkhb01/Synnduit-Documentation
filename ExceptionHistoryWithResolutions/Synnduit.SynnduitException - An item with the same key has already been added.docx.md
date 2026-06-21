# Synnduit.SynnduitException - An item with the same key has already been added

> Extracted from `Synnduit.SynnduitException - An item with the same key has already been added.docx`

Synnduit.SynnduitException - An item with the same key has already been added

Loading entity identifier mappings ... Synnduit.SynnduitException: An exception occurred during the run; see inner exception for details. ---> System.InvalidOperationException: An IInitializable instance threw an exception; see inner exception for details. ---> System.ArgumentException: An item with the same key has already been added.

at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)

Starbridge database may be corrupt. Trying to restore a backup.  This worked.

It may be possible that concurrent jobs were running against the same starbridge database which is a known No-No!

Run query in C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\SqlScripts\Starbridge\StarbridgeCorruptionCheck.sql to see if any rows show up. If not rows not corrupted. If rows show you have 2 options.

Restore the most recent backup. As you restore, Check backup but restoring locally and running the same query to confirm 0 rows returned.

Try using the Clean script to deactivate the duplicate rows (less desirable, potential loss of history) or use the "SetMappingState" stored proc option below.

Connected with Jon and the issue actually related to the Key we used for CommonListAllStaff. It included the EMAIL. Our upgrade to support the new AD integration had the emails all standardized to lowercase.

This switching between lower case exposed a bug in starbridge where on the ifeed side it was case sensitive and the iSnk side it was not. This then allowed Starbridge to add a 2nd entry to a sourcekeyIdentifier. This shows the same error as having run concurrent processes against a starbridge database.

Jon upgraded dev branch of Synnduit and we applied the new complied code an d that fixed the starbridge side of the problem. The SharePoint list then threw exception and the key was not allowed to be updated now that the new lowercase emails were coming through.

We now have to fix the key used for the SharePoint list. Lesson learnt do not use mutable fields in a composite key.

//----------------------------------------------------

Another option to try cleanup the mapping table:

USE [StarBridge-DDH-OldProd]

GO

/****** Run this to deal with error "System.ArgumantException: An item with the same key has already been added" ******/

/* Run this to identify the items with the same key */

SELECT [EntityTypeId], [SourceSystemId], [SourceSystemEntityId], COUNT(*)

FROM [dbo].[EntityMapping]

WHERE [State] <> 3

GROUP BY [EntityTypeId], [SourceSystemId], [SourceSystemEntityId]

HAVING COUNT(*) > 1

ORDER BY COUNT(*) DESC

GO

/* Then run this Stabridge storedProc to inactivate the dupicates. Take to most rescent, its usually the culpret */

ALTER PROCEDURE [dbo].[SetMappingState]

@mappingId UNIQUEIDENTIFIER, -- Mapping Id of Entity you want to diable

@operationId UNIQUEIDENTIFIER, -- A new Guid

@timeStamp DATETIMEOFFSET(7), -- Current Timestamp

@state INT -- Set to 3 to disable

AS

DECLARE @previousState CHAR(1)

BEGIN

EXECUTE [dbo].[EnsureOperationExists] @operationId, @timeStamp

SELECT @previousState = [State] FROM [dbo].[Mapping] WHERE [Id] = @mappingId

UPDATE [dbo].[Mapping] SET [State] = @state WHERE [Id] = @mappingId

INSERT INTO [dbo].[MappingStateChange]

VALUES(@operationId, @mappingId, @previousState)

END
