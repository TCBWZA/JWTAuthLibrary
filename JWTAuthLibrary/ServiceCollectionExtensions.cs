using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static JWTAuthOptions _jwtAuthOptionsCache;

        public static IServiceCollection AddJWTAuth(
          this IServiceCollection services,
          Action<JWTAuthOptions> configure = null)
        {
            OptionsServiceCollectionExtensions.AddOptions<JWTAuthOptions>(services).Configure<IConfiguration, ILogger<JWTAuthOptions>>((Action<JWTAuthOptions, IConfiguration, ILogger<JWTAuthOptions>>)((options, configuration, logger) =>
            {
                ConfigurationBinder.Bind((IConfiguration)configuration.GetSection(JWTAuthOptions.SectionName), (object)options);
                Action<JWTAuthOptions> action = configure;
                if (action != null)
                    action(options);
                if (string.IsNullOrEmpty(options.IssuerSigningSecret))
                {
                    if (logger != null)
                        LoggerExtensions.LogWarning((ILogger)logger, message: "Issuer signing secret is not specified. Using random string as secrets temperory. Please specify issuer signing secret.", Array.Empty<object>());
                    options.IssuerSigningSecret = Guid.NewGuid().ToString();
                }
                ServiceCollectionExtensions._jwtAuthOptionsCache = options;
            }));
            JwtBearerExtensions.AddJwtBearer(AuthenticationServiceCollectionExtensions.AddAuthentication(services, (Action<AuthenticationOptions>)(opt =>
            {
                opt.DefaultAuthenticateScheme = "Bearer";
                opt.DefaultChallengeScheme = "Bearer";
            })), (Action<JwtBearerOptions>)(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = (SecurityKey)new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ServiceCollectionExtensions._jwtAuthOptionsCache.IssuerSigningSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = ServiceCollectionExtensions._jwtAuthOptionsCache.Issuer,
                    ValidateAudience = true,
                    ValidAudience = ServiceCollectionExtensions._jwtAuthOptionsCache.Audience,
                    NameClaimType = ServiceCollectionExtensions._jwtAuthOptionsCache.NameClaimType
                };
                if (!string.Equals(ServiceCollectionExtensions._jwtAuthOptionsCache.RoleClaimType, "role", StringComparison.OrdinalIgnoreCase))
                    opt.TokenValidationParameters.RoleClaimType = ServiceCollectionExtensions._jwtAuthOptionsCache.RoleClaimType;
                if (ServiceCollectionExtensions._jwtAuthOptionsCache.OnJWTAuthenticationMessageReceived == null)
                    return;
                opt.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = ServiceCollectionExtensions._jwtAuthOptionsCache.OnJWTAuthenticationMessageReceived
                };
            }));
            return services;
        }
    }
}
