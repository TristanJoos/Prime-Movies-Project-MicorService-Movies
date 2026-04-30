using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace Howestprime.Movies.Application.Movies;

public sealed record BookMovieEventInput(
    Guid MovieEventId,
    int StandardVisitors,
    int DiscountVisitors
);

public sealed class BookMovieEvent(
    IUnitOfWork uow
) : IUseCase<BookMovieEventInput, Guid>
{
    private readonly IUnitOfWork uow = uow;

    public async Task<Guid> Execute(BookMovieEventInput input)
    {
        MovieEvent movieEvent = await uow.Get<IMovieEventRepository>().GetById(input.MovieEventId);
        MovieEvent movieEvent = MovieEvent.Book(
            input.MovieEventId,
            input.StandardVisitors,
            input.DiscountVisitors
        );

        await uow.Save<IMovieEventRepository>(movieEvent);
        await uow.Do();

        return movieEvent.Id.Value;
    }
            input.Actors.Select(Actors.Create),
            AgeRating.Create(input.AgeRating),
            PosterUrl.Create(input.PosterUrl)
        );

        await uow.Save<IMovieRepository>(movie);
        await uow.Do();

        return movie.Id.Value;
    }
}
