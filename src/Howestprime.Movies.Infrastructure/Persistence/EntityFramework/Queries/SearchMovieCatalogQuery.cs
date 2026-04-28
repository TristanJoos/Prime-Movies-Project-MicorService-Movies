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

public sealed class SearchMovieCatalogQuery(
    QueryDbContext context
) : ISearchMovieCatalogQuery
{
    public Task<IEnumerable<MovieData>> Fetch(Expression<Func<MovieData, bool>> filter)
    {
        return Task.FromResult(context.Movies
            .Include(movie => movie.Genres)
            .Where(filter)
            .AsEnumerable());
    }
}
