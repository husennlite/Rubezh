USE [master]
GO
/****** Object:  Database [Firesec]    Script Date: 08/16/2011 22:28:44 ******/
CREATE DATABASE [Firesec] ON  PRIMARY 
( NAME = N'Persons', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\Persons.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Persons_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\Persons_log.ldf' , SIZE = 47616KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 COLLATE Cyrillic_General_CI_AS
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'Firesec', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Firesec].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Firesec] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Firesec] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Firesec] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Firesec] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Firesec] SET ARITHABORT OFF 
GO
ALTER DATABASE [Firesec] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Firesec] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Firesec] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Firesec] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Firesec] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Firesec] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Firesec] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Firesec] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Firesec] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Firesec] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Firesec] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Firesec] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Firesec] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Firesec] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Firesec] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Firesec] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Firesec] SET  READ_WRITE 
GO
ALTER DATABASE [Firesec] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Firesec] SET  MULTI_USER 
GO
ALTER DATABASE [Firesec] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Firesec] SET DB_CHAINING OFF 






USE [Firesec]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 08/16/2011 22:35:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) COLLATE Cyrillic_General_CI_AS NULL,
	[Age] [int] NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Person] FOREIGN KEY([Id])
REFERENCES [dbo].[Person] ([Id])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Person]




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
