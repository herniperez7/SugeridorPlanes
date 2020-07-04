CREATE TABLE [dbo].[EquipoPropuestas] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CODIGO_EQUIPO] VARCHAR (250) NOT NULL,
    [IdPropuesta]   INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([IdPropuesta]) REFERENCES [dbo].[Propuestas] ([Id])
);

