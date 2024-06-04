using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Senium.API.Configuration;
using Senium.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .Configure<RequestLocalizationOptions>(o =>
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        o.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
        o.SupportedCultures = supportedCultures;
        o.SupportedUICultures = supportedCultures;
    });

builder
    .Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder
    .Services
    .SetupSettings(builder.Configuration);

builder
    .Services
    .AddResponseCompression(options => { options.EnableForHttps = true; });

builder
    .Services
    .AddApiConfiguration();

builder.Services.ConfigureApplication(builder.Configuration);
builder.Services.AddDependencyServices();

builder
    .Services.AddVersioning();

builder
    .Services
    .AddSwagger();

builder
    .Services
    .AddAuthenticationConfig(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseApiConfiguration(app.Services, app.Environment);

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthenticationConfig();

app.UseStaticFileConfiguration(builder.Configuration);

app.UseStaticFileConfiguration(app.Configuration);

app.MapControllers();

app.Run();