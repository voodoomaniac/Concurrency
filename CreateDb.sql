USE [master] 
GO 
CREATE DATABASE [Concurrency];
GO 
USE [Concurrency] 
GO 
CREATE TABLE [Authors]( 
[Id] [int] PRIMARY KEY IDENTITY NOT NULL, 
[FirstName] [nvarchar](50) NOT NULL, 
[LastName] [nvarchar](50) NOT NULL);
GO

CREATE TABLE [Books]( 
[Id] [int] PRIMARY KEY IDENTITY NOT NULL, 
[Name] [nvarchar](50) NOT NULL, 
[PublishDate] [datetime] NOT NULL,
[IsLocked] [bit] NULL,
[LockedBy] [nvarchar](50) NULL);
GO