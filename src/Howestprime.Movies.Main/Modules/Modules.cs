using Howestprime.Movies.Main.Modules.Application;
using Howestprime.Movies.Main.Modules.Messaging.DomainEvents;
using Howestprime.Movies.Main.Modules.Messaging.IntegrationEvents;
using Howestprime.Movies.Main.Modules.Persistence.EntityFramework;
using Howestprime.Movies.Main.Modules.Security;
using Howestprime.Movies.Main.Modules.WebApi;

namespace Howestprime.Movies.Main.Modules;

public static class ModuleExtensions
{
    public static IServiceCollection AddModules(
        this IServiceCollection services, 
        IConfiguration configuration
    )
    {
        return services
            .AddApplicationModule(configuration)
            .AddSecurityModule(configuration)
            .AddDomainEventModule(configuration)
            .AddEFCoreModule(configuration)
            .AddMessagingModule(configuration)
            .AddWebApiModule(configuration);
    }

    public static async Task<WebApplication> UseModules(this WebApplication app)
    {
        await app.UseEFCoreModule();
        return app.UseWebApiModule();
    }
}