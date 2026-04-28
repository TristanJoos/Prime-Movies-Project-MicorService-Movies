using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class GenresTest
{
    [Fact]
    public void Create_WithValidName_ShouldReturnGenres()
    {
        // Arrange
        const string genreName = "Action";

        // Act
        var genre = Genres.Create(genreName);

        // Assert
        Assert.NotNull(genre);
        Assert.Equal(genreName, genre.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_NullOrEmptyName_ShouldThrowArgumentOutOfRangeException(string? invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Genres.Create(invalidName!));
    }
}