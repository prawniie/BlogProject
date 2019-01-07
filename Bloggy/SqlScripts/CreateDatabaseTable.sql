USE [Blogposts]
GO

CREATE TABLE [dbo].[BlogpostNEW2](
	[BlogpostId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Author] [nvarchar](50) NULL,
	)