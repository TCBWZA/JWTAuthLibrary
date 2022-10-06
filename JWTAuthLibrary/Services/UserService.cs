using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using JWTAuthLibrary;

namespace JWTAuthLibrary
{
    public class UserService
    {

        public async Task<Users> AddUserAsync(UserRegisterModel NewUser, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(NewUser.LoginName))
            {
                throw new System.ArgumentException($"'Login Name' cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(NewUser.Firstname))
            {
                throw new System.ArgumentException($"'Firstname' cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(NewUser.Lastname))
            {
                throw new System.ArgumentException($"'Lastname' cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(NewUser.Password))
            {
                throw new System.ArgumentException($"'Password' cannot be null or empty.");
            }


            Users_TSQL dbusers = new Users_TSQL("");
            var AddedUser = await dbusers.AddAsync(NewUser);

            return AddedUser; // New user with id.
        }

        public async IAsyncEnumerable<Users> ListUsersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Users_TSQL dbusers = new Users_TSQL("");
            foreach (Users user in dbusers.GetAllActiveAsync().Result.ToList())
            {
                RedactSensitiveData(user);
                yield return user;
            }
        }

        public Task<Users> GetUserByNameAsync(string userName)
        {
            Users_TSQL dbusers = new Users_TSQL("");
            return dbusers.GetByLoginNameAsync(userName);
        }
        public async Task<Users> GetValidUserAsync(string userName, string clearTextPassword, CancellationToken cancellationToken = default)
        {
            UserLoginModel inputuser = new UserLoginModel();
            inputuser.LoginName = userName;
            inputuser.Password = clearTextPassword;
            Users target = await GetUserByNameAsync(userName).ConfigureAwait(false);
            if (inputuser.PasswordHash.SequenceEqual(target.PasswordHash))
            {
                return target;
            }
            return null;
        }

        public Task ChangePasswordAsync(Users user, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Users GetUserByIdAsync(Guid userId)
        {
            Users_TSQL dbusers = new Users_TSQL("");
            return dbusers.GetByGuidAsync(userId).Result;
        }

        public List<String> GetRoles(Users user)
        {
            var rolelist = new List<String>();
            foreach (Roles role in user.roles)
            {
                rolelist.Add(role.Name);
            }
            return rolelist;
        }

        private static Users RedactSensitiveData(Users user)
        {
            // Redact the password hash
            user.PasswordHash = null;
            return user;
        }
    }
}