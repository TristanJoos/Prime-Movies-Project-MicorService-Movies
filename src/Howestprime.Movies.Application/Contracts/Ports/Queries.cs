using System.Linq.Expressions;
using Howestprime.Movies.Application.Contracts.Data;

namespace Howestprime.Movies.Application.Contracts.Ports;

public interface ISearchMovieCatalogQuery
{
    public Task<IEnumerable<MovieData>> Fetch(
        Expression<Func<MovieData, bool>> filter
    );
}

