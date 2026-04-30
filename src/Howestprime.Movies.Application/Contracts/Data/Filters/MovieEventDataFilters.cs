using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;

public static class MovieEventDataFilters
{
    public static Expression<Func<MovieEventData, bool>> InTimeRange(DateTime fromDate, DateTime toDate)
    {
        return movieEvent => movieEvent.Showtime >= fromDate && movieEvent.Showtime <= toDate;
    }
}