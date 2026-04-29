using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
using Microsoft.EntityFrameworkCore;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

public abstract class QueryDbContext : DbContext
{
    public DbSet<MovieData> Movies { get; set; }

    protected QueryDbContext()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieDataConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}