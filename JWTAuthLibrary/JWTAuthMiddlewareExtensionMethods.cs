using JWTAuthLibrary;
using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JWTAuthMiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseJWTAuth(this IApplicationBuilder builder)
        {
            return builder
                    .UseMiddleware<TokenMiddleware>()
                    .UseAuthentication()
                    .UseAuthorization();
        }
    }
}