using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Contracts.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Howestprime.Movies.Infrastructure.Authorization;

namespace Howestprime.Movies.Main.Modules.Security;

public static class SecurityModule
{
    public static IServiceCollection AddSecurityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthorizationOptions>(
            configuration.GetSection(AuthorizationOptions.SectionName));

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}