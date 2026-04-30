using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
using Microsoft.EntityFrameworkCore;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

public abstract class QueryDbContext : DbContext
{
    public DbSet<MovieData> Movies { get; set; }
    public DbSet<MovieEventData> MovieEvents { get; set; }
    public DbSet<RoomData> Rooms { get; set; }

    protected QueryDbContext()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieDataConfiguration());
        modelBuilder.ApplyConfiguration(new MovieEventDataConfiguration());
        modelBuilder.ApplyConfiguration(new RoomDataConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}