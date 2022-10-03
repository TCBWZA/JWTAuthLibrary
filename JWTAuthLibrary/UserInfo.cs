using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JWTAuthLibrary
{
    public class UserInfo
    {
        private object _boxedLogin;

        public UserInfo(string userName, object login)
        {
            this.Name = !string.IsNullOrEmpty(userName) ? userName : throw new ArgumentException("'userName' cannot be null or empty.", nameof(userName));
            this._boxedLogin = login ?? throw new ArgumentNullException(nameof(login));
        }

        public string Name { get; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<Claim> AdditionalClaims { get; private set; } = Enumerable.Empty<Claim>();

        public void AppendAdditionalClaims(params Claim[] claims) => this.AdditionalClaims = this.AdditionalClaims.Union<Claim>((IEnumerable<Claim>)claims);

        public bool TryUnboxLogin<T>(out T login) where T : class
        {
            login = default(T);
            if (!(this._boxedLogin is T boxedLogin))
                return false;
            login = boxedLogin;
            return true;
        }
    }
}