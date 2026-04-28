
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Domain.Movies;

namespace Howestprime.Movies.Application.Contracts.Ports;

public interface ISearchMovieCatalogQuery
{
    public Task<IEnumerable<MovieData>> Fetch(
        string title,
        IEnumerable<string> genres
    );
}

