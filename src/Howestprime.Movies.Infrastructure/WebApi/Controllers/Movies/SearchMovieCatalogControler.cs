
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
    [FromServices] IUseCase<SearchMovieCatalogInput, IEnumerable<MovieData>> UseCase
);

public static class SearchMovieCatalogController
{
    public static async Task<Results<Ok<MovieDataCollection>, BadRequest>> Invoke(
        [AsParameters] SearchMovieCatalogRequest request
    )
    {
        IEnumerable<string> genres = request.Genres.Split(',').Select(g => g.Trim()).Where(g => !string.IsNullOrEmpty(g));
        SearchMovieCatalogInput input = new(request.Title, genres);

        IEnumerable<MovieData> moviesData =
            await request.UseCase.Execute(input);

        return TypedResults.Ok(new MovieDataCollection([.. moviesData]));
    }
}
