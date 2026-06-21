# Synnduit Deduplication sequence

> Extracted from `Synnduit Deduplication sequence.docx`

Here are the steps the Insert crud process takes to add a new record that already exists in the destination system.

- Read iFeed and process deltas
- For an insert , check using the key you defined as the Duplication key in your poco class if the record exists in the destination:
namespace Cssd.IT.ActiveDirectoryIntegration.ActiveDirectory
{

[EntityType(

"AAB7C01F-A4E6-40F1-9DA3-C12CD7976689",

typeof(AzureActiveDirectory),

OrphanMappingBehavior = OrphanMappingBehavior.Remove,

GarbageCollectionBehavior = GarbageCollectionBehavior.DeleteMapped)]

public class User

{

[SourceSystemIdentifier]

public EntityIdentifier StaffId { get; set; }

[DestinationSystemIdentifier]

public EntityIdentifier Id { get; set; }

[EntityProperty]

[DuplicationKey]

public string UserPrincipalName { get; set; }

[EntityProperty]

public bool? ForceChangePasswordNextSignIn { get; set; }

}

}

- If there is no record, then add a new record to the destination.
- If not, then call the Update method, and if both source and destination columns match, it will process as “Duplicate detected, no changes merged”,
if they don’t match, then record the delta and update the record as “Duplicate detected, changes merged”.
