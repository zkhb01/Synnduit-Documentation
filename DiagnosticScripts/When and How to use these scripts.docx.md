# When and How to use these scripts

> Extracted from `When and How to use these scripts.docx`


## Work in progress to document this for the popular scripts used:

- Starbridge-RemoveObsoleteEntityTypes.sql
- Used to remove an obsolete entity type from StarBridge (class associated with StarBridge for data syncing)
- The script provides for you to enter the table / entity via:
INSERT INTO #EntityTypes
	SELECT [Id]
		FROM [dbo].[EntityType]
		WHERE [Name] IN ('-- Type your entity type name here --'
