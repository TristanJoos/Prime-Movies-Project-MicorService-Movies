namespace Simplifyme.Taskly.Infrastructure.Persistence.EntityFramework.Queries;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;
using System.Linq.Expressions;

public sealed class MovieEventQuery(
    QueryDbContext context
) : IMovieEventQuery
{
    public async Task<IReadOnlyList<MovieEventData>> Fetch(Expression<Func<MovieEventData, bool>> filter)
    {
        return await context.MovieEvents
            .Where(filter)
            .ToListAsync();
    }
}
