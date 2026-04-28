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

        return movies;
    }
}