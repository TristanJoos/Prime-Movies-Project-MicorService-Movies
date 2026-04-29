namespace Howestprime.Movies.Application.Contracts.Data;

public record MovieData(
    Guid Id,
    string PosterUrl,
    string Title,
    IEnumerable<GenreData> Genres,
    IEnumerable<ActorData> Actors,
    int AgeRating,
    int ReleaseYear,
    int Duration,
    string Description
)
{
    public MovieData() : this(Guid.Empty, string.Empty, string.Empty, new List<GenreData>(), new List<ActorData>(), 0, 0, 0, string.Empty)
    {
    }
}