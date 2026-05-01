using Aornis;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Shared.Exceptions;

namespace UnitTests.Application.Movies;

public class ChangeMovieDetailsTests
{
    [Fact]
    public async Task Execute_WithValidInput_ShouldUpdateMovieAndSave()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Action", "Thriller" },
            ReleaseYear: 2021,
            Actors: new[] { "Leonardo DiCaprio", "Tom Hardy" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act
        await sut.Execute(input);

        // Assert
        Assert.True(fakeUow.IsDoCalled);
        Assert.NotNull(fakeUow.SavedAggregate);

        var savedMovie = Assert.IsType<Movie>(fakeUow.SavedAggregate);
        Assert.Equal(input.Title, savedMovie.Title);
        Assert.Equal(input.Description, savedMovie.Description);
        Assert.Equal(input.Duration, savedMovie.Duration.Value);
        Assert.Equal(input.ReleaseYear, savedMovie.ReleaseYear.Value);
        Assert.Equal(input.AgeRating, savedMovie.AgeRating.Value);
        Assert.Equal(input.PosterUrl, savedMovie.PosterUrl.Value);
        Assert.Equal(input.Genres.Count(), savedMovie.Genres.Count);
        Assert.Equal(input.Actors.Count(), savedMovie.Actors.Count);
    }

    [Fact]
    public async Task Execute_WithNonExistentMovieId_ShouldThrowNotFoundException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var input = new ChangeMovieDetailsInput(
            MovieId: Guid.NewGuid(),
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Action" },
            ReleaseYear: 2021,
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => sut.Execute(input));
        Assert.Equal("Movie not found.", ex.Message);
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "", // Invalid
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Action" },
            ReleaseYear: 2021,
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "", // Invalid
            Duration: 150,
            Genres: new[] { "Action" },
            ReleaseYear: 2021,
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithNoGenres_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: Array.Empty<string>(), // Invalid
            ReleaseYear: 2021,
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidEntityStateException>(() => sut.Execute(input));
        Assert.Equal("A movie must have at least one genre.", ex.Message);
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithNoActors_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Action" },
            ReleaseYear: 2021,
            Actors: Array.Empty<string>(), // Invalid
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidEntityStateException>(() => sut.Execute(input));
        Assert.Equal("A movie must have at least one actor.", ex.Message);
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithInvalidDuration_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 0, // Invalid
            Genres: new[] { "Action" },
            ReleaseYear: 2021,
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidEntityStateException>(() => sut.Execute(input));
        Assert.Equal("Duration must be a positive integer.", ex.Message);
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithFutureReleaseYear_ShouldThrowInvalidEntityStateException()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Action" },
            ReleaseYear: DateTime.UtcNow.Year + 1, // Invalid
            Actors: new[] { "Actor 1" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidEntityStateException>(() => sut.Execute(input));
        Assert.Equal("Release year must be the current year or earlier.", ex.Message);
        Assert.False(fakeUow.IsDoCalled);
    }

    [Fact]
    public async Task Execute_WithValidInput_ShouldRaiseDomainEvent()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: new[] { "Thriller" },
            ReleaseYear: 2021,
            Actors: new[] { "New Actor" },
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act
        await sut.Execute(input);

        // Assert
        var savedMovie = Assert.IsType<Movie>(fakeUow.SavedAggregate);
        Assert.NotEmpty(savedMovie.DomainEvents);
        Assert.True(savedMovie.DomainEvents.Any(e => e.GetType().Name == "MovieDetailsChanged"));
    }

    [Fact]
    public async Task Execute_WithMultipleGenresAndActors_ShouldUpdateCorrectly()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new ChangeMovieDetails(fakeUow);

        var existingMovie = Movie.Create(
            "Original Title",
            "Original Description",
            ReleaseYear.Create(2020),
            Duration.Create(120),
            new[] { Genres.Create("Action") },
            new[] { Actors.Create("Actor 1") },
            AgeRating.Create(13),
            PosterUrl.Create("https://example.com/original.jpg")
        );

        fakeUow.SetMovieForRepository(existingMovie);

        var genres = new[] { "Action", "Thriller", "Drama", "Comedy" };
        var actors = new[] { "Actor 1", "Actor 2", "Actor 3", "Actor 4", "Actor 5" };

        var input = new ChangeMovieDetailsInput(
            MovieId: existingMovie.Id.Value,
            Title: "Updated Title",
            Description: "Updated Description",
            Duration: 150,
            Genres: genres,
            ReleaseYear: 2021,
            Actors: actors,
            AgeRating: 16,
            PosterUrl: "https://example.com/updated.jpg"
        );

        // Act
        await sut.Execute(input);

        // Assert
        var savedMovie = Assert.IsType<Movie>(fakeUow.SavedAggregate);
        Assert.Equal(genres.Length, savedMovie.Genres.Count);
        Assert.Equal(actors.Length, savedMovie.Actors.Count);
    }

    private class FakeUnitOfWork : IUnitOfWork
    {
        private Movie? _movieForRepository;
        public bool IsDoCalled { get; private set; }
        public IAggregateRoot? SavedAggregate { get; private set; }

        public void SetMovieForRepository(Movie movie)
        {
            _movieForRepository = movie;
        }

        public Task Do()
        {
            IsDoCalled = true;
            return Task.CompletedTask;
        }

        public Task Save<TRepository>(IAggregateRoot aggregateRoot) where TRepository : IRepository
        {
            SavedAggregate = aggregateRoot;
            return Task.CompletedTask;
        }

        public TRepository Repo<TRepository>() where TRepository : IRepository
        {
            if (typeof(TRepository).Name == "IMovieRepository")
            {
                return (TRepository)(object)new FakeMovieRepository(_movieForRepository);
            }
            throw new NotImplementedException();
        }
    }

    private class FakeMovieRepository : IMovieRepository
    {
        private readonly Movie? _movie;

        public FakeMovieRepository(Movie? movie)
        {
            _movie = movie;
        }

        public Task<Optional<Movie>> ById(MovieId id)
        {
            return Task.FromResult(
                _movie != null && _movie.Id.Equals(id)
                    ? new Optional<Movie>(_movie)
                    : new Optional<Movie>()
            );
        }

        public Task<bool> Exists(MovieId id)
        {
            return Task.FromResult(_movie != null && _movie.Id.Equals(id));
        }

        public Task Save(Movie aggregateRoot)
        {
            return Task.CompletedTask;
        }

        public Task Remove(Movie aggregateRoot)
        {
            return Task.CompletedTask;
        }
    }
}
