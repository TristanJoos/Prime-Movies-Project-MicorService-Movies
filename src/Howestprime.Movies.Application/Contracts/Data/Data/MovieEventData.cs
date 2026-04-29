/*Id (uuid)	Unique identifier
Showtime (datetime)	Date and time of the event
Capacity (int)	Room capacity
Room (RoomData)	Room data
Movie (MovieData)	Movie data
*/

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