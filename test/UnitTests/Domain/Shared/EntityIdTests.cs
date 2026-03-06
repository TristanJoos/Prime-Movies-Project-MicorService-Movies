using Howestprime.Movies.Domain.Shared;

namespace UnitTests.Domain.Shared;

public sealed class EntityIdTests
{
    [Fact]
    public void New_WithProvidedGuid_ShouldUseGuid()
    {
        // Arrange
        Guid guid = Guid.NewGuid();

        // Act
        TestId id = EntityId.New<TestId>(guid);

        // Assert
        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void New_WithoutGuid_ShouldCreateNonEmptyValue()
    {
        // Arrange
        Guid empty = Guid.Empty;

        // Act
        TestId id = EntityId.New<TestId>();

        // Assert
        Assert.NotEqual(empty, id.Value);
    }

    [Fact]
    public void New_WithMissingGuidConstructor_ShouldThrow()
    {
        // Arrange
        Action act = () => EntityId.New<BadId>();

        // Act
        Exception? exception = Record.Exception(act);

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
    }

    public readonly record struct TestId(Guid Value) : IEntityId;

    public readonly record struct BadId(int Number) : IEntityId
    {
        public Guid Value => Guid.Empty;
    }
}
