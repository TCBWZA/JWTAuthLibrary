namespace JWTAuthLibrary
{
    public interface IRoleValidationService
    {
        Task<bool> ValidateRolesAsync(UserInfo validUser);
    }
}
