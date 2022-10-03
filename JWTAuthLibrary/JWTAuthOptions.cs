using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public class JWTAuthOptions
    {
        public static string SectionName = "JWTAuthLib";

        public string IssuerSigningSecret { get; set; }

        public string Issuer { get; set; } = "JWTAuthIssuer";

        public string Audience { get; set; } = "JWTAuthAudience";

        public string RoleClaimType { get; set; } = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public string NameClaimType { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        public TimeSpan TokenLifeSpan { get; set; } = TimeSpan.FromHours(4.0);

        public PathString TokenPath { get; set; } = new PathString("/token");

        public Func<MessageReceivedContext, Task> OnJWTAuthenticationMessageReceived { get; set; }
    }
}