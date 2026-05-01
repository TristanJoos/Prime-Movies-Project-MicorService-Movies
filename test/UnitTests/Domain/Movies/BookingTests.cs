using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Xunit;

namespace UnitTests.Domain.Movies;

public class BookingTests
{
    [Fact]
    public void Create_ValidInput_ReturnsBooking()
    {
        // Act
        var booking = Booking.Create(2, 1);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(2, booking.StandardVisitors);
        Assert.Equal(1, booking.DiscountVisitors);
        Assert.Equal(BookingStatus.open, booking.BookingStatus);
        Assert.Equal(PaymentStatus.pending, booking.PaymentStatus);
        Assert.NotEqual(Guid.Empty, booking.Id.Value);
        Assert.Empty(booking.SeatNumbers);
    }

    [Fact]
    public void Create_NegativeStandardVisitors_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Booking.Create(-1, 0));
    }

    [Fact]
    public void Create_NegativeDiscountVisitors_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Booking.Create(0, -1));
    }

    [Fact]
    public void AddSeatNumbers_ValidSeats_AddsSeats()
    {
        // Arrange
        var booking = Booking.Create(2, 0);
        var seats = new List<string> { "Seat 1", "Seat 2" };

        // Act
        booking.AddSeatNumbers(seats);

        // Assert
        Assert.Equal(2, booking.SeatNumbers.Count);
        Assert.Contains("Seat 1", booking.SeatNumbers);
        Assert.Contains("Seat 2", booking.SeatNumbers);
    }

    [Fact]
    public void AddSeatNumbers_NullList_ThrowsArgumentNullException()
    {
        var booking = Booking.Create(1, 0);
        Assert.Throws<ArgumentNullException>(() => booking.AddSeatNumbers(null!));
    }

    [Fact]
    public void AddSeatNumbers_EmptySeatString_ThrowsArgumentException()
    {
        var booking = Booking.Create(1, 0);
        var seats = new List<string> { "" };
        Assert.Throws<ArgumentException>(() => booking.AddSeatNumbers(seats));
    }

    [Fact]
    public void ValidateState_ThrowsNotImplementedException()
    {
        var booking = Booking.Create(1, 0);
        Assert.Throws<NotImplementedException>(() => booking.ValidateState());
    }
}
