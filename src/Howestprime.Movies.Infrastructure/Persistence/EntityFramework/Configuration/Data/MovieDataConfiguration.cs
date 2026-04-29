using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Domain.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
public sealed class MovieDataConfiguration : IEntityTypeConfiguration<MovieData>
{
     public void Configure(EntityTypeBuilder<MovieData> builder)
    {
        builder.ToTable("Movies");

        builder.HasKey(movie => movie.Id);
        builder.Property(movie => movie.Id).ValueGeneratedNever();

        builder.Property(movie => movie.Title);
        builder.Property(movie => movie.Description);
        builder.Property(movie => movie.ReleaseYear);
        builder.Property(movie => movie.Duration);
        builder.Property(movie => movie.AgeRating);
        builder.Property(movie => movie.PosterUrl);

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