using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTAuthLibrary
{
    public abstract class UserServiceBase<T> : IUserValidationService, IRoleValidationService
      where T : class
    {
        public async Task<UserInfo> ValidateUserAsync(string requestStringContent)
        {
            UserInfo userInfo;
            try
            {
                userInfo = await this.IsValidUserAsync(await this.DeserializeUserLoginAsync(requestStringContent).ConfigureAwait(false)).ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                throw new InvalidCastException();
            }
            return userInfo;
        }

        public virtual Task<bool> ValidateRolesAsync(UserInfo validUser) => Task.FromResult<bool>(true);

        protected abstract Task<UserInfo> IsValidUserAsync(T login);

        protected virtual Task<T> DeserializeUserLoginAsync(string jsonText) => Task.FromResult<T>(JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }));
    }
}