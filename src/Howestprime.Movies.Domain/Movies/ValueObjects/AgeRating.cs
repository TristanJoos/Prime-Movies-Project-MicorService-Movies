using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record AgeRating(int Value) : ValueObject
{
    public static AgeRating Create(int value)
    {
        if ( value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Age rating must be a positive value.");
        } 

        return new AgeRating(value);
        
    }
}

