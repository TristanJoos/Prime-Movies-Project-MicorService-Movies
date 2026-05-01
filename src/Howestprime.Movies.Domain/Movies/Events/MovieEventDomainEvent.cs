using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.Events;

public abstract class MovieEventDomainEvent(string eventName) :
     BaseDomainEvent(
        eventName, 
        aggregateName: nameof(MovieEvent), 
        boundedContext: nameof(Movies),
         companyName: nameof(Howestprime)
    )
{
}
