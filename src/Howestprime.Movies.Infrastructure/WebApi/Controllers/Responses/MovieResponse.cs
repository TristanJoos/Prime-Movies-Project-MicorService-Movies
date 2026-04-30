


public sealed record MovieResponse(
    Guid Id,
    string Title,
    string Description,
    int ReleaseYear,
    int Duration,
    List<string> Genres,
    List<string> Actors,
    int AgeRating,
    string PosterUrl
);