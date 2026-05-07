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
        DateTime utcValue = input.Showtime.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(input.Showtime, DateTimeKind.Utc)
            : input.Showtime.ToUniversalTime();

        var roomOptional = await uow.Repo<IRoomRepository>().ById(roomId);

        Room? room = roomOptional.HasValue ? roomOptional.Value : null;
        if (!await uow.Repo<IMovieRepository>().Exists(movieId))
        {
            throw new InvalidOperationException("Movie not found");
        }

        if (room == null)
        {
            throw new InvalidOperationException("Room not found");
        }

        IMovieEventRepository repo = uow.Repo<IMovieEventRepository>();
        MovieEvent? existingEvent = await repo.ByShowtimeAndRoomId(utcValue, input.RoomId);
        if (existingEvent != null)
        {
            existingEvent.UpdateMovie((movieId));
            await uow.Do();
            return existingEvent.Id.Value;
        }

        var movieEvent = MovieEvent.Create(movieId, roomId, utcValue, room.Capacity);
        await uow.Save<IMovieEventRepository>(movieEvent);
        await uow.Do();

        return movieEvent.Id.Value;

    }
}
