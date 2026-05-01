using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;

public sealed record CloseBookingInput(Guid BookingId, string Reason);

public sealed class CloseBooking(IUnitOfWork uow) : IUseCase<CloseBookingInput>
{
    public async Task Execute(CloseBookingInput input)
    {
        if (input.BookingId == Guid.Empty)
        {
            throw new ArgumentException("Booking ID cannot be empty.");
        }

        if (input.Reason is not ("PaymentSuccess" or "PaymentFailed"))
        {
            throw new ArgumentException("Close booking reason must be PaymentSuccess or PaymentFailed.");
        }

        Optional<MovieEvent> movieEvent = await uow.Repo<IMovieEventRepository>().GetByBookingId(input.BookingId);
        if (!movieEvent.HasValue)
        {
            throw new ArgumentException("Movie event not found for the given booking ID.");
        }
        movieEvent.Value.CloseBooking(input.BookingId, input.Reason);

        await uow.Save<IMovieEventRepository>(movieEvent.Value);
        await uow.Do();
    }
}
