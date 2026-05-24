using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Xunit;

namespace UnitTests.Domain.Movies;

public class MovieEventTests
{
    private readonly MovieId _validMovieId = new MovieId(Guid.NewGuid());
    private readonly RoomId _validRoomId = new RoomId(Guid.NewGuid());
    private readonly DateTime _validShowtime;

    public MovieEventTests()
    {
        var now = DateTime.Now;
        _validShowtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);
    }

    [Fact]
    public void Create_WithValidData_ReturnsMovieEvent()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            100
        );

        Assert.NotNull(movieEvent);
        Assert.Equal(_validMovieId, movieEvent.MovieId);
        Assert.Equal(_validRoomId, movieEvent.RoomId);
        Assert.Equal(_validShowtime, movieEvent.Showtime);
        Assert.Equal(100, movieEvent.Capacity);
    }

    [Fact]
    public void Create_WithZeroCapacity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            0
        ));
    }

    [Fact]
    public void Create_InvalidShowtimeHour_ThrowsArgumentException()
    {
        var invalidHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0).AddDays(1);
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            invalidHour,
            100
        ));
    }

    [Fact]
    public void Create_PastShowtime_ThrowsArgumentException()
    {
        var pastShowtime = new DateTime(2000, 1, 1, 15, 0, 0);
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            pastShowtime,
            100
        ));
    }

    [Fact]
    public void PrivateConstructor_IsExecuted()
    {
        var ctor = typeof(MovieEvent).GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        Assert.NotNull(instance);
    }

    [Fact]
    public void UpdateMovie_WithValidMovieId_UpdatesMovieId()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            100
        );

        var newMovieId = new MovieId(Guid.NewGuid());
        movieEvent.UpdateMovie(newMovieId);

        Assert.Equal(newMovieId, movieEvent.MovieId);
    }

    [Fact]
    public void Book_ValidBooking_AddsBookingAndRaisesEvent()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            10
        );

        var booking = Booking.Create(2, 0);

        movieEvent.Book(booking, "Room 1");

        Assert.Single(movieEvent.Bookings);
        Assert.Equal(2, movieEvent.Visitors);
        Assert.Equal(2, booking.SeatNumbers.Count);
        Assert.Equal("1", booking.SeatNumbers[0]);
        Assert.Equal("2", booking.SeatNumbers[1]);
        Assert.NotEmpty(movieEvent.DomainEvents);
    }

    [Fact]
    public void Book_ExceedsCapacity_ThrowsInvalidOperationException()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            2
        );

        var booking = Booking.Create(3, 0);

        Assert.Throws<InvalidOperationException>(() => movieEvent.Book(booking, "Room 1"));
    }

    [Fact]
    public void CloseBooking_WithPaymentSuccess_ClosesBookingAndMarksPaymentAsPaid()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            10
        );
        var booking = Booking.Create(2, 0);
        movieEvent.Book(booking, "Room 1");

        movieEvent.CloseBooking(booking.Id.Value, "PaymentSuccess");

        Assert.Equal(BookingStatus.closed, booking.BookingStatus);
        Assert.Equal(PaymentStatus.success, booking.PaymentStatus);
        Assert.Equal(2, movieEvent.Visitors);
        Assert.Equal(["1", "2"], booking.SeatNumbers);
    }

    [Fact]
    public void CloseBooking_WithPaymentFailed_ClosesBookingMarksPaymentAsFailedAndReleasesSeats()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            10
        );
        var booking = Booking.Create(1, 1);
        movieEvent.Book(booking, "Room 1");

        movieEvent.CloseBooking(booking.Id.Value, "PaymentFailed");

        Assert.Equal(BookingStatus.closed, booking.BookingStatus);
        Assert.Equal(PaymentStatus.failed, booking.PaymentStatus);
        Assert.Equal(0, movieEvent.Visitors);
        Assert.Empty(booking.SeatNumbers);
    }

    [Fact]
    public void CloseBooking_WithUnknownBooking_ThrowsArgumentException()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            10
        );

        Assert.Throws<ArgumentException>(() => movieEvent.CloseBooking(Guid.NewGuid(), "PaymentSuccess"));
    }

    [Fact]
    public void CloseBooking_WithInvalidReason_ThrowsArgumentException()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            10
        );
        var booking = Booking.Create(1, 0);
        movieEvent.Book(booking, "Room 1");

        Assert.Throws<ArgumentException>(() => movieEvent.CloseBooking(booking.Id.Value, "Canceled"));
    }
}
