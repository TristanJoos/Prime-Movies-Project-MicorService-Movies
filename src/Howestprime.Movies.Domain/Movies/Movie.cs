using Howestprime.Movies.Domain.Movies.Events;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies;

public readonly record struct MovieId(Guid Value) : IEntityId;

public sealed class Movie : AggregateRoot<MovieId>
{
    private readonly List<Genres> _genres = new();
    private readonly List<Actors> _actors = new();

    public string Title { get; private set; }
    public string Description { get; private set; }
    public ReleaseYear ReleaseYear { get; private set; }
    public Duration Duration { get; private set; }
    public AgeRating AgeRating { get; private set; }
    public PosterUrl PosterUrl { get; private set; }

    public IReadOnlyCollection<Genres> Genres => _genres.AsReadOnly();
    public IReadOnlyCollection<Actors> Actors => _actors.AsReadOnly();

    public static Movie Create(
        string title,
        string description,
        ReleaseYear releaseYear,
        Duration duration,
        IEnumerable<Genres> genres,
        IEnumerable<Actors> actors,
        AgeRating ageRating,
        PosterUrl posterUrl)
    {

        Asserts.EnsureNotEmpty(title);
        Asserts.EnsureNotEmpty(description);

        var genreList = genres?.ToList() ?? new List<Genres>();
        var actorList = actors?.ToList() ?? new List<Actors>();

        if (!genreList.Any()) throw new InvalidEntityStateException("A movie must have at least one genre.");
        if (!actorList.Any()) throw new InvalidEntityStateException("A movie must have at least one actor.");

        var movie = new Movie(
            EntityId.New<MovieId>(),
            title,
            description,
            releaseYear,
            duration,
            genreList,
            actorList,
            ageRating,
            posterUrl
        );

        movie.RaiseDomainEvent(new MovieRegistered(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.ReleaseYear.Value,
            movie.Duration.Value,
            movie.Genres.Select(g => g.Value).ToList(),
            movie.Actors.Select(a => a.Value).ToList(),
            movie.AgeRating.Value.ToString(),
            movie.PosterUrl.Value
        ));

        return movie;
    }

    private Movie(
        MovieId id,
        string title,
        string description,
        ReleaseYear releaseYear,
        Duration duration,
        IEnumerable<Genres> genres,
        IEnumerable<Actors> actors,
        AgeRating ageRating,
        PosterUrl posterUrl) : base(id)
    {
        Title = title;
        Description = description;
        ReleaseYear = releaseYear;
        Duration = duration;
        _genres.AddRange(genres);
        _actors.AddRange(actors);
        AgeRating = ageRating;
        PosterUrl = posterUrl;

        ValidateState();
    }

    // Required for ORM / Serialization
#pragma warning disable CS8618 
    private Movie() : base(default!) { }
#pragma warning restore CS8618

    public override void ValidateState()
    {
        Asserts.EnsureNotEmpty(Title);
        Asserts.EnsureNotEmpty(Description);


        if (Duration.Value <= 0)
            throw new InvalidEntityStateException("Duration must be a positive integer.");


        if (ReleaseYear.Value > DateTime.UtcNow.Year)
            throw new InvalidEntityStateException("Release year must be the current year or earlier.");

        if (!_genres.Any())
            throw new InvalidEntityStateException("A movie must have at least one genre.");

        if (!_actors.Any())
            throw new InvalidEntityStateException("A movie must have at least one actor.");
    }

    public void UpdatePoster(PosterUrl newUrl)
    {
        PosterUrl = newUrl ?? throw new ArgumentNullException(nameof(newUrl));
        ValidateState();
    }


    public void ChangeDetails(
    string title,
    string description,
    ReleaseYear releaseYear,
    Duration duration,
    IEnumerable<Genres> genres,
    IEnumerable<Actors> actors,
    AgeRating ageRating,
    PosterUrl posterUrl)
    {
        Asserts.EnsureNotEmpty(title);
        Asserts.EnsureNotEmpty(description);
        var genreList = genres?.ToList() ?? new List<Genres>();
        var actorList = actors?.ToList() ?? new List<Actors>();
        if (!genreList.Any()) throw new InvalidEntityStateException("A movie must have at least one genre.");
        if (!actorList.Any()) throw new InvalidEntityStateException("A movie must have at least one actor.");

        Title = title;
        Description = description;
        ReleaseYear = releaseYear;
        Duration = duration;

        _genres.Clear();
        _genres.AddRange(genreList);

        _actors.Clear();
        _actors.AddRange(actorList);

        AgeRating = ageRating;
        PosterUrl = posterUrl;

        ValidateState();

        RaiseDomainEvent(new MovieDetailsChanged(
            Id,
            Title,
            Description,
            ReleaseYear.Value,
            Duration.Value,
            Genres.Select(g => g.Value).ToList(),
            Actors.Select(a => a.Value).ToList(),
            AgeRating.Value.ToString(),
            PosterUrl.Value
        ));
    }
}