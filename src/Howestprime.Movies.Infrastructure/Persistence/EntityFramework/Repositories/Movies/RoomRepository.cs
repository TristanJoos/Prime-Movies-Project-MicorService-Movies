using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Domain.Movies.Repositorys;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

using Microsoft.EntityFrameworkCore;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Repositories;

public sealed class RoomRepository (
    DomainDbContext context
) : EfCoreGenericRepository<Room, RoomId>(context), IRoomRepository
{
}
