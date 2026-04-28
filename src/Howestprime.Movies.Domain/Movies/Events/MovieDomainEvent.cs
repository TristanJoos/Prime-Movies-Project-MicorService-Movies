using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.Events;

public abstract class MovieDomainEvent(string eventName) :
     BaseDomainEvent(
        eventName, 
        aggregateName: nameof(Movie), 
        boundedContext: nameof(Movies),
         companyName: nameof(Howestprime)
    )
{
}
