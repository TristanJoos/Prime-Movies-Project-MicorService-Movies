using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Shared.Exceptions;

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
    public async Task<Guid> Execute(BookMovieEventInput input)
    {
        if (input.StandardVisitors < 0 || input.DiscountVisitors < 0)
        {
            throw new ArgumentException("Visitors cannot be negative.");
        }
        if (input.MovieEventId == Guid.Empty)
        {
            throw new ArgumentException("Movie event ID cannot be empty.");
        }

        MovieEventId movieEventId = new(input.MovieEventId);
        Optional<MovieEvent> optionalMovieEvent = await uow.Repo<IMovieEventRepository>().ById(movieEventId);
        if (!optionalMovieEvent.HasValue)
        {
            throw new NotFoundException("Movie event not found.");
        }

        if (optionalMovieEvent.Value.Capacity < input.StandardVisitors + input.DiscountVisitors)
        {
            throw new InvalidOperationException("Not enough available seats for this movie event.");
        }


        Optional<Room> optionalRoom = await uow.Repo<IRoomRepository>().ById(optionalMovieEvent.Value.RoomId);
        if (!optionalRoom.HasValue)
        {
            throw new NotFoundException("Room not found.");
        }
        DateTime fourteenDaysFromNow = DateTime.UtcNow.AddDays(14);
        
        if (optionalMovieEvent.Value.Showtime < DateTime.UtcNow || optionalMovieEvent.Value.Showtime > fourteenDaysFromNow)
        {
            throw new InvalidOperationException("Bookings can only be made for events occurring within the next 14 days.");
        }

        Booking booking = Booking.Create(
            input.StandardVisitors,
            input.DiscountVisitors
        );
        optionalMovieEvent.Value.Book(booking, optionalRoom.Value.Name);

        await uow.Save<IMovieEventRepository>(optionalMovieEvent.Value);
        await uow.Do();

        return booking.Id.Value;
    }
}
