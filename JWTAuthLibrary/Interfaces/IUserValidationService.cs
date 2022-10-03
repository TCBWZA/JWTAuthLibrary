namespace JWTAuthLibrary
{
    public interface IUserValidationService
    {
        Task<UserInfo> ValidateUserAsync(string requestStringContent);
    }
}
