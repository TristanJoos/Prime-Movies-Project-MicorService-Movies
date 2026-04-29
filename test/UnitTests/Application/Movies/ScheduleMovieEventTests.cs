using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Domain.Shared;
using Xunit;

namespace UnitTests.Application.Movies;

public class ScheduleMovieEventTests
{
    private class FakeUnitOfWork : IUnitOfWork, IRoomRepository, IMovieRepository, IMovieEventRepository
    {
        public bool IsDoCalled { get; private set; }
        public IAggregateRoot? SavedAggregate { get; private set; }

        public Room? StubRoom { get; set; }
        public bool StubMovieExists { get; set; }
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
        Task<Optional<Room>> IRepository<Room, RoomId>.ById(RoomId id) => Task.FromResult(Optional.Of(StubRoom));
        Task IRepository<Room, RoomId>.Save(Room aggregateRoot) => Task.CompletedTask;
        Task IRepository<Room, RoomId>.Remove(Room aggregateRoot) => Task.CompletedTask;

        Task<bool> IRepository<Movie, MovieId>.Exists(MovieId id) => Task.FromResult(StubMovieExists);
        Task<Optional<Movie>> IRepository<Movie, MovieId>.ById(MovieId id) => Task.FromResult(Optional.Of<Movie>(null));
        Task IRepository<Movie, MovieId>.Save(Movie aggregateRoot) => Task.CompletedTask;
        Task IRepository<Movie, MovieId>.Remove(Movie aggregateRoot) => Task.CompletedTask;

        Task<bool> IRepository<MovieEvent, MovieEventId>.Exists(MovieEventId id) => Task.FromResult(StubMovieEvent != null && StubMovieEvent.Id == id);
        Task<Optional<MovieEvent>> IRepository<MovieEvent, MovieEventId>.ById(MovieEventId id) => Task.FromResult(Optional.Of(StubMovieEvent));
        Task IRepository<MovieEvent, MovieEventId>.Save(MovieEvent aggregateRoot) => Task.CompletedTask;
        Task IRepository<MovieEvent, MovieEventId>.Remove(MovieEvent aggregateRoot) => Task.CompletedTask;

        public Task<MovieEvent?> ByShowtimeAndRoomId(DateTime showtime, Guid roomId)
        {
            if (StubMovieEvent != null && StubMovieEvent.Showtime == showtime && StubMovieEvent.RoomId.Value == roomId)
                return Task.FromResult<MovieEvent?>(StubMovieEvent);
            return Task.FromResult<MovieEvent?>(null);
        }
    }

    [Fact]
    public async Task Execute_WithValidInputAndNoExistingEvent_ShouldCreateAndSaveMovieEvent()
    {
        var fakeUow = new FakeUnitOfWork();
        var sut = new ScheduleMovieEvent(fakeUow);

        var roomId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var now = DateTime.Now;
        var showtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);

        fakeUow.StubRoom = Room.Create(new RoomId(roomId), "Test Room", 100);
        fakeUow.StubMovieExists = true;
        fakeUow.StubMovieEvent = null;

        var input = new ScheduleMovieEventInput(
            MovieId: movieId,
            RoomId: roomId,
            Showtime: showtime
        );

        var result = await sut.Execute(input);

        Assert.True(fakeUow.IsDoCalled);
        Assert.NotNull(fakeUow.SavedAggregate);
        var savedEvent = Assert.IsType<MovieEvent>(fakeUow.SavedAggregate);
        Assert.Equal(movieId, savedEvent.MovieId.Value);
        Assert.Equal(roomId, savedEvent.RoomId.Value);
        Assert.Equal(showtime, savedEvent.Showtime);
        Assert.Equal(100, savedEvent.Capacity);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Execute_WithExistingEvent_ShouldUpdateMovieEventInsteadOfCreatingNew()
    {
        var fakeUow = new FakeUnitOfWork();
        var sut = new ScheduleMovieEvent(fakeUow);

        var roomId = Guid.NewGuid();
        var oldMovieId = Guid.NewGuid();
        var newMovieId = Guid.NewGuid();
        var now = DateTime.Now;
        var showtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);

        fakeUow.StubRoom = Room.Create(new RoomId(roomId), "Test Room", 100);
        fakeUow.StubMovieExists = true;

        var existingEvent = MovieEvent.Create(new MovieId(oldMovieId), new RoomId(roomId), showtime, 100);
        fakeUow.StubMovieEvent = existingEvent;

        var input = new ScheduleMovieEventInput(
            MovieId: newMovieId,
            RoomId: roomId,
            Showtime: showtime
        );

        var result = await sut.Execute(input);

        Assert.True(fakeUow.IsDoCalled);
        Assert.Null(fakeUow.SavedAggregate); // No new instance saved conceptually through Save method since it already exists
        Assert.Equal(newMovieId, existingEvent.MovieId.Value);
        Assert.Equal(existingEvent.Id.Value, result);
    }

    [Fact]
    public async Task Execute_RoomNotFound_ThrowsInvalidOperationException()
    {
        var fakeUow = new FakeUnitOfWork();
        var sut = new ScheduleMovieEvent(fakeUow);

        var roomId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var now = DateTime.Now;
        var showtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);

        fakeUow.StubRoom = null;
        fakeUow.StubMovieExists = true;

        var input = new ScheduleMovieEventInput(
            MovieId: movieId,
            RoomId: roomId,
            Showtime: showtime
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
        Assert.Equal("Room not found", exception.Message);
    }

    [Fact]
    public async Task Execute_MovieNotFound_ThrowsInvalidOperationException()
    {
        var fakeUow = new FakeUnitOfWork();
        var sut = new ScheduleMovieEvent(fakeUow);

        var roomId = Guid.NewGuid();
        var movieId = Guid.NewGuid();
        var now = DateTime.Now;
        var showtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);

        fakeUow.StubRoom = Room.Create(new RoomId(roomId), "Test Room", 100);
        fakeUow.StubMovieExists = false;

        var input = new ScheduleMovieEventInput(
            MovieId: movieId,
            RoomId: roomId,
            Showtime: showtime
        );

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(input));
        Assert.Equal("Movie not found", exception.Message);
    }
}