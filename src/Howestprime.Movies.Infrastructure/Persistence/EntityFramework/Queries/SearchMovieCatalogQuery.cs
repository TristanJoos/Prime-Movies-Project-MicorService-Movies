namespace Simplifyme.Taskly.Infrastructure.Persistence.EntityFramework.Queries;

using System.Linq;
using System.Threading.Tasks;
using Aornis;
using Microsoft.EntityFrameworkCore;
using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Application.Contracts.Ports;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq.Expressions;
using System.Collections.Immutable;

public sealed class SearchMovieCatalogQuery(
    QueryDbContext context
) : ISearchMovieCatalogQuery
{
    public async Task<IReadOnlyList<MovieData>> Fetch(Expression<Func<MovieData, bool>> filter)
    {
        return await context.Movies
            .Where(filter)
            .ToListAsync();
    }
}
