
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Infrastructure.WebApi.Controllers.Responses;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record SearchMovieCatalogRequest(
    [FromQuery] string Title,
    [FromQuery] string Genres,
    [FromHeader(Name = "x-user-role")] string UserRole,
    [FromServices] IUseCase<SearchMovieCatalogInput, IReadOnlyList<MovieData>> UseCase
);

public static class SearchMovieCatalogController
{
    public static async Task<Results<Ok<MovieDataCollectionResponse>, BadRequest>> Invoke(
        [AsParameters] SearchMovieCatalogRequest request
    )
    {
        IEnumerable<string> genres = request.Genres.Split(',').Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g));
        SearchMovieCatalogInput input = new(request.Title, genres , request.UserRole);

        IReadOnlyList<MovieData> moviesData =
            await request.UseCase.Execute(input);

        return TypedResults.Ok(new MovieDataCollectionResponse([.. moviesData]));
    }
}
