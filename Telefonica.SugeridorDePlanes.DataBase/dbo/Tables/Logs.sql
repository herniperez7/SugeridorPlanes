CREATE TABLE [dbo].[Logs] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [CreatedDate] DATETIME      NOT NULL,
    [Reference]   VARCHAR (100) NULL,
    [LogMessage]  VARCHAR (MAX) NULL,
    [ExtraData]   VARCHAR (MAX) NULL
);



