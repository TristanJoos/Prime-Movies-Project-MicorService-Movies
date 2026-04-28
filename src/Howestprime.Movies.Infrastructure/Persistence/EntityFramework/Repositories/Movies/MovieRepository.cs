using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Repositories;

public sealed class MovieRepository (
    DomainDbContext context
) : EfCoreGenericRepository<Movie, MovieId>(context), IMovieRepository
{
}
