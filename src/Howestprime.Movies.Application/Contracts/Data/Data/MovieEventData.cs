namespace Howestprime.Movies.Application.Contracts.Data;

public record MovieEventData{
    public required Guid Id { get; init; }
    public required DateTime Showtime { get; init; }
    public required int Capacity { get; init; }
    public required RoomData Room { get; init; }
    public required MovieData Movie { get; init; }
}


