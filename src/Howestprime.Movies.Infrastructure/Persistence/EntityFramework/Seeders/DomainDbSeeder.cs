using Microsoft.Extensions.Logging;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Seeders;

public class DomainDbSeeder(DomainDbContext context, ILogger<DomainDbSeeder> _logger)
{
    public async Task Seed()
    {
    }
}
