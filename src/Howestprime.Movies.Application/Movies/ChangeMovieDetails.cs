using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Shared.Exceptions;

namespace Howestprime.Movies.Application.Movies;

public sealed record ChangeMovieDetailsInput(
    Guid MovieId,
    string Title,
    string Description,
    int Duration,
    IEnumerable<string> Genres,
    int ReleaseYear,
    IEnumerable<string> Actors,
    int AgeRating,
    string PosterUrl
);

public sealed class ChangeMovieDetails(
    IUnitOfWork uow
) : IUseCase<ChangeMovieDetailsInput>
{
    private readonly IUnitOfWork uow = uow;

    public async Task Execute(ChangeMovieDetailsInput input)
    {
       MovieId movieId = new(input.MovieId);
        Optional<Movie> optionalMovie = await uow.Repo<IMovieRepository>().ById(movieId);

        if (!optionalMovie.HasValue)
        {
            throw new NotFoundException("Movie not found.");
        }

        Movie movie = optionalMovie.Value;

        var releaseYear = new ReleaseYear(input.ReleaseYear);
        var duration = new Duration(input.Duration);
        var posterUrl = new PosterUrl(input.PosterUrl);
        var genres = input.Genres.Select(g => new Genres(g));
        var actors = input.Actors.Select(a => new Actors(a));
        AgeRating ageRating = new(input.AgeRating);

        movie.ChangeDetails(
            input.Title,
            input.Description,
            releaseYear,
            duration,
            genres,
            actors,
            ageRating,
            posterUrl
        );

        await uow.Save<IMovieRepository>(movie);
        await uow.Do(); 
    }
}
