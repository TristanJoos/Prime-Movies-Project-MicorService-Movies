using Howestprime.Movies.Application.Contracts.Data;
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
) : IUseCase<SearchMovieCatalogInput, IEnumerable<MovieData>>
{

    public async Task<IEnumerable<MovieData>> Execute(SearchMovieCatalogInput input)
    {
        IEnumerable<MovieData> movies = await searchMovieCatalogQuery.Fetch(input.Title, input.Genres);

        return movies;
    }
}
