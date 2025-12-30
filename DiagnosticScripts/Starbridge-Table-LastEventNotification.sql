CREATE TABLE [dbo].[LastEventNotification](
[Id] [uniqueidentifier] NOT NULL,
[ApplicationName] [varchar](255) NOT NULL,
[NotificationType] [int] NULL,
[LastNotificationDateTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_LastEventNotification] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]