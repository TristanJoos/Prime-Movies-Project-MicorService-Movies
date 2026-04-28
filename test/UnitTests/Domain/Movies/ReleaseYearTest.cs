using System;
using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class ReleaseYearTest
{
    [Theory]
    [InlineData(1888)]
    [InlineData(1995)]
    [InlineData(2020)]
    public void Create_WithValidYear_ShouldReturnReleaseYear(int validYear)
    {
        // Act
        var releaseYear = ReleaseYear.Create(validYear);

        // Assert
        Assert.NotNull(releaseYear);
        Assert.Equal(validYear, releaseYear.Value);
    }

    [Fact]
    public void Create_WithCurrentYear_ShouldReturnReleaseYear()
    {
        // Arrange
        int currentYear = DateTime.Now.Year;

        // Act
        var releaseYear = ReleaseYear.Create(currentYear);

        // Assert
        Assert.NotNull(releaseYear);
        Assert.Equal(currentYear, releaseYear.Value);
    }

    [Fact]
    public void Create_WithFutureYear_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        int futureYear = DateTime.Now.Year + 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => ReleaseYear.Create(futureYear));
    }
}