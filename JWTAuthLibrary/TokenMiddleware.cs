using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JWTAuthLibrary
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next) => this._next = next ?? throw new ArgumentNullException(nameof(next));

        public async Task Invoke(
          HttpContext httpContext,
          IUserValidationService userValidationService,
          IServiceProvider serviceProvider,
          IOptions<JWTAuthOptions> options)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            if (userValidationService == null)
                throw new InvalidOperationException("No IUserValidationService registered.");
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            JWTAuthOptions jwtAuthOptions = options?.Value ?? new JWTAuthOptions();
            PathString path = httpContext.Request.Path;
            StreamReader sr;
            if (!this.IsTokenPath(httpContext, jwtAuthOptions.TokenPath))
            {
                await this._next.Invoke(httpContext);
                jwtAuthOptions = (JWTAuthOptions)null;
                sr = (StreamReader)null;
            }
            else
            {
                sr = new StreamReader(httpContext.Request.Body);
                try
                {
                    string requestStringContent = await ((TextReader)sr).ReadToEndAsync().ConfigureAwait(false);
                    int num = 0;
                    try
                    {
                        UserInfo validUser = await userValidationService.ValidateUserAsync(requestStringContent).ConfigureAwait(false);
                        if (validUser == null)
                        {
                            this.WriteUnauthorized(httpContext);
                            jwtAuthOptions = (JWTAuthOptions)null;
                            sr = (StreamReader)null;
                            return;
                        }
                        if (!await this.ValidateRoleInfo(validUser, serviceProvider))
                        {
                            this.WriteUnauthorized(httpContext);
                            jwtAuthOptions = (JWTAuthOptions)null;
                            sr = (StreamReader)null;
                            return;
                        }
                        string str = this.BuildAccessToken(jwtAuthOptions, validUser);
                        httpContext.Response.StatusCode = 200;
                        await HttpResponseWritingExtensions.WriteAsync(httpContext.Response, JsonSerializer.Serialize<TokenResponseBody>(new TokenResponseBody()
                        {
                            Token = str
                        }, new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        }), new CancellationToken()).ConfigureAwait(false);
                        validUser = (UserInfo)null;
                    }
                    catch (InvalidCastException ex)
                    {
                        num = 1;
                    }
                    if (num != 1)
                    {
                        jwtAuthOptions = (JWTAuthOptions)null;
                        sr = (StreamReader)null;
                    }
                    else
                    {
                        await HttpResponseWritingExtensions.WriteAsync(httpContext.Response, "Parsing Login info failed. Is it in valid json format?", new CancellationToken()).ConfigureAwait(false);
                        jwtAuthOptions = (JWTAuthOptions)null;
                        sr = (StreamReader)null;
                    }
                }
                finally
                {
                    ((IDisposable)sr)?.Dispose();
                }
            }
        }

        private void WriteUnauthorized(HttpContext httpContext) => httpContext.Response.StatusCode = 401;

        private Task<bool> ValidateRoleInfo(UserInfo validUser, IServiceProvider serviceProvider)
        {
            IRoleValidationService service = ServiceProviderServiceExtensions.GetService<IRoleValidationService>(serviceProvider);
            if (service == null)
                Task.FromResult<bool>(true);
            return service.ValidateRolesAsync(validUser);
        }

        private string BuildAccessToken(JWTAuthOptions jwtAuthOptions, UserInfo userInfo)
        {
            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            byte[] bytes = Encoding.UTF8.GetBytes(jwtAuthOptions.IssuerSigningSecret);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(jwtAuthOptions.Issuer, jwtAuthOptions.Audience, this.GetClaims(userInfo, jwtAuthOptions), new DateTime?(DateTime.UtcNow), new DateTime?(DateTime.UtcNow.Add(jwtAuthOptions.TokenLifeSpan)), new SigningCredentials((SecurityKey)new SymmetricSecurityKey(bytes), "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"));
            return ((SecurityTokenHandler)securityTokenHandler).WriteToken((SecurityToken)jwtSecurityToken);
        }

        private IEnumerable<Claim> GetClaims(UserInfo user, JWTAuthOptions options)
        {
            yield return new Claim(options.NameClaimType, user.Name);
            foreach (string str in user.Roles.NullAsEmpty<string>())
                yield return new Claim(options.RoleClaimType, str);
            if (user.AdditionalClaims.NullAsEmpty<Claim>().Any<Claim>())
            {
                foreach (Claim additionalClaim in user.AdditionalClaims)
                    yield return additionalClaim;
            }
        }

        private bool IsTokenPath(HttpContext httpContext, PathString configuredTokenPath)
        {
            if (httpContext.Request.Method == HttpMethods.Post)
            {
                PathString path = httpContext.Request.Path;
                if (path.HasValue)
                    return PathString.Equals(httpContext.Request.Path, configuredTokenPath);
            }
            return false;
        }
    }
}
