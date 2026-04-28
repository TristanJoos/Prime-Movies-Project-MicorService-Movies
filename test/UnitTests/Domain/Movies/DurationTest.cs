using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class DurationTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(120)]
    [InlineData(500)]
    public void Create_WithValidValue_ShouldReturnDuration(int validValue)
    {
        // Act
        var duration = Duration.Create(validValue);

        // Assert
        Assert.NotNull(duration);
        Assert.Equal(validValue, duration.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Duration.Create(invalidValue));
    }
}