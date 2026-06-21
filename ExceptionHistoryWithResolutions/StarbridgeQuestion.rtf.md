# StarbridgeQuestion

> Extracted from `StarbridgeQuestion.rtf`


Q1: After spliting up 2 starbridge db's into 3 there were some issues:
An exception comes up as:
Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
Microsoft.Data.SqlClient.SqlException (0x80131904): The UPDATE statement conflicted with the FOREIGN KEY constraint "FK_Student_School".
The conflict occurred in database "DDH", table "dbo.School", column 'Id'.
How can I solve this?

I caputured a sqlProfiler output of the call causing the Sql Exception:
exec sp_executesql N'SET NOCOUNT ON;
UPDATE [dbo].[Entity] SET [CreatedBy] = @p0, [CreatedOn] = @p1, [DeactivatedDate] = @p2, [DeletedTimestamp] = @p3, [SourceKey] = @p4, [UpdatedBy] = @p5, [UpdatedOn] = @p6
WHERE [Id] = @p7;
SELECT @@ROWCOUNT;

UPDATE [stdnt].[Student] SET [AlbertaStudentNumber] = @p8, [BirthDate] = @p9, [DCID] = @p10, [EnrollmentStatus] = @p11, [EntryDate] = @p12, [ExitDate] = @p13, [FirstName] = @p14, [Grade] = @p15, [IsIndependentStudent] = @p16, [LastName] = @p17, [MiddleName] = @p18, [RegistrationTypeId] = @p19
, [SchoolId] = @p20, [StudentNumber] = @p21
WHERE [Id] = @p22;
SELECT @@ROWCOUNT;

',N'@p7 uniqueidentifier,@p0 varchar(50),@p1 datetime2(7),@p2 datetime2(7),@p3 datetime2(7),@p4 int,@p5 varchar(50),@p6 datetime2(7),@p22 uniqueidentifier,@p8 nvarchar(4000),@p9 datetime2(7),@p10 int,@p11 int,@p12 datetime2(7),@p13 datetime2(7),@p14 varchar(255),@p15 int,@p16 bit,@p17 varchar(255),@p18 varchar(255),@p19 uniqueidentifier,@p20 uniqueidentifier,@p21 int',@p7='EB482121-AAA0-480D-A4A2-9B441BF51B57',@p0='karl.burndorfer@cssd.ab.ca',@p1='2022-05-13 16:03:52.7370000',@p2=NULL,@p3=NULL,@p4=2,@p5='karl.burndorfer@cssd.ab.ca',@p6='2023-08-15 15:07:01.2815319',@p22='EB482121-AAA0-480D-A4A2-9B441BF51B57',@p8=N'110155934',@p9='2004-09-16 00:00:00',@p10=1985,@p11=0,@p12='2023-09-05 00:00:00',@p13='2024-06-27 00:00:00',@p14='Matthew',@p15=12,@p16=1,@p17='Howery',@p18='James',@p19='7D0EF999-A48B-4759-9DB2-6293176515F0'
,@p20='D55AF390-4081-49B1-AE05-ECF93ED11EB5',@p21=154563

I put the id into the school query to confirm the ID was not there.
	select * from school s where s.id = 'D55AF390-4081-49B1-AE05-ECF93ED11EB5'
No rows were found.
Checked starbridge  mapping to see what was mapped to this destination identifier 'D55AF390-4081-49B1-AE05-ECF93ED11EB5'
	select *  from [dbo].[Mapping] m
        join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id] --and ssei.EntityTypeId = 'A2D5F496-0A1E-4989-8F98-E5F6F41577C4'-- where m.[State] <> 3
		join MappingEntityTransaction met on met.MappingId = m.Id
		join EntityTransaction et on et.Id = met.Id
		join EntityType ety on ety.Id =  ssei.EntityTypeId
		join Operation o on o.Id = et.Id
       where m.DestinationSystemEntityId like 'D55AF390-4081-49B1-AE05-ECF93ED11EB5' --order by [Timestamp] desc
	   --where ssei.SourceSystemEntityId like '0140' --'0000005C'
	   and ety.Name = 'School'
	   and ssei.SourceSystemId = '87A05A3A-89F1-4058-9B17-0EE388304E99'
	   order by [Timestamp] desc
That showed the associated source system identifier was school '0140' and not '0012'
--3136CC96-C2FD-4D39-B8F2-0BAE219E6624  is 140

update m
set DestinationSystemEntityId = '3136CC96-C2FD-4D39-B8F2-0BAE219E6624'
--select *
from mapping m
        join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
		join [dbo].[DestinationSystemEntityIdentifier] d on m.DestinationSystemEntityIdentifierId = d.Id
		where m.DestinationSystemEntityId = '3136CC96-C2FD-4D39-B8F2-0BAE219E6624'

		and m.SourceSystemEntityIdentityId = '89A01DAE-5362-4FCE-AB8E-525A17525B3C'


update d
set DestinationSystemEntityId ='3136CC96-C2FD-4D39-B8F2-0BAE219E6624'
--  select *
from mapping m
        join [dbo].[SourceSystemEntityIdentity] ssei ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
		join [dbo].[DestinationSystemEntityIdentifier] d on m.DestinationSystemEntityIdentifierId = d.Id
		where d.Id = '5373FD45-C0A3-4732-954F-721DCB103B61'
Ran the integration again and it picked up on the FK for the school in question.
____________________________________________________________________________________________________________________________________
Q2: Why are there "Not found in destination system: xx"  repeatedly found on Powerschool entity StudentContact.
Not sure yet what causes this. A fix is to set the mapping status to 3. Get Mapping Id of record causing the Not found. (Use Starbridge query for that)
DECLARE @RC int
DECLARE @mappingId uniqueidentifier
DECLARE @operationId uniqueidentifier
DECLARE @timeStamp datetimeoffset(7)
DECLARE @state int

set @mappingId = '4515BC6D-4EC0-49C6-BD10-E437B0212F31'
set @operationId = newid();
set @timeStamp = getdate();
-- TODO: Set parameter values here.

EXECUTE @RC = [dbo].[SetMappingState]
   @mappingId
  ,@operationId
  ,@timeStamp
  ,'3'
____________________________________________________________________________________________________________________________________

Q3: We have the Staff entity that uses multiple SharedEntityIdentiers:
    [SharedSourceSystemIdentifiers(
        typeof(SchoolObjectModel),
        typeof(PowerSchool.PowerSchool),
        typeof(HumanResources.HumanResources),
        typeof(Json.Json))]
Running the Poweschool Staff integration (Used only to allow FK checks for the SectionTeacher migrator to only insert if the staff exists.) caused 34 "Referred for manual deduplication:".
The  Staff.EmployeeId is the deduplication key and the starbridge database did not have any mappings to the Staff under the Powerschool Source system. It did have one for that staff for HRAd and SchoolObjectModel.
Jon suggested in a test env, to drop the Powerschool entry from the list of sharedSourceSystemIdentifiers. This worked and there were no manual deduplications picked up. I then added it back and it stayed working.
Jons concern was we could have a new Powerschool staff and it could show the same symtom. He determined there were some orphaned mappings that we underlying this issue and provided a script to clear them:

DECLARE @RC int
DECLARE @MappingId uniqueidentifier;
DECLARE @NotFoundEntities TABLE
(
	MappingId uniqueidentifier
)

insert into @NotFoundEntities ( MappingId )
select distinct m.Id -- select *
FROM [StarBridge-Stdnt].[dbo].[Mapping] m
              INNER JOIN [StarBridge-Stdnt].[dbo].[SourceSystemEntityIdentity] ssei
                     ON m.[SourceSystemEntityIdentityId] = ssei.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[EntityType] et ON ssei.[EntityTypeId] = et.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[ExternalSystem] es ON ssei.[SourceSystemId] = es.[Id]
              INNER JOIN [StarBridge-Stdnt].[dbo].[DestinationSystemEntityIdentifier] dsei
                     ON m.[DestinationSystemEntityIdentifierId] = dsei.[Id]
              LEFT OUTER JOIN [DDH].[dbo].[Staff] s_on_id
                     ON dsei.[DestinationSystemEntityId] = s_on_id.[Id]
              INNER JOIN [DDH].[dbo].[Staff] s_on_employeeId
                     ON ssei.[SourceSystemEntityId] = s_on_employeeId.[EmployeeId]
       WHERE et.[Name] = 'Staff' AND m.[State] <> 3
              AND s_on_id.[Id] IS NULL


DECLARE cur CURSOR FOR SELECT MappingId FROM @NotFoundEntities
DECLARE @operationId uniqueidentifier,
@timestamp datetime;

OPEN cur
FETCH NEXT FROM cur INTO @MappingId
WHILE @@FETCH_STATUS = 0 BEGIN
  SET @operationId = NEWID();
  SET @timestamp = GETDATE();
  PRINT convert(varchar(36), @MappingId)+ ', ' + convert(varchar(36), @operationId) + ', ' + convert(varchar(36), @timestamp)
  EXECUTE @RC = [dbo].[SetMappingState]    @MappingId, @operationId, @timestamp, 3
  FETCH NEXT FROM cur INTO @MappingId
END

CLOSE cur
DEALLOCATE cur

This cleared the mapings (State=3) and seems to have fixed the issue.
____________________________________________________________________________________________________________________________________

Q4: How can I debug an Ifeed:
Launch Starbridge from the console (as on prod server) and attach the debugger to the Visual Studio project using it.
You can also print out the loadEntities collection to a json file as:
using System.Text.Json;
        public IEntityCollection<Staff> LoadEntities()
        {
            using var context =
                this.schoolObjectModelContextFactory.CreateSchoolObjectModelContext();
            var x =
                context
                .Staff
                .Where(staff =>
                    staff.SourceKey == 1 &&
                    staff.DeletedTimestamp == null &&
                    (staff.DeactivatedDate == null ||
                     staff.DeactivatedDate > DateTime.Now))
                .ToArray()
                .Select(
                    staff => new Staff()
                    {
                        EmployeeId = staff.EmployeeId,
                    });

            var res = x;
            var stringOFJson = JsonSerializer.Serialize(res);
            File.WriteAllText(@"c:\temp\iFeedString.json", stringOFJson);

            return x.ToEntityCollection();
        }

 
