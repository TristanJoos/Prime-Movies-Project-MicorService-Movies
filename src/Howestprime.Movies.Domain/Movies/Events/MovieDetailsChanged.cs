namespace Howestprime.Movies.Domain.Movies.Events;

public sealed class MovieDetailsChanged(
    MovieId movieId,
    string title,
    string description,
    int releaseYear,
    int duration,
    IEnumerable<string> genres,
    IEnumerable<string> actors,
    string ageRating,
    string posterUrl
) : MovieDomainEvent(nameof(MovieDetailsChanged))
{
    public MovieId MovieId { get; } = movieId;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public int ReleaseYear { get; } = releaseYear;
    public int Duration { get; } = duration;
    public IEnumerable<string> Genres { get; } = genres;
    public IEnumerable<string> Actors { get; } = actors;
    public string AgeRating { get; } = ageRating;
    public string PosterUrl { get; } = posterUrl;
}
