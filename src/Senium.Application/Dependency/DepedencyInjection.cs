using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senium.Infra.Data.Dependency;

namespace Senium.Application.Dependency;

public static class DependecyInjection
{
    public static void ConfigureApplication(this IServiceCollection service, IConfiguration configuration)
    {

        service.ConfigureDbContext(configuration);

    }
}