CREATE VIEW dbo.RoleMemberships
AS
SELECT        dbo.[Users].Firstname, dbo.[Users].Lastname, dbo.[Users].LoginName, dbo.[Users].Active AS UserActive, dbo.[Users].Email, 
               dbo.Roles.Name AS RoleName, dbo.Roles.Active AS RoleActive, 
                         dbo.RoleMembership.Active AS RoleMembershipActive
FROM            dbo.Roles INNER JOIN
                         dbo.RoleMembership ON dbo.Roles.ID = dbo.RoleMembership.Role INNER JOIN
                         dbo.[Users] ON dbo.RoleMembership.[User] = dbo.[Users].ID

GO
