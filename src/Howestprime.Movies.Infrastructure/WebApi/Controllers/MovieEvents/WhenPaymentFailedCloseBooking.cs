using Microsoft.Extensions.Logging;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Shared.Contracts;
using Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Shared.Messages;

namespace Howestprime.Movies.Infrastructure.Messaging.IntegrationEvents.Controllers;

public sealed class WhenPaymentFailedCloseBooking(
    IUseCase<CloseBookingInput> closeBooking, 
    ILogger<WhenPaymentFailedCloseBooking> logger
) : IController<ConsumerContext>
{
    public async Task Handle(ConsumerContext context)
    {
        logger.LogInformation("Invoking {Controller} for message {Message}", nameof(WhenPaymentFailedCloseBooking), context.Message);
        
        // Parse using the Taskly-style converter
        PaymentFailed body = AmqpMessageConverter.ParseBody<PaymentFailed>(context);
        
        await closeBooking.Execute(new CloseBookingInput(body.BookingId, "PaymentFailed"));
    }
}