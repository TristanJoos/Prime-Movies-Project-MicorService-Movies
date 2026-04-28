using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Application.Movies;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Shared;

namespace UnitTests.Application.Movies;

public class RegisterMovieTests
{
    [Fact]
    public async Task Execute_WithValidInput_ShouldSaveMovieAndReturnId()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new RegisterMovie(fakeUow);

        var input = new RegisterMovieInput(
            Title: "Inception",
            Description: "A thief who steals corporate secrets...",
            Duration: 148,
            Genres: new[] { "Action", "Sci-Fi", "Thriller" },
            ReleaseYear: 2010,
            Actors: new[] { "Leonardo DiCaprio", "Joseph Gordon-Levitt" },
            AgeRating: 13,
            PosterUrl: "https://example.com/inception.jpg"
        );

        // Act
        var result = await sut.Execute(input);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
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
    public async Task Execute_WithInvalidInput_ShouldThrowExceptionAndNotSave()
    {
        // Arrange
        var fakeUow = new FakeUnitOfWork();
        var sut = new RegisterMovie(fakeUow);

        var invalidInput = new RegisterMovieInput(
            Title: "", // Invalid title
            Description: "A thief who steals corporate secrets...",
            Duration: 148,
            Genres: new[] { "Action" },
            ReleaseYear: 2010,
            Actors: new[] { "Leonardo DiCaprio" },
            AgeRating: 13,
            PosterUrl: "https://example.com/inception.jpg"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(invalidInput));
        Assert.False(fakeUow.IsDoCalled);
        Assert.Null(fakeUow.SavedAggregate);
    }

    // A simple fake unit of work to trace calls in the tests without needing a mocking framework
    private class FakeUnitOfWork : IUnitOfWork
    {
        public bool IsDoCalled { get; private set; }
        public IAggregateRoot? SavedAggregate { get; private set; }

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
            throw new NotImplementedException();
        }
    }
}