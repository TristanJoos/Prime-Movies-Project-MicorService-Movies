using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;

namespace Howestprime.Movies.Application.Movies;

public sealed record SearchMovieEventsInTimeRangeInput(
    DateTime FromDate,
    DateTime ToDate 
);

public sealed class SearchMovieEventsInTimeRange(
    IMovieEventQuery MovieEventQuery
) : IUseCase<SearchMovieEventsInTimeRangeInput, IReadOnlyList<MovieEventData>>
{


    public async Task<IReadOnlyList<MovieEventData>> Execute(SearchMovieEventsInTimeRangeInput? input)
    {
        DateTime from = input?.FromDate ?? DateTime.UtcNow;
        DateTime to = input?.ToDate ?? DateTime.UtcNow;

        from = from.ToUniversalTime();
        to = to.ToUniversalTime();

        if (from > to)
        {
            throw new ArgumentException("From date must be before or equal to to date.");
        }

        return await MovieEventQuery.Fetch(
            MovieEventDataFilters.InTimeRange(from, to)
        );
    }
}
