using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Application.Movies;

public sealed record RegisterMovieInput(
    string Title,
    string Description,
    int Duration,
    IEnumerable<string> Genres,
    int ReleaseYear,
    IEnumerable<string> Actors,
    int AgeRating,
    string PosterUrl
);

public sealed class RegisterMovie(
    IUnitOfWork uow
) : IUseCase<RegisterMovieInput, Guid>
{
    private readonly IUnitOfWork uow = uow;

    public async Task<Guid> Execute(RegisterMovieInput input)
    {
        var movie = Movie.Create(
            input.Title,
            input.Description,
            ReleaseYear.Create(input.ReleaseYear),
            Duration.Create(input.Duration),
            input.Genres.Select(Genres.Create),
            input.Actors.Select(Actors.Create),
            AgeRating.Create(input.AgeRating),
            PosterUrl.Create(input.PosterUrl)
        );

        await uow.Save<IMovieRepository>(movie);
        await uow.Do();

        return movie.Id.Value;
    }
}
