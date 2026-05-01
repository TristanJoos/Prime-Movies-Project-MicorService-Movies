using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies.Repositorys;


public interface IMovieEventRepository : IRepository<MovieEvent, MovieEventId>
{
    Task<MovieEvent?> ByShowtimeAndRoomId(DateTime showtime, Guid roomId);

    Task <MovieEvent?> GetById(Guid movieEventId);
}