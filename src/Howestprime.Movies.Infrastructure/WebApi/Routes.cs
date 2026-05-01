using Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Howestprime.Movies.Infrastructure.WebApi;

public static class Routes
{
    public static IEndpointRouteBuilder MapRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder webApi = app.MapGroup("/api");

        webApi.MapMovieRoutes();
        webApi.MapMovieEventRoutes();
        webApi.MapScheduleRoutes();
        return webApi;
    }

    public static RouteGroupBuilder MapMovieRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder movies = app.MapGroup("/movie-catalog")
            .WithTags("Movie Catalog")
            .WithDescription("All endpoints related to managing the movie catalog.");

        movies.MapPost("/", RegisterMovieController.Invoke)
           .WithName("RegisterMovie")
           .WithDescription(" Register a new movie in the catalog.");

        movies.MapGet("/", SearchMovieCatalogController.Invoke)
            .WithName("SearchMovieCatalog")
            .WithDescription("Filter movies based on title and genres.");

        movies.MapGet("/{id:guid}", FindMovieByIdController.Invoke)
            .WithName("FindMovieById")
            .WithDescription("Find a movie by its unique identifier.");

        movies.MapPut("/{id:guid}", UpdateMovieController.Invoke)
            .WithName("UpdateMovie")
            .WithDescription("Update the details of an existing movie.");

        return movies;
    }

    public static RouteGroupBuilder MapMovieEventRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder movieEvents = app.MapGroup("/movie-events")
            .WithTags("Movie Events")
            .WithDescription("All endpoints related to managing movie events.");

        movieEvents.MapPost("/", ScheduleMovieEventController.Invoke)
           .WithName("ScheduleMovieEvent")
           .WithDescription("Schedule a new movie event.");
        
        movieEvents.MapGet("/", FindMovieEventByYearAndMonthController.Invoke)
            .WithName("FindMovieEventByYearAndMonth")
            .WithDescription("Find movie events by year and month.");
        movieEvents.MapPost("/{id:guid}/bookings", BookMovieEventController.Invoke)
            .WithName("BookMovieEvent")
            .WithDescription("Book a movie event.");
        return movieEvents;
    }

    public static RouteGroupBuilder MapScheduleRoutes(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder schedule = app.MapGroup("/howestprime-schedule")
            .WithTags("Schedule")
            .WithDescription("Endpoints related to the movie event schedule.");

        schedule.MapGet("/", GetHowestprimeScheduleController.Invoke)
            .WithName("GetHowestprimeSchedule")
            .WithDescription("Retrieve the schedule of upcoming movie events.");
        return schedule;
    }
}