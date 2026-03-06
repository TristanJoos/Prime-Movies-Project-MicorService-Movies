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
}
