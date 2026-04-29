using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using System.ComponentModel.DataAnnotations;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record ScheduleMovieEventRequest(
    [FromBody] ScheduleMovieEventBody Body,
    [FromServices] IUseCase<ScheduleMovieEventInput, Guid> UseCase
);

public static class ScheduleMovieEventController
{
    public static async Task<Results<Created, BadRequest>> Invoke(
        [AsParameters] ScheduleMovieEventRequest request
    )
    {
        ScheduleMovieEventInput input = new(
            request.Body.MovieId,
            request.Body.RoomId,
            request.Body.Showtime
        );

        Guid movieId = await request.UseCase.Execute(input);
        
        return TypedResults.Created($"/api/movie-catalog/{movieId}");
    }
}

public record ScheduleMovieEventBody(
    [Required] Guid MovieId,
    [Required] Guid RoomId,
    [Required] DateTime Showtime,
    [Required] int Capacity
);
