
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Infrastructure.WebApi.Controllers.Responses;

namespace Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;

public record FindMovieEventByYearAndMonthRequest(
    [FromQuery(Name = "year")] int Year,
    [FromQuery(Name = "month")] int Month,
    [FromServices] IUseCase<GetHowestprimeScheduleInput, IReadOnlyList<MovieEventData>> UseCase
);

public static class FindMovieEventByYearAndMonthController
{
    public static async Task<Results<Ok<IReadOnlyList<MovieEventResponse>>, BadRequest>> Invoke(
        [AsParameters] FindMovieEventByYearAndMonthRequest request
    )
    {
        DateTime from = new DateTime(request.Year, request.Month, 1);
        DateTime to = from.AddMonths(1).AddSeconds(-1);

        GetHowestprimeScheduleInput input = new(from, to);

        IReadOnlyList<MovieEventData> movieEventsData =
            await request.UseCase.Execute(input);


        var movieEventsResponses = movieEventsData.Select(me => new MovieEventResponse(
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
                me.Room.Capacity
            ),
            me.Showtime,
            me.Capacity
        )).ToList();

        return TypedResults.Ok<IReadOnlyList<MovieEventResponse>>(movieEventsResponses);
    }
}
