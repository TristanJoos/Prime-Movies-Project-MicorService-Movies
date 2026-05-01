using Microsoft.Extensions.Logging;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Shared.Contracts;
using Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Shared.Messages;

namespace Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Controllers;

public sealed class WhenPaymentSuccessCloseBooking(
    IUseCase<CloseBookingInput> closeBooking,
    ILogger<WhenPaymentSuccessCloseBooking> logger
) : IController<ConsumerContext>
{
    public async Task Handle(ConsumerContext context)
    {
        logger.LogInformation("Invoking {Controller}", nameof(WhenPaymentSuccessCloseBooking));
        PaymentSuccess body = AmqpMessageConverter.ParseBody<PaymentSuccess>(context);
        await closeBooking.Execute(new CloseBookingInput(body.BookingId, "PaymentSuccess"));
    }
}