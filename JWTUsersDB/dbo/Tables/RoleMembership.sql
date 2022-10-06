CREATE TABLE [dbo].[RoleMembership] (
    [ID]     BIGINT           NOT NULL,
    [User]   UNIQUEIDENTIFIER NOT NULL,
    [Role]   UNIQUEIDENTIFIER NOT NULL,
    [Active] BIT              CONSTRAINT [DF_RoleMembership_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [FK_RoleMembership_Role] FOREIGN KEY ([Role]) REFERENCES [dbo].[Roles] ([ID]),
    CONSTRAINT [FK_RoleMembership_Users] FOREIGN KEY ([User]) REFERENCES [dbo].[Users] ([ID])
);

