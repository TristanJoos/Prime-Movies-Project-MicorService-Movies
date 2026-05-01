public class WhenPaymentSuccessCloseBookingController(CloseBooking useCase)
{
    public Task Handle(PaymentSuccess @event) 
        => useCase.Execute(new CloseBookingInput(@event.BookingId, "PaymentSuccess"));
}