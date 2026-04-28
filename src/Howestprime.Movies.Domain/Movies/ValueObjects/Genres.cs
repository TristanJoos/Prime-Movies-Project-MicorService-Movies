using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record Genres(string Value) : ValueObject
{
    public static Genres Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Genres must be a non-empty string.");
        } 

        return new Genres(value);
        
    }
}

