using Howestprime.Movies.Domain.Movies.ValueObjects;

namespace UnitTests.Domain.Movies;

public class ActorTest
{
    [Fact]
    public void Create_WithValidName_ShouldReturnActors()
    {
        // Arrange
        const string actorName = "John Doe";

        // Act
        var actor = Actors.Create(actorName);

        // Assert
        Assert.NotNull(actor);
        Assert.Equal(actorName, actor.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_NullOrEmptyName_ShouldThrowArgumentOutOfRangeException(string? invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Actors.Create(invalidName!));
    }
}