# Synnduit - No changes detected – when doing initial load

> Extracted from `Synnduit - No changes detected – when doing initial load.docx`

No changes detected – when all doing initial load

From: Karl Burndorfer 
Sent: Wednesday, May 18, 2022 07:34
To: Jon Suda <jon@synnduit.com>
Cc: Angela Sebella <Angela.Sebella@cssd.ab.ca>
Subject: SourceSystemEntityId fields spans class and extended class but treated as duplicates.

Hi Jon. I’m having issues with  “No change detected” when doing initial load. I confirmed the source records are unique based on the same key.

C:\IT_Portal\Dev\Source\PortalIntegration-StarBridge\bin>PowerSchool-Som.cmd

Segment 1 out of 2:

Start time: 2022-05-18T06:40:45.5144321-06:00

Type: Migration

Source system: Power School

Destination system: School Object Model

Entity type: GuardianContact

Loading entity identifier mappings ... 670,699 mappings loaded.

Caching destination system entities ... 0 entities cached.

Indexing the entity cache ... 1 index created.

Loading source system entities ... 349,084 entities loaded.

Skipped:

Rejected:

No changes detected: 725 ( 725 )

No changes merged:

Changes detected & merged:

Not found in destination system:

Duplicate detected, changes merged:

Duplicate detected, no changes merged:

Referred for manual deduplication:

New entity created: 444 ( 444 )

Exception thrown:

Migration progress: .33%

I was wondering if the fields can’t span 2 tables: GuardianContact and Contact

Here’s the class for GuardianContact:

[Table("[stdnt].[GuardianContact]")]

[EntityType(

"BF2DACA5-6F31-4F30-BAD8-C429E3A9C881",

typeof(SchoolObjectModel))]

public class GuardianContact : Contact

{

[ForeignKey(nameof(Guardian))]

[EntityProperty]

[ReferenceIdentifier(typeof(Guardian), nameof(GuardianCode))]

public Guid GuardianId { get; set; }

public Guardian Guardian { get; set; }

[NotMapped]

public string GuardianCode

{

get

{

return $"{this.StudentNumber}_{this.RelationshipCode}";

}

}

[NotMapped]

public int StudentNumber { get; set; }

[NotMapped]

public string RelationshipCode { get; set; }

[Column(TypeName = "varchar")]

[MaxLength(255)]

[EntityProperty]

public string For { get; set; }

[NotMapped]

[SourceSystemIdentifier]

public string SourceSystemEntityId

{

get

{

return string.Format(

"{0}_{1}_{2}", this.GuardianCode, this.For, this.Label); ç the first 2 fields are on the GuadianContact table and the 3rd is on the Contact table

}

}

[NotMapped]

[DuplicationKey]

public string DuplicationKey

{

get

{

return string.Format(

"{0}_{1}_{2}", this.GuardianId, this.For, this.Label);

}

}

}


### Root Cause:

The Keys were not set appropriately on the iFeed data class. Cut/paste error and only 2 of the 4 required columns composed the unique key.
