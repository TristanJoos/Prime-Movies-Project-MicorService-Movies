using Howestprime.Movies.Domain.Shared;

namespace UnitTests.Domain.Shared;

public sealed class AssertsTests
{
    [Fact]
    public void EnsureNotEmpty_WithEmptyString_ShouldThrow()
    {
        // Arrange
        const string value = "";

        // Act
        Action act = () => Asserts.EnsureNotEmpty(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void EnsureNotEmpty_WithWhitespaceString_ShouldThrow()
    {
        // Arrange
        const string value = "   ";

        // Act
        Action act = () => Asserts.EnsureNotEmpty(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void EnsureNotEmpty_WithNullObject_ShouldThrow()
    {
        // Arrange
        object? value = null;

        // Act
        Action act = () => Asserts.EnsureNotEmpty(value!);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void EnsureNotEmpty_WithValidString_ShouldNotThrow()
    {
        // Arrange
        const string value = "ok";

        // Act
        Action act = () => Asserts.EnsureNotEmpty(value);

        // Assert
        Assert.Null(Record.Exception(act));
    }

    [Fact]
    public void EnsureNotEmpty_WithValidObject_ShouldNotThrow()
    {
        // Arrange
        object value = new();

        // Act
        Action act = () => Asserts.EnsureNotEmpty(value);

        // Assert
        Assert.Null(Record.Exception(act));
    }

    [Theory]
    [InlineData(4, 3)]
    [InlineData(10, 0)]
    public void EnsureGreaterThan_WithValueGreaterThanThreshold_ShouldNotThrow(int value, int threshold)
    {
        // Act
        Action act = () => Asserts.EnsureGreaterThan(value, threshold);

        // Assert
        Assert.Null(Record.Exception(act));
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(2, 3)]
    public void EnsureGreaterThan_WithValueLessOrEqual_ShouldThrow(int value, int threshold)
    {
        // Act
        Action act = () => Asserts.EnsureGreaterThan(value, threshold);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(0)]
    public void EnsureNotNegative_WithPositiveValueOrZero_ShouldNotThrow(int value)
    {
        // Act
        Action act = () => Asserts.EnsureNotNegative(value);

        // Assert
        Assert.Null(Record.Exception(act));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void EnsureNotNegative_WithNegativeValue_ShouldThrow(int value)
    {
        // Act
        Action act = () => Asserts.EnsureNotNegative(value);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
