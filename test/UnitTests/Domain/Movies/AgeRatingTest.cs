using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class AgeRatingTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(13)]
    [InlineData(18)]
    public void Create_WithValidValue_ShouldReturnAgeRating(int validValue)
    {
        // Act
        var ageRating = AgeRating.Create(validValue);

        // Assert
        Assert.NotNull(ageRating);
        Assert.Equal(validValue, ageRating.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => AgeRating.Create(invalidValue));
    }
}