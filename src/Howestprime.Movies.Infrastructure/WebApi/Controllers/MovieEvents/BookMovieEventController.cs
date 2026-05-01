using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using System.ComponentModel.DataAnnotations;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record BookMovieEventRequest(
    [FromRoute] Guid Id,
    [FromBody] BookMovieEventBody Body,
    [FromServices] IUseCase<BookMovieEventInput, Guid> UseCase
);

public static class BookMovieEventController
{
    public static async Task<Results<Created, BadRequest>> Invoke(
        [AsParameters] BookMovieEventRequest request
    )
    {
        BookMovieEventInput input = new(
            request.Id,
            request.Body.standardVisitors,
            request.Body.discountVisitors
        );

        Guid movieEventId = await request.UseCase.Execute(input);
        
        return TypedResults.Created($"/api/movie-events/{movieEventId}");
    }
}

public record BookMovieEventBody(
    [Required] int standardVisitors,
    [Required] int discountVisitors
);
