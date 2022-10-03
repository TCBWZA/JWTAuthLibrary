using JWTAuthLibrary;
using Microsoft.AspNetCore.Builder;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JWTAuthMiddlewareExtensionMethods
    {
        public static IApplicationBuilder UseJWTAuth(this IApplicationBuilder builder) => AuthorizationAppBuilderExtensions.UseAuthorization(AuthAppBuilderExtensions.UseAuthentication(UseMiddlewareExtensions.UseMiddleware<TokenMiddleware>(builder, Array.Empty<object>())));
    }
}