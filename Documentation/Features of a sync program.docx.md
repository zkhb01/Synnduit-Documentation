# Features of a sync program

> Extracted from `Features of a sync program.docx`

Synnduit supported features

- Data integrity setup and validation (rejects on defined foreign keys relationships)
- Data change history down to when an individual field changes
- Change Threshold detection – allows sync process to terminate if say source sync causes more deletions than a defined threshold %
- Dashboard to view sync/change history
- Allows for any source application to provide data under a standard API to feed in the source data.
- Allows for any destination type application to receive data under a standard API to update the destination data.
- Configurable destination Sync and Garbage collection options.
- Sync program is interruptible, as in, the server it’s running on crashes. No data should be lost, and sync can pick up where it left off
- Definable business keys for deduplication.
- Custom overrides for common internal methods.
Needed features

- Performance metrics for sync transaction
- Split Destination sync time from starbridge sync time
- Based on destination
