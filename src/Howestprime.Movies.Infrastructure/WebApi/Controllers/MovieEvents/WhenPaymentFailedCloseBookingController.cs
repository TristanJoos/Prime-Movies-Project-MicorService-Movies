public class WhenPaymentFailedCloseBookingController(CloseBooking useCase)
{
    public Task Handle(PaymentFailed @event) 
        => useCase.Execute(new CloseBookingInput(@event.BookingId, "PaymentFailed"));
}