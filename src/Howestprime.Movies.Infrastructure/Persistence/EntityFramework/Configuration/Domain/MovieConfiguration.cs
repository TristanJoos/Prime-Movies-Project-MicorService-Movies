using Howestprime.Movies.Domain.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
     public void Configure(EntityTypeBuilder<Movie> builder)
    {

        builder.OwnsMany(m => m.Actors, actorBuilder =>
        {
            actorBuilder.ToJson();
            actorBuilder.Property(a => a.Value);
        });

        builder.OwnsMany(m => m.Genres, genreBuilder =>
        {
            genreBuilder.ToJson();
            genreBuilder.Property(g => g.Value);
        });
    }
}