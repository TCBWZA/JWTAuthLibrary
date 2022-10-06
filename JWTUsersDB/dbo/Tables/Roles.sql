CREATE TABLE [dbo].[Roles] (
    [ID]     UNIQUEIDENTIFIER CONSTRAINT [DF_Role_ID] DEFAULT (newsequentialid()) NOT NULL,
    [Name]   NVARCHAR (50)    NOT NULL,
    [Active] BIT              CONSTRAINT [DF_Role_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Roles_Name]
    ON [dbo].[Roles]([Name] ASC);

