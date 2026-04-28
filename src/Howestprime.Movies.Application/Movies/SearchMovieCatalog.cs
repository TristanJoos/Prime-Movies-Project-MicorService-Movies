using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;

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
        IReadOnlyList<GenreData> normalizedGenres = [
            .. input.Genres.Select(genre => new GenreData(genre))
        ];

        return await searchMovieCatalogQuery.Fetch(
            MovieDataFilters.ByTitleAndGenres(input.Title, normalizedGenres)
        );
    }


}
