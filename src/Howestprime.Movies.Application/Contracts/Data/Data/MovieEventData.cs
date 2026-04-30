namespace Howestprime.Movies.Application.Contracts.Data;

public record MovieEventData(
    Guid Id,
    DateTime Showtime,
    int Capacity,
    RoomData Room,
    MovieData Movie
)
{
    public MovieEventData() : this(Guid.Empty, DateTime.MinValue, 0, new RoomData(), new MovieData())
    {
    }
}