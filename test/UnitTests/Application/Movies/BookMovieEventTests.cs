using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Shared.Exceptions;
using Xunit;
using Aornis;
using System.Reflection;

namespace UnitTests.Application.Movies;

public class BookMovieEventTests
{
    private class FakeUnitOfWork : IUnitOfWork, IRoomRepository, IMovieEventRepository
    {
        public bool IsDoCalled { get; private set; }
        public IAggregateRoot? SavedAggregate { get; private set; }

        public Room? StubRoom { get; set; }
        public MovieEvent? StubMovieEvent { get; set; }

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

        Task<bool> IRepository<Room, RoomId>.Exists(RoomId id) => Task.FromResult(StubRoom != null && StubRoom.Id == id);
        Task<Optional<Room>> IRepository<Room, RoomId>.ById(RoomId id) => Task.FromResult(StubRoom != null && StubRoom.Id == id ? Optional.Of(StubRoom) : Optional.Of<Room>(null));
        Task IRepository<Room, RoomId>.Save(Room aggregateRoot) => Task.CompletedTask;
        Task IRepository<Room, RoomId>.Remove(Room aggregateRoot) => Task.CompletedTask;

        Task<bool> IRepository<MovieEvent, MovieEventId>.Exists(MovieEventId id) => Task.FromResult(StubMovieEvent != null && StubMovieEvent.Id == id);
        Task<Optional<MovieEvent>> IRepository<MovieEvent, MovieEventId>.ById(MovieEventId id) => Task.FromResult(StubMovieEvent != null && StubMovieEvent.Id == id ? Optional.Of(StubMovieEvent) : Optional.Of<MovieEvent>(null));
        Task IRepository<MovieEvent, MovieEventId>.Save(MovieEvent aggregateRoot) => Task.CompletedTask;
        Task IRepository<MovieEvent, MovieEventId>.Remove(MovieEvent aggregateRoot) => Task.CompletedTask;

        public Task<MovieEvent?> ByShowtimeAndRoomId(DateTime showtime, Guid roomId) => Task.FromResult<MovieEvent?>(null);
        public Task<MovieEvent?> GetById(Guid movieEventId) => Task.FromResult<MovieEvent?>(null);
    }

    [Fact]
    public async Task Execute_NegativeVisitors_ThrowsArgumentException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);
        var input = new BookMovieEventInput(Guid.NewGuid(), -1, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_EmptyMovieEventId_ThrowsArgumentException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);
        var input = new BookMovieEventInput(Guid.Empty, 1, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_MovieEventNotFound_ThrowsNotFoundException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);
        var input = new BookMovieEventInput(Guid.NewGuid(), 1, 0);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_NotEnoughSeats_ThrowsInvalidOperationException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);

        var id = new MovieEventId(Guid.NewGuid());
        var movieEvent = MovieEvent.Create(new MovieId(Guid.NewGuid()), new RoomId(Guid.NewGuid()), DateTime.UtcNow.Date.AddDays(1).AddHours(15), 5);
        SetEntityId(movieEvent, id);
        uow.StubMovieEvent = movieEvent;

        var input = new BookMovieEventInput(id.Value, 10, 0);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_RoomNotFound_ThrowsNotFoundException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);

        var roomId = new RoomId(Guid.NewGuid());
        var id = new MovieEventId(Guid.NewGuid());
        var movieEvent = MovieEvent.Create(new MovieId(Guid.NewGuid()), roomId, DateTime.UtcNow.Date.AddDays(1).AddHours(15), 10);
        SetEntityId(movieEvent, id);

        uow.StubMovieEvent = movieEvent;
        uow.StubRoom = null;

        var input = new BookMovieEventInput(id.Value, 2, 0);

        await Assert.ThrowsAsync<NotFoundException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShowtimeInPast_ThrowsInvalidOperationException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);

        var id = new MovieEventId(Guid.NewGuid());
        var roomId = new RoomId(Guid.NewGuid());
        var showtime = DateTime.UtcNow.AddSeconds(-1);
        var movieEvent = CreateMovieEventWithShowtime(id, roomId, showtime);

        var room = Room.Create(roomId, "Room 1", 100);

        uow.StubMovieEvent = movieEvent;
        uow.StubRoom = room;

        var input = new BookMovieEventInput(id.Value, 2, 0);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ShowtimeTooFarInFuture_ThrowsInvalidOperationException()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);

        var id = new MovieEventId(Guid.NewGuid());
        var roomId = new RoomId(Guid.NewGuid());
        var showtime = DateTime.UtcNow.AddDays(15);
        var movieEvent = CreateMovieEventWithShowtime(id, roomId, showtime);

        var room = Room.Create(roomId, "Room 1", 100);

        uow.StubMovieEvent = movieEvent;
        uow.StubRoom = room;

        var input = new BookMovieEventInput(id.Value, 2, 0);

        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ValidBooking_ReturnsBookingId()
    {
        var uow = new FakeUnitOfWork();
        var sut = new BookMovieEvent(uow);

        var id = new MovieEventId(Guid.NewGuid());
        var roomId = new RoomId(Guid.NewGuid());
        var showtime = DateTime.UtcNow.Date.AddDays(1).AddHours(15);
        var movieEvent = MovieEvent.Create(new MovieId(Guid.NewGuid()), roomId, showtime, 10);
        SetEntityId(movieEvent, id);

        var room = Room.Create(roomId, "Room 1", 100);

        uow.StubMovieEvent = movieEvent;
        uow.StubRoom = room;

        var input = new BookMovieEventInput(id.Value, 2, 0);

        var result = await sut.Execute(input);

        Assert.NotEqual(Guid.Empty, result);
        Assert.True(uow.IsDoCalled);
        Assert.NotNull(uow.SavedAggregate);
        Assert.Single(movieEvent.Bookings);
    }

    private MovieEvent CreateMovieEventWithShowtime(MovieEventId id, RoomId roomId, DateTime showtime)
    {
        var movieEvent = (MovieEvent)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(MovieEvent));

        SetEntityId(movieEvent, id);

        var type = typeof(MovieEvent);
        type.GetProperty("MovieId")!.SetValue(movieEvent, new MovieId(Guid.NewGuid()));
        type.GetProperty("RoomId")!.SetValue(movieEvent, roomId);
        type.GetProperty("Showtime")!.SetValue(movieEvent, showtime);
        type.GetProperty("Capacity")!.SetValue(movieEvent, 10);
        type.GetProperty("Bookings")!.SetValue(movieEvent, new List<Booking>());

        return movieEvent;
    }

    private void SetEntityId(Entity<MovieEventId> entity, MovieEventId id)
    {
        var prop = typeof(Entity<MovieEventId>).GetProperty("Id");
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(entity, id);
        }
        else
        {
            var field = typeof(Entity<MovieEventId>).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)?.DeclaringType?.GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                field = typeof(Entity<MovieEventId>).GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            if (field != null)
                field.SetValue(entity, id);
        }
    }
}
