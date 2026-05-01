using Aornis;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;

namespace Howestprime.Movies.Main.Modules.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplicationModule(
        this IServiceCollection services,
        IConfiguration _configuration
    )
    {
        // Register command use cases
        services.AddScoped<IUseCase<RegisterMovieInput, Guid>, RegisterMovie>();
        services.AddScoped<IUseCase<ChangeMovieDetailsInput>, ChangeMovieDetails>();
        services.AddScoped<IUseCase<ScheduleMovieEventInput, Guid>, ScheduleMovieEvent>();
        services.AddScoped<IUseCase<BookMovieEventInput, Guid>, BookMovieEvent>();
        services.AddScoped<IUseCase<CloseBookingInput>, CloseBooking>();
        // Register query use cases
        services.AddScoped<IUseCase<SearchMovieCatalogInput, IReadOnlyList<MovieData>>, SearchMovieCatalog>();
        services.AddScoped<IUseCase<FindMovieByIdInput, MovieData>, FindMovieById>();
        services.AddScoped<IUseCase<GetHowestprimeScheduleInput, IReadOnlyList<MovieEventData>>, SearchMovieEventsInTimeRange>();
        // Register Policies (example of automatic registration with reflection at boot time)

        services.RegisterPolicies();

        return services;
    }

    private static IServiceCollection RegisterPolicies(this IServiceCollection services)
    {
        static bool IsPolicyInterface(Type t) =>
            t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IPolicy<>);

        var assembly = typeof(IPolicy<>).Assembly;

        foreach (var implementationType in assembly
                     .GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false }))
        {
            var policyInterfaces = implementationType
                .GetInterfaces()
                .Where(IsPolicyInterface)
                .Distinct()
                .ToArray();

            if (policyInterfaces.Length == 0) continue;

            foreach (var policyInterface in policyInterfaces)
                services.AddScoped(policyInterface, implementationType);

            services.AddScoped(implementationType);
        }

        return services;
    }
}
