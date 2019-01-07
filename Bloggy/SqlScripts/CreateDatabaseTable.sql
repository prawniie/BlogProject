USE [Blogposts]
GO

CREATE TABLE [dbo].[Blogpost](
	[BlogpostId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Description] [nvarchar](50) NULL,
	[Author] [nvarchar](50) NULL,
	[Created] [datetime] NULL,
	[Updated] [datetime] NULL,
	)

USE [Blogposts]
GO
CREATE TABLE [dbo].[BlogpostTag](
	[BlogpostId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
	)

CREATE TABLE [dbo].[Comment](
	[CommentId] [int] IDENTITY(1,1) NOT NULL,
	[BlogpostId] [int] NOT NULL,
	[CommentText] [nvarchar](50) NULL,
	[DateTime] [datetime] NULL,
	[Author] [nvarchar](50) NULL,
	)

CREATE TABLE [dbo].[Tag](
	[TagId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	)