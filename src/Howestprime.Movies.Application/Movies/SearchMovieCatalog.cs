using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;

namespace Howestprime.Movies.Application.Movies;

public sealed record SearchMovieCatalogInput(
    string Title,
    IEnumerable<string> Genres,
    string UserRole
);

public sealed class SearchMovieCatalog(
    ISearchMovieCatalogQuery searchMovieCatalogQuery,
    IAuthorizationService AuthorizationService
) : IUseCase<SearchMovieCatalogInput, IReadOnlyList<MovieData>>
{

     public Task<IReadOnlyList<MovieData>> Execute(SearchMovieCatalogInput input)
    { 
        IReadOnlyList<GenreData> normalizedGenres = [
            .. input.Genres.Select(genre => new GenreData(genre))
        ];

        AuthorizationService.Authorize(input.UserRole, nameof(SearchMovieCatalog));

        return searchMovieCatalogQuery.Fetch(
            MovieDataFilters.ByTitleAndGenres(input.Title, normalizedGenres)
        );
    }


}
