using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;

namespace Howestprime.Movies.Application.Contracts.Ports;

public interface ISearchMovieCatalogQuery
{
    public Task<IReadOnlyList<MovieData>> Fetch(
        Expression<Func<MovieData, bool>> filter
    );
}

public interface IMovieEventQuery
{
    public Task<IReadOnlyList<MovieEventData>> Fetch(
        Expression<Func<MovieEventData, bool>> filter
    );
}
