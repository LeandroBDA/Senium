using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;
using Senium.Application.Contracts.Services;
using Senium.Application.Notifications;
using Senium.Application.Services;
using Senium.Core.Settings;
using Senium.Domain.Entities;
using Senium.Infra.Data.Configuration;

namespace Senium.Application.Configuration;

public static class DependecyInjection
{
    
    public static void SetupSettings(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        service.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    }
    
    public static void ConfigureApplication(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        service.ConfigureDbContext(configuration);
        service.AddDependencyRepositories();

    }
    
    public static void AddDependencyServices(this IServiceCollection service)
    {
        service.AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>();
        
        service.AddScoped<INotificator, Notificator>();

        service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        service
            .AddScoped<IUsuarioAuthService, UsuarioAuthService>()
            .AddScoped<IUsuarioService, UsuarioService>();
    }
    
}