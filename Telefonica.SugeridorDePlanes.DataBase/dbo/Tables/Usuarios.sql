CREATE DATABASE [Telefonica.SugeridorDePlanes1]

USE [Telefonica.SugeridorDePlanes1]
GO

/** Object:  Table [dbo].[Usuarios]    Script Date: 06/07/2020 0:23:58 **/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NombreCompleto] [varchar](250) NOT NULL,
	[Email] [varchar](250) NOT NULL,
	[Rol] [varchar](250) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO