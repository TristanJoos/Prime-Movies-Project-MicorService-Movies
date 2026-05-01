using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using System.ComponentModel.DataAnnotations;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record UpdateMovieRequest(
    [FromRoute] Guid Id,
    [FromBody] UpdateMovieBody Body,
    [FromServices] IUseCase<ChangeMovieDetailsInput> UseCase
);

public static class UpdateMovieController
{
    public static async Task<Results<NoContent, BadRequest>> Invoke(
        [AsParameters] UpdateMovieRequest request
    )
    {
        ChangeMovieDetailsInput input = new(
            request.Id,
            request.Body.Title,
            request.Body.Description,
            request.Body.Duration,
            request.Body.Genres,
            request.Body.ReleaseYear,
            request.Body.Actors,
            request.Body.AgeRating,
            request.Body.PosterUrl
        );

        await request.UseCase.Execute(input);
        return TypedResults.NoContent();
    }
}

public record UpdateMovieBody(
    [Required] string Title,
    [Required] string Description,
    [Required] int Duration,
    [Required] IEnumerable<string> Genres,
    [Required] int ReleaseYear,
    [Required] IEnumerable<string> Actors,
    [Required] int AgeRating,
    [Required] string PosterUrl
);
