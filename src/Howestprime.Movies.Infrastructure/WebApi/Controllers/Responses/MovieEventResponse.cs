

public sealed record MovieEventResponse(
    Guid Id,
    MovieResponse Movie,
    RoomResponse Room,
    DateTime Showtime,
    int Capacity
);