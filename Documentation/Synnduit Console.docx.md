# Synnduit Console

> Extracted from `Synnduit Console.docx`

Synnduit Console

The migrators are scheduled via the Windows Task Scheduler and console output is not available. To be able to view the output to get additional information, you can run them manually via the command prompt. The transaction outcome counts are captured. (See descriptions for them below)

Each logged change detected by a migrator is logged as a transaction and can be queried in StarBridge.  The EntityTransaction table stores these with transaction outcomes set as an integer with the following meanings: 
Source found in the Synnduit source code:

C:\Repos\Synnduit.Core\src\Synnduit.Core\EntityTransactionOutcome.cs

// <summary>

// Represents possible outcomes of the processing of a source system entity.

// </summary>

public enum EntityTransactionOutcome

{

// <summary>

// The entity was skipped, as an existing mapping was found for it, and the

// settings specified that entities with existing mappings should be skipped.

// </summary>

Skipped = 1,

// <summary>

// The entity was rejected by a preprocessor operation; this means it can't be

// processed at this point; this does NOT, however, indicate an error, and another

// attempt to process the entity should be made later. (E.g., another entity that

// the entity being processed depends on may not yet exist in the destination

// system.)

// </summary>

Rejected = 2,

// <summary>

// An existing mapping was found for the entity, and no changes were detected

// between the previous and current versions of the entity; as a result, no action

// was taken.

// </summary>

NoChangesDetected = 3,

// <summary>

// An existing mapping was found for the entity, and changes were detected between

// the previous and current versions of the entity; however, the merge resulted in

// no changes being propagated into the destination system entity; consequently,

// no need existed to update the destination system entity; the current version of

// the (source system) entity on record was updated, and no further action was

// taken.

// </summary>

NoChangesMerged = 4,

// <summary>

// An existing mapping was found for the entity, and changes were detected between

// the previous and current versions of the entity; the changes were merged into

// the destination system entity.

// </summary>

ChangesDetectedAndMerged = 5,

// <summary>

// An existing mapping was found for the entity, and changes were detected between

// the previous and current versions of the entity; however, the entity could not

// be found in the destination system; the current version of the entity on record

// was updated, and no further action was taken.

// </summary>

NotFoundInDestinationSystem = 6,

// <summary>

// No existing mapping for the entity was found; it was determined that the entity

// represents a duplicate of an existing entity in the destination system; a

// mapping was created, and the entity was merged into the one in the destination

// system.

// </summary>

DuplicateDetectedChangesMerged = 7,

// <summary>

// No existing mapping for the entity was found; it was determined that the entity

// represents a duplicate of an existing entity in the destination system; a

// mapping was created, and no pending value changes were detected during the

// attempt to merge the entity into the one in the destination system.

// </summary>

DuplicateDetectedNoChangesMerged = 8,

// <summary>

// No existing mapping for the entity was found; it was determined that the entity

// MAY represent an existing entity in the destination system (one or more

// candidate dupes were found), and the entity was referred for manual

// deduplication.

// </summary>

ReferredForManualDeduplication = 9,

// <summary>

// No existing mapping for the entity was found; it was determined that the entity

// DOES NOT represent an existing entity in the destination system; a new entity

// was created in the destination system.

// </summary>

NewEntityCreated = 10,

// <summary>

// An exception was thrown while the entity was being processed.

// </summary>

ExceptionThrown = 11

}

Console logger will provide an indication of how the merge process went for importers and exporters

If you get some mappings that end up as 'CreateSkipped' and want to debug in place then:

public void EntityProcessed(

object sourceEntity,

object destinationEntity,

EntityMigrationOutcome outcome,

IEnumerable<EntityMessage> messages)

{

MigrationResult.RecordOutcome(outcome); <== put breakpoint here in ConsoleLogger.cs file.

Console.WriteLine(outcome);

}

Then set breakpoint to break on this condition:

at this point you can interogate the 'message', the 'souceEntity', and the 'destinationEntity.

Starbidge also tracks changes and exceptions relating to each migrator run.  To help triage issues see Starbridge Support Features
