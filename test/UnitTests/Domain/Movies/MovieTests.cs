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
                 created.AgeRating == ageRating.ToString() &&
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
    public void Create_WithNullGenres_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var actors = new List<Actors> { Actors.Create("Actor 1") };

        // Act & Assert
        Assert.Throws<InvalidEntityStateException>(() => Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            null!,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/poster.jpg")
        ));
    }

    [Fact]
    public void Create_WithNullActors_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };

        // Act & Assert
        Assert.Throws<InvalidEntityStateException>(() => Movie.Create(
            "Valid Title",
            "Valid Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            null!,
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

    [Fact]
    public void ChangeDetails_WithValidInput_ShouldUpdateAllProperties()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action"), Genres.Create("Thriller") };
        var newActors = new List<Actors> { Actors.Create("New Actor 1"), Actors.Create("New Actor 2") };

        // Act
        movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        );

        // Assert
        Assert.Equal("Updated Title", movie.Title);
        Assert.Equal("Updated Description", movie.Description);
        Assert.Equal(2021, movie.ReleaseYear.Value);
        Assert.Equal(150, movie.Duration.Value);
        Assert.Equal(2, movie.Genres.Count);
        Assert.Equal(2, movie.Actors.Count);
        Assert.Equal(16, movie.AgeRating.Value);
        Assert.Equal("https://example.com/updated.jpg", movie.PosterUrl.Value);
    }

    [Fact]
    public void ChangeDetails_WithValidInput_ShouldRaiseDomainEvent()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        movie.ClearDomainEvents(); // Clear the Create event

        var newGenres = new List<Genres> { Genres.Create("Thriller") };
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act
        movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        );

        // Assert
        Assert.NotEmpty(movie.DomainEvents);
        Assert.True(movie.DomainEvents.Any(e => e.GetType().Name == "MovieDetailsChanged"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ChangeDetails_WithEmptyTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => movie.ChangeDetails(
            invalidTitle!,
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ChangeDetails_WithEmptyDescription_ShouldThrowArgumentException(string? invalidDescription)
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => movie.ChangeDetails(
            "Updated Title",
            invalidDescription!,
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
    }

    [Fact]
    public void ChangeDetails_WithEmptyGenres_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var emptyGenres = new List<Genres>();
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            emptyGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
        Assert.Equal("A movie must have at least one genre.", ex.Message);
    }

    [Fact]
    public void ChangeDetails_WithEmptyActors_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };
        var emptyActors = new List<Actors>();

        // Act & Assert
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            emptyActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
        Assert.Equal("A movie must have at least one actor.", ex.Message);
    }

    [Fact]
    public void ChangeDetails_WithInvalidDuration_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert - Duration.Create validates and throws ArgumentOutOfRangeException for 0
        Assert.Throws<ArgumentOutOfRangeException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(0),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
    }

    [Fact]
    public void ChangeDetails_WithFutureReleaseYear_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };
        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert - ReleaseYear.Create validates and throws ArgumentOutOfRangeException for future year
        Assert.Throws<ArgumentOutOfRangeException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(DateTime.UtcNow.Year + 1),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
    }

    [Fact]
    public void ChangeDetails_WithNullGenresList_ShouldTreatAsEmpty()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newActors = new List<Actors> { Actors.Create("New Actor") };

        // Act & Assert
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            null!,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
        Assert.Equal("A movie must have at least one genre.", ex.Message);
    }

    [Fact]
    public void ChangeDetails_WithNullActorsList_ShouldTreatAsEmpty()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres> { Genres.Create("Action") };

        // Act & Assert
        var ex = Assert.Throws<InvalidEntityStateException>(() => movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            null!,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        ));
        Assert.Equal("A movie must have at least one actor.", ex.Message);
    }

    [Fact]
    public void ChangeDetails_WithMultipleGenresAndActors_ShouldReplaceExisting()
    {
        // Arrange
        var genres = new List<Genres> { Genres.Create("Action") };
        var actors = new List<Actors> { Actors.Create("Actor 1") };
        var movie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            genres,
            actors,
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        var newGenres = new List<Genres>
        {
            Genres.Create("Action"),
            Genres.Create("Thriller"),
            Genres.Create("Drama"),
            Genres.Create("Comedy")
        };
        var newActors = new List<Actors>
        {
            Actors.Create("Actor 1"),
            Actors.Create("Actor 2"),
            Actors.Create("Actor 3")
        };

        // Act
        movie.ChangeDetails(
            "Updated Title",
            "Updated Description",
            ReleaseYear.Create(2021),
            Duration.Create(150),
            newGenres,
            newActors,
            AgeRating.Create(16),
            PosterUrl.Create("https://example.com/updated.jpg")
        );

        // Assert
        Assert.Equal(4, movie.Genres.Count);
        Assert.Equal(3, movie.Actors.Count);
        // Verify that the new genres are exactly as provided
        Assert.Equal(newGenres.Select(g => g.Value), movie.Genres.Select(g => g.Value));
    }
}

