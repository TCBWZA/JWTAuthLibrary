CREATE TABLE [dbo].[Users] (
    [ID]           UNIQUEIDENTIFIER CONSTRAINT [DF_User_ID] DEFAULT (newsequentialid()) NOT NULL,
    [LoginName]    NVARCHAR (50)    NOT NULL,
    [Firstname]    NVARCHAR (50)    NOT NULL,
    [Lastname]     NVARCHAR (50)    NOT NULL,
    [PasswordHash] NVARCHAR (200)   NOT NULL,
    [Active]       BIT              CONSTRAINT [DF_User_Active] DEFAULT ((1)) NOT NULL,
    [Email]        NVARCHAR (250)   NULL,
    CONSTRAINT [PK_User] PRIMARY KEY NONCLUSTERED ([ID] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_Users_LoginName]
    ON [dbo].[Users]([LoginName] ASC);

