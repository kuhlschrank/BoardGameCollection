USE [BoardGameCollection]
GO
/****** Object:  Table [dbo].[BoardGame]    Script Date: 26.12.2019 20:43:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BoardGame](
	[Id] [int] NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[ThumbnailUri] [nvarchar](2043) NULL,
	[ImageUri] [nvarchar](2043) NULL,
	[MinPlayers] [int] NULL,
	[MaxPlayers] [int] NULL,
	[YearPublished] [int] NULL,
	[BestPlayerNumber] [int] NULL,
	[AverageRating] [float] NULL,
	[LastUpdate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_BoardGame] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Expansion]    Script Date: 26.12.2019 20:43:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Expansion](
	[BoardGameId] [int] NOT NULL,
	[ExpansionId] [int] NOT NULL,
 CONSTRAINT [PK_Expansion] PRIMARY KEY CLUSTERED 
(
	[BoardGameId] ASC,
	[ExpansionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Unknown]    Script Date: 26.12.2019 20:43:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unknown](
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_Unknown] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Expansion]  WITH CHECK ADD  CONSTRAINT [FK_Expansion_BoardGame] FOREIGN KEY([BoardGameId])
REFERENCES [dbo].[BoardGame] ([Id])
GO
ALTER TABLE [dbo].[Expansion] CHECK CONSTRAINT [FK_Expansion_BoardGame]
GO
