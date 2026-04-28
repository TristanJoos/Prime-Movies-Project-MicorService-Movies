using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record Actors(string Value) : ValueObject
{
    public static Actors Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Actors must be a non-empty string.");
        } 

        return new Actors(value);
        
    }
}

