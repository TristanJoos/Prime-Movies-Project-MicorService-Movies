using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Xunit;

namespace UnitTests.Domain.Movies;

public class MovieEventTests
{
    private readonly MovieId _validMovieId = new MovieId(Guid.NewGuid());
    private readonly RoomId _validRoomId = new RoomId(Guid.NewGuid());
    private readonly DateTime _validShowtime;

    public MovieEventTests()
    {
        var now = DateTime.Now;
        _validShowtime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddDays(1);
    }

    [Fact]
    public void Create_WithValidData_ReturnsMovieEvent()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            100
        );

        Assert.NotNull(movieEvent);
        Assert.Equal(_validMovieId, movieEvent.MovieId);
        Assert.Equal(_validRoomId, movieEvent.RoomId);
        Assert.Equal(_validShowtime, movieEvent.Showtime);
        Assert.Equal(100, movieEvent.Capacity);
    }

    [Fact]
    public void Create_WithZeroCapacity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            0
        ));
    }

    [Fact]
    public void Create_InvalidShowtimeHour_ThrowsArgumentException()
    {
        var invalidHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 0, 0).AddDays(1);
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            invalidHour,
            100
        ));
    }

    [Fact]
    public void Create_PastShowtime_ThrowsArgumentException()
    {
        var pastShowtime = new DateTime(2000, 1, 1, 15, 0, 0);
        Assert.Throws<ArgumentException>(() => MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            pastShowtime,
            100
        ));
    }

    [Fact]
    public void UpdateMovie_WithValidMovieId_UpdatesMovieId()
    {
        var movieEvent = MovieEvent.Create(
            _validMovieId,
            _validRoomId,
            _validShowtime,
            100
        );

        var newMovieId = new MovieId(Guid.NewGuid());
        movieEvent.UpdateMovie(newMovieId);

        Assert.Equal(newMovieId, movieEvent.MovieId);
    }
}