using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies;

public readonly record struct BookingId(Guid Value) : IEntityId;

public sealed class Booking : Entity<BookingId>
{
    public BookingStatus BookingStatus { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public int StandardVisitors { get; private set; }
    public int DiscountVisitors { get; private set; }
    public IList<string> SeatNumbers { get; private set; } = new List<string>();

    private Booking(
        BookingId id,
        int standardVisitors,
        int discountVisitors ) : base(id)
    {
        BookingStatus = BookingStatus.open;
        PaymentStatus = PaymentStatus.pending;
        StandardVisitors = standardVisitors;
        DiscountVisitors = discountVisitors;
    }
    

    public static Booking Create( int standardVisitors, int discountVisitors)
    {
        if (standardVisitors < 0) throw new ArgumentException("Standard visitors cannot be negative.");
        if (discountVisitors < 0) throw new ArgumentException("Discount visitors cannot be negative.");
        return new Booking(
            id: EntityId.New<BookingId>(),
            standardVisitors: standardVisitors,
            discountVisitors: discountVisitors
        );
    }

    public override void ValidateState()
    {
        throw new NotImplementedException();
    }
}