
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Movies;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record GetHowestprimeScheduleRequest(
    [FromServices] IUseCase<GetHowestprimeScheduleInput, IReadOnlyList<MovieEventData>> UseCase
);

public static class GetHowestprimeScheduleController
{
    public static async Task<Results<Ok<HowestprimeScheduleResponse>, BadRequest>> Invoke(
        [AsParameters] GetHowestprimeScheduleRequest request
    )
    {   

        DateTime fromDate = DateTime.UtcNow;
        DateTime toDate = fromDate.AddDays(14);
        GetHowestprimeScheduleInput input = new(fromDate, toDate);
        IReadOnlyList<MovieEventData>? movieEvents = await request.UseCase.Execute(input);

        if (movieEvents == null)
        {
            return TypedResults.BadRequest();
        }
    
        List<Guid> movieIds = movieEvents.Select(me => me.Movie.Id).ToList();
        List<MovieEventResponse> movieEventResponses = movieEvents.Select(me => new MovieEventResponse(
            me.Id,
            new MovieResponse(
                me.Movie.Id,
                me.Movie.Title,
                me.Movie.Description,
                me.Movie.ReleaseYear,
                me.Movie.Duration,
                me.Movie.Genres.Select(g => g.Value).ToList(),
                me.Movie.Actors.Select(a => a.Value).ToList(),
                me.Movie.AgeRating,
                me.Movie.PosterUrl
            ),
            new RoomResponse(
                me.Room.Id,
                me.Room.Name, 
                me.Room.Capacity),
            me.Showtime,
            me.Capacity
        )).ToList();

        return TypedResults.Ok(new HowestprimeScheduleResponse(movieIds , movieEventResponses));
    }
}
