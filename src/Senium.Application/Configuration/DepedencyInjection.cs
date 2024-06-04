using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ScottBrady91.AspNetCore.Identity;
using Senium.Application.Contracts.Services;
using Senium.Application.Notifications;
using Senium.Application.Services;
using Senium.Core.Enums;
using Senium.Core.Extensions;
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
        service.Configure<UploadSettings>(configuration.GetSection("UploadSettings"));
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
            .AddScoped<IUsuarioService, UsuarioService>()
            .AddScoped<IEmpresaService, EmpresaService>()
            .AddScoped<ICurriculoService, CurriculoService>()
            .AddScoped<IExperienciaService, ExperienciaService>()
            .AddScoped<IFileService, FileService>();
    }
    
    public static void UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
    {
        var uploadSettings = configuration.GetSection("UploadSettings");
        var publicBasePath = uploadSettings.GetValue<string>("PublicBasePath");
        var privateBasePath = uploadSettings.GetValue<string>("PrivateBasePath");
    
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(publicBasePath),
            RequestPath = $"/{EPathAccess.Public.ToDescriptionString()}"
        });
    
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(privateBasePath),
            RequestPath = $"/{EPathAccess.Private.ToDescriptionString()}",
            OnPrepareResponse = ctx =>
            {
                // respond HTTP 401 Unauthorized.
                ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ctx.Context.Response.ContentLength = 0;
                ctx.Context.Response.Body = Stream.Null;
                ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
            }
        });
    }
    
}