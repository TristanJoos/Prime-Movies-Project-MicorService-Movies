using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using System.ComponentModel.DataAnnotations;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record RegisterMovieRequest(
    [FromBody] RegisterMovieBody Body,
    [FromServices] IUseCase<RegisterMovieInput, Guid> UseCase
);

public static class RegisterMovieController
{
    /*input 
    {
  "title": "string",
  "description": "string",
  "releaseYear": 0,
  "duration": 0,
  "genres": [
    "string"
  ],
  "actors": [
    "string"
  ],
  "ageRating": 0,
  "posterUrl": "string"
}
*/
    public static async Task<Results<Created, BadRequest>> Invoke(
        [AsParameters] RegisterMovieRequest request
    )
    {
        RegisterMovieInput input = new(
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

public record RegisterMovieBody(
    [Required, MinLength(4)] string Title,
    [Required] string Description,
    [Required] int Duration,
    [Required] string Genres,
    [Required] int ReleaseYear,
    [Required] string Actors,
    [Required] int AgeRating,
    [Required] string PosterUrl
);
