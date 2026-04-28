namespace Howestprime.Movies.Application.Contracts.Data;

public record MovieData(
    Guid id,
    string PosterUrl,
    string Title,
    IEnumerable<GenreData> Genres,
    IEnumerable<ActorData> Actors,
    int AgeRating,
    int ReleaseYear,
    int Duration,
    string Description
);