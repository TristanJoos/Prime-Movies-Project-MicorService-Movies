

public sealed record MovieEvent(
    Guid Id,
    Movie Movie,
    Room Room,
    DateTime Showtime,
    int Capacity
);