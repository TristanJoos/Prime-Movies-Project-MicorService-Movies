
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Infrastructure.WebApi.Controllers.Responses;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record SearchMovieCatalogRequest(
    [FromQuery(Name = "title")] string? Title,
    [FromQuery(Name = "genres")] string? Genres,
    [FromHeader(Name = "x-user-role")] string UserRole,
    [FromServices] IUseCase<SearchMovieCatalogInput, IReadOnlyList<MovieData>> UseCase
);

public static class SearchMovieCatalogController
{
    public static async Task<Results<Ok<MovieDataCollectionResponse>, BadRequest>> Invoke(
        [AsParameters] SearchMovieCatalogRequest request
    )
    {
        IEnumerable<string> genres = request.Genres?.Split(',').Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g)) ?? Enumerable.Empty<string>();
        SearchMovieCatalogInput input = new(request.Title ?? string.Empty, genres, request.UserRole);

        IReadOnlyList<MovieData> moviesData =
            await request.UseCase.Execute(input);
        var response = new MovieDataCollectionResponse(
        [.. moviesData.Select(movie => new MovieResponse(
                Id: movie.Id,
                Title: movie.Title,
                Description: movie.Description,
                ReleaseYear: movie.ReleaseYear,
                Duration: movie.Duration,
                AgeRating: movie.AgeRating,
                PosterUrl: movie.PosterUrl,
                Genres: movie.Genres.Select(g => g.Value).ToList(),
                Actors: movie.Actors.Select(a => a.Value).ToList()
            ))]
            );
        return TypedResults.Ok(response);
    }
}
