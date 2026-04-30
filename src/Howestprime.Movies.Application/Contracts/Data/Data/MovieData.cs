namespace Howestprime.Movies.Application.Contracts.Data;

public record MovieData{
    public required Guid Id { get; init; }
    public required string PosterUrl { get; init; }
    public required string Title { get; init; }
    public required IEnumerable<GenreData> Genres { get; init; }
    public required IEnumerable<ActorData> Actors { get; init; }
    public required int AgeRating { get; init; }
    public required int ReleaseYear { get; init; }
    public required int Duration { get; init; }
    public required string Description { get; init; }
}
