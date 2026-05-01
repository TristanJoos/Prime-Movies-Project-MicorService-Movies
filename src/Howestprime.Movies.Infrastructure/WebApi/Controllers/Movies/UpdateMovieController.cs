using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using System.ComponentModel.DataAnnotations;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record UpdateMovieRequest(
    [FromBody] UpdateMovieBody Body,
    [FromServices] IUseCase<ChangeMovieDetailsInput, Guid> UseCase
);

public static class UpdateMovieController
{
    public static async Task<Results<Created, BadRequest>> Invoke(
        [AsParameters] UpdateMovieRequest request
    )
    {
        ChangeMovieDetailsInput input = new(
            request.Body.MovieId,
            request.Body.Title,
            request.Body.Description,
            request.Body.Duration,
            request.Body.Genres,
            request.Body.ReleaseYear,
            request.Body.Actors,
            request.Body.AgeRating,
            request.Body.PosterUrl
        );

        Guid movieId = await request.UseCase.Execute(input);
        
        return TypedResults.Created($"/api/movie-catalog/{movieId}");
    }
}

public record UpdateMovieBody(
    [Required] Guid MovieId,
    [Required] string Title,
    [Required] string Description,
    [Required] int Duration,
    [Required] IEnumerable<string> Genres,
    [Required] int ReleaseYear,
    [Required] IEnumerable<string> Actors,
    [Required] int AgeRating,
    [Required] string PosterUrl
);
