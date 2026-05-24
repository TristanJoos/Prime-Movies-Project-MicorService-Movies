using Howestprime.Movies.Domain.Movies.Events;
using Howestprime.Movies.Domain.Shared;
namespace Howestprime.Movies.Domain.Movies;

public readonly record struct MovieEventId(Guid Value) : IEntityId;

public sealed class MovieEvent : AggregateRoot<MovieEventId>
{
    public MovieId MovieId { get; private set; }
    public RoomId RoomId { get; private set; }
    public DateTime Showtime { get; private set; }
    public int Capacity { get; private set; }
    public List<Booking> Bookings { get; private set; } = new();
    public int Visitors { get; private set; }

    public static MovieEvent Create(
        MovieId MovieId,
        RoomId RoomId,
        DateTime Showtime,
        int Capacity
    )
    {
        Asserts.EnsureNotEmpty(MovieId);
        Asserts.EnsureNotEmpty(RoomId);
        Asserts.EnsureGreaterThan(Capacity, 0);
        MovieEventAsserts.EnsureShowtimeIsAt15hOr19h(Showtime);
        MovieEventAsserts.EnsureShowtimeIsInTheFuture(Showtime);


        MovieEvent movieEvent = new MovieEvent(
            EntityId.New<MovieEventId>(),
            MovieId,
            RoomId,
            Showtime,
            Capacity
        );

        return movieEvent;
    }

    private MovieEvent(
        MovieEventId id,
        MovieId MovieId,
        RoomId RoomId,
        DateTime Showtime,
        int Capacity
    ) : base(id)
    {
        this.MovieId = MovieId;
        this.RoomId = RoomId;
        this.Showtime = Showtime;
        this.Capacity = Capacity;

        ValidateState();
    }

    // Required for ORM / Serialization
#pragma warning disable CS8618 
    private MovieEvent() : base(default!) { }
#pragma warning restore CS8618


    public override void ValidateState()
    {
        Asserts.EnsureNotEmpty(MovieId);
        Asserts.EnsureNotEmpty(RoomId);
        Asserts.EnsureGreaterThan(Capacity, 0);
        MovieEventAsserts.EnsureShowtimeIsAt15hOr19h(Showtime);
        MovieEventAsserts.EnsureShowtimeIsInTheFuture(Showtime);

    }

    public void UpdateMovie(MovieId newMovieId)
    {
        Asserts.EnsureNotEmpty(newMovieId);
        MovieId = newMovieId;
        ValidateState();
    }

    public void Book(Booking booking, string RoomName)
    {
        Asserts.EnsureNotEmpty(booking);

        int newVisitorsCount = booking.StandardVisitors + booking.DiscountVisitors;
        int totalVisitorsAfterBooking = Visitors + newVisitorsCount;

        if (totalVisitorsAfterBooking > Capacity)
        {
            throw new InvalidOperationException("Cannot book more visitors than the capacity of the movie event.");
        }

        List<string> assignedSeats = new();
        for (int i = 1; i <= newVisitorsCount; i++)
        {

            assignedSeats.Add($"{Visitors + i}");
        }


        booking.AddSeatNumbers(assignedSeats);


        Bookings.Add(booking);
        Visitors = totalVisitorsAfterBooking;

        RaiseDomainEvent(new BookingOpened(
            booking.Id,
            MovieId,
            RoomName,
            Showtime,
            booking.StandardVisitors,
            booking.DiscountVisitors,
            booking.SeatNumbers
        ));
    }

    public void CloseBooking(Guid bookingId, string reason)
    {
        var booking = Bookings.FirstOrDefault(b => b.Id.Value == bookingId);
        if (booking == null) throw new ArgumentException("Booking not found.");
        booking.Close();

        if (reason == "PaymentFailed")
        {
            Visitors -= booking.StandardVisitors + booking.DiscountVisitors;
            booking.MarkAsFailed();
        }
        else if (reason == "PaymentSuccess")
        {
            booking.MarkAsPaid();
        }
        else
        {
            throw new ArgumentException("Close booking reason must be PaymentSuccess or PaymentFailed.");
        }
    }
}
