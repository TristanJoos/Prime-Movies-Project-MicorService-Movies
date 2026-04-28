using Howestprime.Movies.Domain.Shared;

namespace UnitTests.Domain.Shared;

public sealed class ValueObjectTests
{
    [Fact]
    public void DerivedType_ShouldBeValueObject()
    {
        // Arrange
        SampleValueObject valueObject = new(1, "text");

        // Act
        bool isValueObject = valueObject is not null;

        // Assert
        Assert.True(isValueObject);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        SampleValueObject first = new(5, "value");
        SampleValueObject second = new(5, "value");

        // Act
        bool equals = first == second;

        // Assert
        Assert.True(equals);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        SampleValueObject first = new(5, "value");
        SampleValueObject second = new(6, "value");

        // Act
        bool equals = first == second;

        // Assert
        Assert.False(equals);
    }

    [Fact]
    public void CopyConstructor_WithRecord_ShouldClone()
    {
        // Arrange
        SampleValueObject original = new(5, "value");

        // Act
        SampleValueObject clone = original with { };

        // Assert
        Assert.Equal(original, clone);
        Assert.NotSame(original, clone);
    }

    public sealed record SampleValueObject(int Number, string Text) : ValueObject;
}
