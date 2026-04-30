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

    public void Book(Booking booking , string RoomName)
     {
         Asserts.EnsureNotEmpty(booking);
         int totalVisitors = Visitors + booking.StandardVisitors + booking.DiscountVisitors;
         if (totalVisitors > Capacity)
         {
             throw new InvalidOperationException("Cannot book more visitors than the capacity of the movie event.");
         }
         Bookings.Add(booking);
         Visitors = totalVisitors;

        this.RaiseDomainEvent(new BookingOpened(
            booking.Id,
            Id,
            RoomName,
            booking.PaymentStatus,
            booking.StandardVisitors,
            booking.DiscountVisitors,
            booking.SeatNumbers
        ));
     }
}