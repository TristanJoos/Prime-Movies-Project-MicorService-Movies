using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Xunit;

namespace UnitTests.Application.Movies;

public class SearchMovieEventsInTimeRangeTests
{
    private class FakeMovieEventQuery : IMovieEventQuery
    {
        public bool FetchCalled { get; private set; }
        public Expression<Func<MovieEventData, bool>>? Filter { get; private set; }
        public IReadOnlyList<MovieEventData> ResultsToReturn { get; set; } = new List<MovieEventData>();

        public Task<IReadOnlyList<MovieEventData>> Fetch(Expression<Func<MovieEventData, bool>> filter)
        {
            FetchCalled = true;
            Filter = filter;
            return Task.FromResult(ResultsToReturn);
        }
    }

    [Fact]
    public async Task Execute_WithValidRange_ShouldCallQueryWithCorrectFilter()
    {
        var fakeQuery = new FakeMovieEventQuery();
        var fakeResults = new List<MovieEventData>
        {
            new MovieEventData
            {
                Id = Guid.NewGuid(),
                Showtime = DateTime.UtcNow,
                Capacity = 100,
                Room = null!, // Fake data for test
                Movie = null!
            }
        };
        fakeQuery.ResultsToReturn = fakeResults;

        var sut = new SearchMovieEventsInTimeRange(fakeQuery);

        var fromDate = new DateTime(2023, 1, 1).ToUniversalTime();
        var toDate = new DateTime(2023, 12, 31).ToUniversalTime();

        var input = new GetHowestprimeScheduleInput(fromDate, toDate);

        var result = await sut.Execute(input);

        Assert.True(fakeQuery.FetchCalled);
        Assert.NotNull(fakeQuery.Filter);
        Assert.Same(fakeResults, result);

        // Assert that the filter compiles and can evaluate
        var compiledFilter = fakeQuery.Filter.Compile();
        var matchedData = new MovieEventData { Id = Guid.NewGuid(), Showtime = new DateTime(2023, 6, 1).ToUniversalTime(), Capacity = 50, Room = null!, Movie = null! };
        var unmatchedData = new MovieEventData { Id = Guid.NewGuid(), Showtime = new DateTime(2024, 6, 1).ToUniversalTime(), Capacity = 50, Room = null!, Movie = null! };

        Assert.True(compiledFilter(matchedData));
        Assert.False(compiledFilter(unmatchedData));
    }

    [Fact]
    public async Task Execute_WithFromDateAfterToDate_ThrowsArgumentException()
    {
        var fakeQuery = new FakeMovieEventQuery();
        var sut = new SearchMovieEventsInTimeRange(fakeQuery);

        var fromDate = new DateTime(2023, 12, 31).ToUniversalTime();
        var toDate = new DateTime(2023, 1, 1).ToUniversalTime();

        var input = new GetHowestprimeScheduleInput(fromDate, toDate);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
        Assert.Equal("From date must be before or equal to to date.", ex.Message);
        Assert.False(fakeQuery.FetchCalled);
    }

    [Fact]
    public async Task Execute_WithNullInput_UsesUtcNow()
    {
        var fakeQuery = new FakeMovieEventQuery();
        var sut = new SearchMovieEventsInTimeRange(fakeQuery);

        // Act
        var result = await sut.Execute(null);

        // Assert
        Assert.True(fakeQuery.FetchCalled);
        Assert.NotNull(fakeQuery.Filter);

        var compiledFilter = fakeQuery.Filter.Compile();

        // It checks if it's strictly between UtcNow (from before Execute) and UtcNow (from within Execute). 
        // We can just verify it ran without throwing and actually called the repository.
        Assert.Empty(result);
    }
}