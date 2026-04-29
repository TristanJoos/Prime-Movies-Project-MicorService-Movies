using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;


namespace Howestprime.Movies.Application.Movies;

public sealed record ScheduleMovieEventInput(
    MovieId MovieId,
    RoomId RoomId,
    DateTime Showtime,
    int Capacity
);


public sealed class ScheduleMovieEvent(
    IUnitOfWork uow
) : IUseCase<ScheduleMovieEventInput, Guid>
{
    private readonly IUnitOfWork uow = uow;

    public async Task<Guid> Execute(ScheduleMovieEventInput input)
    {
        if (!await uow.Repo<IMovieRepository>().Exists(input.MovieId))
        {
            throw new InvalidOperationException("Movie not found");
        }
        
        IMovieEventRepository repo = uow.Repo<IMovieEventRepository>();
        MovieEvent? existingEvent = await repo.ByShowtimeAndRoomId(input.Showtime, input.RoomId);

        if (existingEvent != null)
        {
            existingEvent.UpdateMovie(input.MovieId);
            await uow.Do(); 
            return existingEvent.Id.Value;
        }

        var movieEvent = MovieEvent.Create(input.MovieId, input.RoomId, input.Showtime, input.Capacity);
        await uow.Save<IMovieEventRepository>(movieEvent);
        await uow.Do();

        return movieEvent.Id.Value;

    }
}
