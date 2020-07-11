CREATE TABLE [dbo].[Propuestas] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Documento]     VARCHAR (250) NOT NULL,
    [Payback]       FLOAT (53)    NULL,
    [PagoEquipos]   FLOAT (53)    NULL,
    [Subsidio]      FLOAT (53)    NULL,
    [Estado]        NVARCHAR (10) NULL,
    [IdUsuario]     INT           NULL,
    [FechaCreacion] DATETIME      NULL,
    [Activa]        BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



