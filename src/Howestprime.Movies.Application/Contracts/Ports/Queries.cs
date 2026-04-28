
using Howestprime.Movies.Domain.Movies;

namespace Howestprime.Movies.Application.Contracts.Ports;

public interface ISearchMovieCatalogQuery
{
    public Task<IEnumerable<Movie>> Fetch(
        string title,
        IEnumerable<string> genres
    );
}

