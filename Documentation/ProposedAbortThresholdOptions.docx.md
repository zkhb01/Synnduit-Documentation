# ProposedAbortThresholdOptions

> Extracted from `ProposedAbortThresholdOptions.docx`

This is the initial implementation that starts to incorporate the requested limit settings to prevent data loss.

Jon here has implemented an option to abort a run once a given segment reaches the specified number of exceptions, along with an overall run limit for the entire run.

Using the same design, Jon will look at the more needed deletion issue, where a failed ifeed pulled no records, which then tells the garbage collection to match and then delete your entire destination entity set. Here, we want to compare the existing destination record count and not allow execution if the iFeed source count is below a specified percentage of the destination.

IE: Destination entity is Student with a count of 65k. Ifeed goofs up and returns 0 records.

Set a SegmentMaxDeletionPercent parm to 10%. With this setting, Synnduit would compare a 0 count to 65K, which is more than 10%, and abort the process accordingly.

Set a RunMaxDeletionPercent parm to 50%. With this setting, Synnduit would compare all segments in the run, which is more than 50%, and abort the process accordingly.

Setting the parm to 100% would allow the full deletion of the destination entity. If the parm is omitted, it would be assumed 100%
