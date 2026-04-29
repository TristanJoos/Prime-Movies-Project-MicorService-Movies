using Microsoft.EntityFrameworkCore;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Repositories;

public sealed class MovieEventRepository (
    DomainDbContext context
) : EfCoreGenericRepository<MovieEvent, MovieEventId>(context), IMovieEventRepository
{
    public async Task<MovieEvent?> ByShowtimeAndRoomId(DateTime showtime, Guid roomId)
    {
        RoomId roomIdValue = new(roomId);
        return await _context.Set<MovieEvent>()
            .FirstOrDefaultAsync(e => e.Showtime == showtime && e.RoomId == roomIdValue);
    }
}
