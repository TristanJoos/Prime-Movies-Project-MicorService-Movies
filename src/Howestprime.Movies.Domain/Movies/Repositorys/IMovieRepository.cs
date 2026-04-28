using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies;

public interface IMovieRepository : IRepository<Movie, MovieId>
{
    
}
