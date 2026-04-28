using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.ValueObjects;

public sealed record Duration(int Value) : ValueObject
{
    public static Duration Create(int value)
    {
        if ( value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Duration must be a positive value.");
        } 

        return new Duration(value);
        
    }
}

