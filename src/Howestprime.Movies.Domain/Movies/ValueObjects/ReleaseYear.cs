using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record ReleaseYear(int Value) : ValueObject
{
    public static ReleaseYear Create(int value)
    {
        if ( value > DateTime.Now.Year)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Release year must be the current year or earlier.");
        } 

        return new ReleaseYear(value);
        
    }
}

