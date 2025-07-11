USE [Borrow]
GO
/****** Object:  Table [dbo].[Borrows]    Script Date: 1/14/2025 6:31:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Borrows](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookId] [int] NOT NULL,
	[UserId] [nvarchar](max) NOT NULL,
	[BorrowDate] [datetime2](7) NOT NULL,
	[ReturnDate] [datetime2](7) NULL,
	[BookName] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](max) NOT NULL,
	[IsTrue] [bit] NOT NULL,
 CONSTRAINT [PK_Borrows] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Borrows] ADD  DEFAULT (N'') FOR [BookName]
GO
ALTER TABLE [dbo].[Borrows] ADD  DEFAULT (N'') FOR [FullName]
GO
ALTER TABLE [dbo].[Borrows] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsTrue]
GO
