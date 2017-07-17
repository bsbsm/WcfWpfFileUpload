CREATE TABLE [dbo].[FileRows] (
    [Id]     INT             NOT NULL IDENTITY,
    [Number] INT             NOT NULL,
    [Text]   NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

