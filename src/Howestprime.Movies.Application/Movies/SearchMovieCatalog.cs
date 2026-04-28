using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Application.Movies;

public sealed record SearchMovieCatalogInput(
    string Title,
    IEnumerable<string> Genres
);

public sealed class SearchMovieCatalog(
    ISearchMovieCatalogQuery searchMovieCatalogQuery
) : IUseCase<SearchMovieCatalogInput, IEnumerable<Movie>>
{

    public async Task<IEnumerable<Movie>> Execute(SearchMovieCatalogInput input)
    {
        IEnumerable<Movie> movies = await searchMovieCatalogQuery.Fetch(input.Title, input.Genres);

        return movies;
    }
}
