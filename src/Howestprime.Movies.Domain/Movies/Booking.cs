/*Id (BookingId)	Unique identifier
BookingStatus (BookingStatus)	Status of the booking
PaymentStatus (PaymentStatus)	Status of the payment
StandardVisitors (int)	Number of standard visitors
DiscountVisitors (int)	Number of discount visitors
SeatNumbers (List<string>)	List of reserved seat numbers*/


using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies;

public readonly record struct BookingId(Guid Value) : IEntityId;

public sealed class Booking : Entity<BookingId>
{
    public Guid Id { get; private set; }
    public BookingStatus BookingStatus { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public int StandardVisitors { get; private set; }
    public int DiscountVisitors { get; private set; }
    public List<string> SeatNumbers { get; private set; }

    private Booking(
        Guid id,
        BookingStatus bookingStatus,
        PaymentStatus paymentStatus,
        int standardVisitors,
        int discountVisitors,
        List<string> seatNumbers)
    {
        Id = id;
        BookingStatus = bookingStatus;
        PaymentStatus = paymentStatus;
        StandardVisitors = standardVisitors;
        DiscountVisitors = discountVisitors;
        SeatNumbers = seatNumbers;
    }
    

    public static Booking Create(BookingStatus bookingStatus, PaymentStatus paymentStatus, int standardVisitors, int discountVisitors, List<string> seatNumbers)
    {
        if (standardVisitors < 0) throw new ArgumentException("Standard visitors cannot be negative.");
        if (discountVisitors < 0) throw new ArgumentException("Discount visitors cannot be negative.");
        if (seatNumbers == null || seatNumbers.Count == 0) throw new ArgumentException("At least one seat number must be provided.");

        return new Booking(
            id: Guid.NewGuid(),
            bookingStatus: bookingStatus,
            paymentStatus: paymentStatus,
            standardVisitors: standardVisitors,
            discountVisitors: discountVisitors,
            seatNumbers: seatNumbers
        );
    }

    public override void ValidateState()
    {
        throw new NotImplementedException();
    }
}