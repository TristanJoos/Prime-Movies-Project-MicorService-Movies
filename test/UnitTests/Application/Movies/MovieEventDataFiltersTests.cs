using Howestprime.Movies.Application.Contracts.Data;
using Xunit;

namespace UnitTests.Application.Movies;

public class MovieEventDataFiltersTests
{
    [Fact]
    public void InTimeRange_ShouldReturnTrueIfWithinRange()
    {
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 12, 31);

        var filterExpression = MovieEventDataFilters.InTimeRange(fromDate, toDate);
        var compiledFilter = filterExpression.Compile();

        var insideRange = new MovieEventData { Id = Guid.NewGuid(), Showtime = new DateTime(2023, 6, 1), Capacity = 50, Room = null!, Movie = null! };
        var onFromDate = new MovieEventData { Id = Guid.NewGuid(), Showtime = fromDate, Capacity = 50, Room = null!, Movie = null! };
        var onToDate = new MovieEventData { Id = Guid.NewGuid(), Showtime = toDate, Capacity = 50, Room = null!, Movie = null! };

        Assert.True(compiledFilter(insideRange));
        Assert.True(compiledFilter(onFromDate));
        Assert.True(compiledFilter(onToDate));
    }

    [Fact]
    public void InTimeRange_ShouldReturnFalseIfOutsideRange()
    {
        var fromDate = new DateTime(2023, 1, 1);
        var toDate = new DateTime(2023, 12, 31);

        var filterExpression = MovieEventDataFilters.InTimeRange(fromDate, toDate);
        var compiledFilter = filterExpression.Compile();

        var beforeRange = new MovieEventData { Id = Guid.NewGuid(), Showtime = new DateTime(2022, 12, 31), Capacity = 50, Room = null!, Movie = null! };
        var afterRange = new MovieEventData { Id = Guid.NewGuid(), Showtime = new DateTime(2024, 1, 1), Capacity = 50, Room = null!, Movie = null! };

        Assert.False(compiledFilter(beforeRange));
        Assert.False(compiledFilter(afterRange));
    }
}