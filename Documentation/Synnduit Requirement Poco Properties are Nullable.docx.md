# Synnduit Requirement Poco Properties are Nullable

> Extracted from `Synnduit Requirement Poco Properties are Nullable.docx`

When defining an entity to be synced in Starbridge the Poco class must provide nullable properties for any source field that is not. IE: Source field is integer, bool, DateTime…

Instead of coding a typical property:

[Required]

[EntityProperty]

public int EnrollmentStatus { get; set; }

Provide a nullable proxy property

[NotMapped]

public int? NullableEnrollmentStatus { get; set; }

And connect it to the actual int property via the ‘NullableProxyProperty’ annotation:

[Required]

[EntityProperty(NullableProxyProperty = nameof(NullableEnrollmentStatus))]

public int EnrollmentStatus { get; set; }

Then when mapping the iFeed Poco entity, you assign the incoming value to the nullable property:

private void Map(OracleDataReader reader, Student c)

{

c.AlbertaStudentNumber = reader.IsDBNull(2) ? null : reader.GetString(2).Trim();

c.BirthDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7);

c.DCID = reader.IsDBNull(0) ? null : reader.GetInt32(0);

c.NullableEnrollmentStatus = reader.GetInt32(11);

c.EntryDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8);

StarBridge will make the assignment internally to the not null property linked via the annotation.

If not-null properties are not coded this way it may seem that they work, but changes will come along that won’t be picked up.  Correct the situation be applying the corrective code above and rerun your app in test. The Starbridge console will show the progress of correcting the property in question with “No changes merged” entries being counted.

If the property you are providing a nullable proxy for is part of the SourceSystemEntityId, you could opt to provide the SourceSystemEntityId a getter/setter and in the iFeed map the values directly from the source.

[NotMapped]

[SourceSystemIdentifier]

public string SourceSystemEntityId { get; set; }

public IEntityCollection<Guardian> LoadEntities()

{

using var context = this.powerSchoolContextFactory.CreatePowerSchoolContext<Guardian>();

return context.Entity(Map);

}

private void Map(OracleDataReader reader, Guardian c)

{

c.StudentNumber = reader.GetInt32(0);

c.NullablePersonId = reader.GetInt32(1);

c.FirstName = reader.IsDBNull(3) ? null : reader.GetString(3).Trim();

…

c.SourceSystemEntityId = string.Format("{0}_{1}", c.StudentNumber, c.NullablePersonId);

}

}
