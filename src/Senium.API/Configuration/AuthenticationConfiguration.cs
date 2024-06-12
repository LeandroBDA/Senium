using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Senium.Core.Enums;
using Senium.Core.Settings;
using StackExchange.Redis;

namespace Senium.API.Configuration;

public static class AuthenticationConfiguration
{
    public static void AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var appSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<JwtSettings>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.IncludeErrorDetails = true; // <- great for debugging
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings?.Emissor ?? string.Empty,
                    ValidAudiences = appSettings?.Audiences()
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(ETipoUsuario.AdministradorComum.ToString(), builder =>
            {
                builder
                    .RequireAuthenticatedUser()
                    .RequireClaim("TipoUsuario", ETipoUsuario.AdministradorComum.ToString());
            });
            
            options.AddPolicy(ETipoUsuario.AdministradorGeral.ToString(), builder =>
            {
                builder
                    .RequireAuthenticatedUser()
                    .RequireClaim("TipoUsuario", ETipoUsuario.AdministradorGeral.ToString());
            });

            options.AddPolicy(ETipoUsuario.Comum.ToString(), builder =>
            {
                builder
                    .RequireAuthenticatedUser()
                    .RequireClaim("TipoUsuario", ETipoUsuario.Comum.ToString());
            });
        });

        var redisConnection = configuration.GetConnectionString("RedisConnection");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnection),
                    $"Senium-{environment.EnvironmentName}-DataProtection-Keys-");
        }
        else
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(appSettings.CaminhoKeys));
        }

        services
            .AddJwksManager()
            .UseJwtValidation();

        services.AddMemoryCache();
        services.AddHttpContextAccessor();
    }

    public static void UseAuthenticationConfig(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}