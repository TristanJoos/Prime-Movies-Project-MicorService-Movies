using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class PosterUrlTest
{
    [Fact]
    public void Create_WithValidUrl_ShouldReturnPosterUrl()
    {
        // Arrange
        const string validUrl = "https://example.com/poster.jpg";

        // Act
        var posterUrl = PosterUrl.Create(validUrl);

        // Assert
        Assert.NotNull(posterUrl);
        Assert.Equal(validUrl, posterUrl.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_NullOrEmptyUrl_ShouldThrowArgumentOutOfRangeException(string? invalidUrl)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => PosterUrl.Create(invalidUrl!));
    }
}