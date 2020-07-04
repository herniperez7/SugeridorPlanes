CREATE TABLE [dbo].[LineaPropuestas] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Plan]        VARCHAR (250) NOT NULL,
    [NumeroLinea] VARCHAR (250) NULL,
    [IdPropuesta] INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([IdPropuesta]) REFERENCES [dbo].[Propuestas] ([Id])
);

