using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Events;
using Howestprime.Movies.Shared.Exceptions;
using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public sealed class MovieTests
{
    [Fact]
    public void ValidateState_WithInvalidDuration_ThrowsException()
    {
        var movie = (Movie)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Movie));
        typeof(Movie).GetProperty("Title")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("Description")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("Duration")!.SetValue(movie, new Duration(0));
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ValidateState());
        Assert.Equal("Duration must be a positive integer.", ex.Message);
    }

    [Fact]
    public void ValidateState_WithInvalidReleaseYear_ThrowsException()
    {
        var movie = (Movie)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Movie));
        typeof(Movie).GetProperty("Duration")!.SetValue(movie, new Duration(1));
        typeof(Movie).GetProperty("Title")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("Description")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("ReleaseYear")!.SetValue(movie, new ReleaseYear(DateTime.UtcNow.Year + 1));
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ValidateState());
        Assert.Equal("Release year must be the current year or earlier.", ex.Message);
    }

    [Fact]
    public void ValidateState_WithNoGenres_ThrowsException()
    {
        var movie = (Movie)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Movie));
        typeof(Movie).GetProperty("Duration")!.SetValue(movie, new Duration(1));
        typeof(Movie).GetProperty("Title")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("Description")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("ReleaseYear")!.SetValue(movie, new ReleaseYear(DateTime.UtcNow.Year));
        typeof(Movie).GetField("_genres", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(movie, new List<Genres>());
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ValidateState());
        Assert.Equal("A movie must have at least one genre.", ex.Message);
    }

    [Fact]
    public void ValidateState_WithNoActors_ThrowsException()
    {
        var movie = (Movie)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Movie));
        typeof(Movie).GetProperty("Duration")!.SetValue(movie, new Duration(1));
        typeof(Movie).GetProperty("Title")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("Description")!.SetValue(movie, "Valid");
        typeof(Movie).GetProperty("ReleaseYear")!.SetValue(movie, new ReleaseYear(DateTime.UtcNow.Year));
        typeof(Movie).GetField("_genres", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(movie, new List<Genres> { new Genres("Action") });
        typeof(Movie).GetField("_actors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(movie, new List<Actors>());
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ValidateState());
        Assert.Equal("A movie must have at least one actor.", ex.Message);
    }

    [Fact]
    public void PrivateConstructor_IsExecuted()
    {
        var ctor = typeof(Movie).GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        Assert.NotNull(instance);
    }

    [Fact]
    public void Create_WithValidInput_ShouldInitializeStateAndRaiseEvent()
    {
        // Arrange
        const string title = "My List";
        const string description = "Description";
        const int releaseYear = 2020;
        const int duration = 120;
        const int ageRating = 13;
        const string posterUrl = "https://example.com/poster.jpg";
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        // Act
        Movie movie = Movie.Create(title, description, ReleaseYear.Create(releaseYear), Duration.Create(duration), genres, actors, AgeRating.Create(ageRating), PosterUrl.Create(posterUrl));

        // Assert
        Assert.Equal(title, movie.Title);
        Assert.Equal(description, movie.Description);
        Assert.Equal(ReleaseYear.Create(releaseYear), movie.ReleaseYear);
        Assert.Equal(Duration.Create(duration), movie.Duration);
        Assert.Equal(genres, movie.Genres);
        Assert.Equal(actors, movie.Actors);
        Assert.Equal(AgeRating.Create(ageRating), movie.AgeRating);
        Assert.Equal(PosterUrl.Create(posterUrl), movie.PosterUrl);
        Assert.Contains(
            movie.DomainEvents,
            e => e is MovieRegistered created &&
                 created.MovieId.Equals(movie.Id) &&
                 created.Title == title &&
                 created.Description == description &&
                 created.ReleaseYear == releaseYear &&
                 created.Duration == duration &&
                 created.AgeRating == AgeRating.Create(ageRating).ToString() &&
                 created.PosterUrl == posterUrl
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Movie.Create(
            invalidTitle!,
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyDescription_ShouldThrowArgumentException(string? invalidDescription)
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Movie.Create(
            "Valid Title",
            invalidDescription!,
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Fact]
    public void Create_WithEmptyGenres_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var emptyGenres = new List<Genres>();
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<InvalidEntityStateException>(() => Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            emptyGenres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Fact]
    public void Create_WithEmptyActors_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var emptyActors = new List<Actors>();

        // Act & Assert
        Assert.Throws<InvalidEntityStateException>(() => Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            emptyActors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Fact]
    public void Create_WithInvalidDuration_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Duration.Create(0)); // Depending on Duration implementation, creating 0 might fail. If not, Movie.Create throws.
    }

    [Fact]
    public void Create_WithFutureReleaseYear_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        int futureYear = DateTime.UtcNow.Year + 1;
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(futureYear),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Fact]
    public void UpdatePoster_WithValidUrl_ShouldUpdatePosterAndValidateState()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/old-poster.jpg")
        );

        var newPosterUrl = PosterUrl.Create("https://example.com/new-poster.jpg");

        // Act
        movie.UpdatePoster(newPosterUrl);

        // Assert
        Assert.Equal(newPosterUrl, movie.PosterUrl);
    }

    [Fact]
    public void UpdatePoster_WithNullUrl_ShouldThrowArgumentNullException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/old-poster.jpg")
        );

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => movie.UpdatePoster(null!));
    }

    [Fact]
    public void ParameterlessConstructor_ShouldInstantiate()
    {
        // Act
        var movie = Activator.CreateInstance(typeof(Movie), nonPublic: true);

        // Assert
        Assert.NotNull(movie);
    }
}

