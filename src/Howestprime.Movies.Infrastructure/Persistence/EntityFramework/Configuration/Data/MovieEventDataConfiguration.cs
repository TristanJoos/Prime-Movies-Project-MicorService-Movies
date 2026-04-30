using Howestprime.Movies.Application.Contracts.Data;
using Howestprime.Movies.Domain.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
public sealed class MovieEventDataConfiguration : IEntityTypeConfiguration<MovieEventData>
{
     public void Configure(EntityTypeBuilder<MovieEventData> builder)
    {
        builder.ToTable("MovieEvents");

        builder.HasKey(movieEvent => movieEvent.Id);
        builder.Property(movieEvent => movieEvent.Id).ValueGeneratedNever();

       builder.Property(movieEvent => movieEvent.Showtime).IsRequired();
       builder.Property(movieEvent => movieEvent.Capacity).IsRequired();

       builder.HasOne(movieEvent => movieEvent.Movie)
            .WithMany()
            .HasForeignKey("MovieId")
            .IsRequired();

        builder.HasOne(movieEvent => movieEvent.Room)
            .WithMany()
            .HasForeignKey("RoomId")
            .IsRequired();
    }

}