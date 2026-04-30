using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Infrastructure.WebApi.Controllers.Movies;
using Howestprime.Movies.Infrastructure.WebApi.Controllers.Responses;
using Xunit;

namespace UnitTests.Infrastructure.WebApi.Controllers;

public class FindMovieEventByYearAndMonthControllerTests
{
    private class FakeUseCase : IUseCase<GetHowestprimeScheduleInput, IReadOnlyList<MovieEventData>>
    {
        public GetHowestprimeScheduleInput? ReceivedInput { get; private set; }
        public IReadOnlyList<MovieEventData> ResultsToReturn { get; set; } = new List<MovieEventData>();

        public Task<IReadOnlyList<MovieEventData>> Execute(GetHowestprimeScheduleInput? input)
        {
            ReceivedInput = input;
            return Task.FromResult(ResultsToReturn);
        }
    }

    [Fact]
    public async Task Invoke_WithValidYearAndMonth_ReturnsOkWithMappedResponses()
    {
        // Arrange
        var fakeUseCase = new FakeUseCase();

        var movieId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var movieEventId = Guid.NewGuid();

        var movieData = new MovieData
        {
            Id = movieId,
            Title = "Test Movie",
            Description = "Test Desc",
            ReleaseYear = 2024,
            Duration = 120,
            Genres = new[] { new GenreData("Action") },
            Actors = new[] { new ActorData("Actor One") },
            AgeRating = 13,
            PosterUrl = "url"
        };

        var roomData = new RoomData
        {
            Id = roomId,
            Name = "Room A",
            Capacity = 100
        };

        var eventData = new MovieEventData
        {
            Id = movieEventId,
            Showtime = new DateTime(2024, 5, 10, 15, 0, 0),
            Capacity = 100,
            Room = roomData,
            Movie = movieData
        };

        fakeUseCase.ResultsToReturn = new List<MovieEventData> { eventData };

        var request = new FindMovieEventByYearAndMonthRequest(2024, 5, fakeUseCase);

        // Act
        var result = await FindMovieEventByYearAndMonthController.Invoke(request);

        // Assert
        Assert.IsType<Results<Ok<IReadOnlyList<MovieEventResponse>>, BadRequest>>(result);
        var okResult = (Ok<IReadOnlyList<MovieEventResponse>>)result.Result;

        Assert.NotNull(okResult.Value);
        Assert.Single(okResult.Value);

        var response = okResult.Value.First();
        Assert.Equal(movieEventId, response.Id);
        Assert.Equal(movieId, response.Movie.Id);
        Assert.Equal("Test Movie", response.Movie.Title);
        Assert.Equal(roomId, response.Room.Id);
        Assert.Equal("Room A", response.Room.Name);
        Assert.Equal(100, response.Capacity);

        // Verify input dates correct
        Assert.NotNull(fakeUseCase.ReceivedInput);
        Assert.Equal(new DateTime(2024, 5, 1), fakeUseCase.ReceivedInput.FromDate);
        Assert.Equal(new DateTime(2024, 5, 1).AddMonths(1).AddSeconds(-1), fakeUseCase.ReceivedInput.ToDate);
    }
}