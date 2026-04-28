using Microsoft.EntityFrameworkCore;
using Howestprime.Movies.Domain.Shared.DomainEvents;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Converters;
using Howestprime.Movies.Domain.Movies;
using Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration;

public abstract class DomainDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    private readonly Queue<IDomainEvent> _queuedDomainEvents = new();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        EntityIdConverter.AddConventions(configurationBuilder);
        SinglePropertyValueObjectConverter.AddConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MovieConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public void QueueDomainEvent(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
            _queuedDomainEvents.Enqueue(domainEvent);
    }

    public IReadOnlyCollection<IDomainEvent> GetQueuedDomainEvents()
    {
        return _queuedDomainEvents.ToList().AsReadOnly();
    }

    public void ClearQueuedDomainEvents()
    {
        _queuedDomainEvents.Clear();
    }
}
