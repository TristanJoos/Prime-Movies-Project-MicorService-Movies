using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record PosterUrl(string Value) : ValueObject
{
    public static PosterUrl Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Poster URL must be a non-empty string.");
        } 

        return new PosterUrl(value);
        
    }
}

