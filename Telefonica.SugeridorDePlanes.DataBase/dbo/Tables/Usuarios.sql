CREATE TABLE [dbo].[Usuarios] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [NombreCompleto] VARCHAR (250) NOT NULL,
    [Email]          VARCHAR (250) NOT NULL,
    [Rol]            VARCHAR (250) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

