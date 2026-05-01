using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Application.Movies;

public sealed class CloseBookingTests
{
    private sealed class FakeUnitOfWork : IUnitOfWork, IMovieEventRepository
    {
        public MovieEvent? StubMovieEvent { get; set; }
        public IAggregateRoot? SavedAggregate { get; private set; }
        public bool IsDoCalled { get; private set; }

        public Task Do()
        {
            IsDoCalled = true;
            return Task.CompletedTask;
        }

        public Task Save<TRepository>(IAggregateRoot aggregateRoot) where TRepository : IRepository
        {
            SavedAggregate = aggregateRoot;
            return Task.CompletedTask;
        }

        public TRepository Repo<TRepository>() where TRepository : IRepository
        {
            return (TRepository)(object)this;
        }

        public Task<MovieEvent?> ByShowtimeAndRoomId(DateTime showtime, Guid roomId)
        {
            return Task.FromResult<MovieEvent?>(null);
        }

        public Task<MovieEvent?> GetByBookingId(Guid bookingId)
        {
            bool hasBooking = StubMovieEvent?.Bookings.Any(booking => booking.Id.Value == bookingId) == true;
            return Task.FromResult(hasBooking ? StubMovieEvent : null);
        }

        Task<bool> IRepository<MovieEvent, MovieEventId>.Exists(MovieEventId id)
        {
            return Task.FromResult(StubMovieEvent?.Id == id);
        }

        Task<Optional<MovieEvent>> IRepository<MovieEvent, MovieEventId>.ById(MovieEventId id)
        {
            return Task.FromResult(StubMovieEvent?.Id == id
                ? Optional.Of(StubMovieEvent)
                : Optional.Of<MovieEvent>(null));
        }

        Task IRepository<MovieEvent, MovieEventId>.Save(MovieEvent aggregateRoot)
        {
            return Task.CompletedTask;
        }

        Task IRepository<MovieEvent, MovieEventId>.Remove(MovieEvent aggregateRoot)
        {
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Execute_WithPaymentSuccess_ClosesBookingSavesAndCommits()
    {
        var uow = new FakeUnitOfWork();
        var movieEvent = CreateMovieEventWithBooking(out var booking);
        uow.StubMovieEvent = movieEvent;
        var sut = new CloseBooking(uow);

        await sut.Execute(new CloseBookingInput(booking.Id.Value, "PaymentSuccess"));

        Assert.Equal(BookingStatus.closed, booking.BookingStatus);
        Assert.Equal(PaymentStatus.success, booking.PaymentStatus);
        Assert.Same(movieEvent, uow.SavedAggregate);
        Assert.True(uow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithPaymentFailed_ClosesBookingAndReleasesVisitors()
    {
        var uow = new FakeUnitOfWork();
        var movieEvent = CreateMovieEventWithBooking(out var booking);
        uow.StubMovieEvent = movieEvent;
        var sut = new CloseBooking(uow);

        await sut.Execute(new CloseBookingInput(booking.Id.Value, "PaymentFailed"));

        Assert.Equal(BookingStatus.closed, booking.BookingStatus);
        Assert.Equal(PaymentStatus.failed, booking.PaymentStatus);
        Assert.Equal(0, movieEvent.Visitors);
        Assert.Empty(booking.SeatNumbers);
        Assert.Same(movieEvent, uow.SavedAggregate);
        Assert.True(uow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithEmptyBookingId_ThrowsArgumentException()
    {
        var sut = new CloseBooking(new FakeUnitOfWork());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.Execute(new CloseBookingInput(Guid.Empty, "PaymentSuccess")));
    }

    [Fact]
    public async Task Execute_WithInvalidReason_ThrowsArgumentException()
    {
        var sut = new CloseBooking(new FakeUnitOfWork());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.Execute(new CloseBookingInput(Guid.NewGuid(), "Canceled")));
    }

    [Fact]
    public async Task Execute_WhenMovieEventIsNotFound_ThrowsArgumentException()
    {
        var sut = new CloseBooking(new FakeUnitOfWork());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            sut.Execute(new CloseBookingInput(Guid.NewGuid(), "PaymentSuccess")));
    }

    private static MovieEvent CreateMovieEventWithBooking(out Booking booking)
    {
        var showtime = DateTime.Now.Date.AddDays(1).AddHours(15);
        var movieEvent = MovieEvent.Create(
            new MovieId(Guid.NewGuid()),
            new RoomId(Guid.NewGuid()),
            showtime,
            10
        );

        booking = Booking.Create(1, 1);
        movieEvent.Book(booking, "Room 1");

        return movieEvent;
    }
}
