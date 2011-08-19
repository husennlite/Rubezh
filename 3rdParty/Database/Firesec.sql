USE [Firesec]
GO
/****** Object:  Table [dbo].[Journal]    Script Date: 08/16/2011 22:37:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Journal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceTime] [datetime] NULL,
	[SystemTime] [datetime] NULL,
	[ZoneName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[Description] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[DeviceName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[PanelName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[DeviceDatabaseId] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[PanelDatabaseId] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[UserName] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[StateType] [int] NULL,
 CONSTRAINT [PK_Journal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
