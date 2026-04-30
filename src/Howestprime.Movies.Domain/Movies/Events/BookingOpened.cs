namespace Howestprime.Movies.Domain.Movies.Events;

public sealed class BookingOpened(
    BookingId bookingId,
    MovieId movieId,
    string roomName,
    DateTime showtime,
    int standardVisitors,
    int discountVisitors,
    IList<string> seatNumbers
) : MovieEventDomainEvent(nameof(BookingOpened))
{
    public BookingId BookingId { get; } = bookingId;
    public MovieId MovieId { get; } = movieId;
    public string RoomName { get; } = roomName;
    public DateTime Showtime { get; } = showtime;
    public int StandardVisitors { get; } = standardVisitors;
    public int DiscountVisitors { get; } = discountVisitors;
    public IList<string> SeatNumbers { get; } = seatNumbers;
}
