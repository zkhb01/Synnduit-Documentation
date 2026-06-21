# Viewing serialized values for an entity in StarBridge

> Extracted from `Viewing serialized values for an entity in StarBridge.docx`

The serialized values for an entity in StarBridge

Scenario: StarBridge picked up a new source record, but there is an existing record in the destination. These are logged as “Duplicate detected changes merged” or “Duplicate detected no changes merged”

If running the value change script only shows you a partial set of fields (only changes here)  and you want to know the original values

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

WHERE [TimeStamp] >=  '2000-07-30T10:36:46.2079593-06:00'

AND EntityType = 'Student'

and SourceSystemEntityId like '1111545'

ORDER BY [TimeStamp] DESC


| DestinationSystem | EntityType | DestinationSystemEntityId | SourceSystem | SourceSystemEntityId | TimeStamp | ValueName | PreviousValue | NewValue |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| School Object Model | Student | 7421a3bd-77d9-4c63-882d-fde53d4ad652 | Power School | 1111545 | 2024-09-16 10:47:45.1514995 -06:00 | FirstName | Ifa | Iva |
| School Object Model | Student | 7421a3bd-77d9-4c63-882d-fde53d4ad652 | Power School | 1111545 | 2024-09-16 10:47:45.1514995 -06:00 | HomeRoom | NULL | 5CAR |

Here is the query to link SourceSystemEntityId’s to its  serialized data:

select se.[Data],ssei.SourceSystemEntityId, *

from [dbo].[SourceSystemEntityIdentity] ssei

join [dbo].[Mapping] m on m.SourceSystemEntityIdentityId = ssei.Id

join MappingEntityTransaction met on met.MappingId = m.Id

join EntityTransaction et on et.Id = met.Id

join [dbo].[OperationSourceSystemEntity] oss on oss.Id = met.Id

join [dbo].[SerializedEntity] se on se.Id = oss.SerializedEntityId

- join EntityType ety on ety.Id =  ssei.EntityTypeId
- join Operation o on o.Id = et.Id
where  ssei.SourceSystemEntityId like '1111545'

Run the query to get the  dbo.SerializedEntity.Data contents.

0x7B2253747564656E744E756D626572223A313131313534352C2244434944223A3336353831312C22416C626572746153747564656E744E756D626572223A22333135373230363536222C225363686F6F6C4964223A2239306132663233342D326636312D343238622D393464332D643861363631336431366235222C2246697273744E616D65223A22497661222C224C6173744E616D65223A22417373656661222C22426972746844617465223A22323031342D30322D31385430303A30303A3030222C22456E74727944617465223A22323032342D30392D31315430303A30303A3030222C224578697444617465223A22323032352D30362D32365430303A30303A3030222C22456E726F6C6C6D656E74537461747573223A302C224772616465223A352C22526567697374726174696F6E547970654964223A2237643065663939392D613438622D343735392D396462322D363239333137363531356630222C224973496E646570656E64656E7453747564656E74223A66616C73652C224E6578745363686F6F6C223A2230222C224C6567616C46697273744E616D65223A22496661222C224C6567616C4C6173744E616D65223A22417373656661222C224C6567616C4D6964646C654E616D65223A22576F6E64776573656E222C2247656E646572223A2246656D616C65222C22486F6D65526F6F6D223A2235434152227D

Go to an online decoder site like: https://cryptii.com/pipes/binary-decoder

Set the decoder to use Hexadecimal to Byte.  Paste in the contents of the data field and strip off the 0x

Then you can use the plugin JsTools in Notepad++ to format and sort the Json for easy reading:

{

"AlbertaStudentNumber": "315720656",

"BirthDate": "2014-02-18T00:00:00",

"DCID": 365811,

"EnrollmentStatus": 0,

"EntryDate": "2024-09-11T00:00:00",

"ExitDate": "2025-06-26T00:00:00",

"FirstName": "Iva",

"Gender": "Female",

"Grade": 5,

"HomeRoom": "5CAR",

"IsIndependentStudent": false,

"LastName": "Assefa",

"LegalFirstName": "Ifa",

"LegalLastName": "Assefa",

"LegalMiddleName": "Wondwesen",

"NextSchool": "0",

"RegistrationTypeId": "7d0ef999-a48b-4759-9db2-6293176515f0",

"SchoolId": "90a2f234-2f61-428b-94d3-d8a6613d16b5",

"StudentNumber": 1111545

}
