using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;

namespace Howestprime.Movies.Application.Movies;

public sealed record FindMovieByIdInput(
    Guid MovieId,
    string UserRole
);

public sealed class FindMovieById(
    ISearchMovieCatalogQuery searchMovieCatalogQuery,
    IAuthorizationService AuthorizationService
) : IUseCase<FindMovieByIdInput, MovieData>
{

     public async Task<MovieData> Execute(FindMovieByIdInput input)
    { 
        AuthorizationService.Authorize(input.UserRole, nameof(FindMovieById));

        IReadOnlyList<MovieData> movies = await searchMovieCatalogQuery.Fetch(
            MovieDataFilters.ById(input.MovieId));
        
        MovieData? movie = movies.FirstOrDefault();
        if (movie == null)
        {
            throw new Exception("Movie not found");
        }

        return movie;
    }
}
