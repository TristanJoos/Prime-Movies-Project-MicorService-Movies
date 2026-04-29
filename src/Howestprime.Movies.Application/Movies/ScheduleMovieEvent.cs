using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;


namespace Howestprime.Movies.Application.Movies;

public sealed record ScheduleMovieEventInput(
    Guid MovieId,
    Guid RoomId,
    DateTime Showtime
);


public sealed class ScheduleMovieEvent(
    IUnitOfWork uow
) : IUseCase<ScheduleMovieEventInput, Guid>
{
    private readonly IUnitOfWork uow = uow;

    public async Task<Guid> Execute(ScheduleMovieEventInput input)
    {

        MovieId movieId = new(input.MovieId);
        RoomId roomId = new(input.RoomId);
        if (!await uow.Repo<IMovieRepository>().Exists(movieId))
        {
            throw new InvalidOperationException("Movie not found");
        }
        
        IMovieEventRepository repo = uow.Repo<IMovieEventRepository>();
        MovieEvent? existingEvent = await repo.ByShowtimeAndRoomId(input.Showtime, input.RoomId);
        if (existingEvent != null)
        {
            existingEvent.UpdateMovie((movieId));
            await uow.Do(); 
            return existingEvent.Id.Value;
        }

        var movieEvent = MovieEvent.Create(movieId, roomId, input.Showtime);
        await uow.Save<IMovieEventRepository>(movieEvent);
        await uow.Do();

        return movieEvent.Id.Value;

    }
}
