using Howestprime.Movies.Domain.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
public sealed class MovieEventConfiguration : IEntityTypeConfiguration<MovieEvent>
{
     public void Configure(EntityTypeBuilder<MovieEvent> builder)
    {
        builder.ToTable("MovieEvents");

        builder.HasKey(movieEvent => movieEvent.Id);
        builder.Property(movieEvent => movieEvent.Id).ValueGeneratedNever();

       builder.Property(movieEvent => movieEvent.MovieId).IsRequired();
       builder.Property(movieEvent => movieEvent.RoomId).IsRequired();

       builder.Property(movieEvent => movieEvent.Showtime).IsRequired();
       builder.Property(movieEvent => movieEvent.Capacity).IsRequired();

       builder.Property(movieEvent => movieEvent.Visitors).IsRequired();
       
       builder.OwnsMany(me => me.Bookings, bookingBuilder =>
        {
            bookingBuilder.ToJson();
            bookingBuilder.Property(b => b.Id);
            bookingBuilder.Property(b => b.PaymentStatus);
            bookingBuilder.Property(b => b.StandardVisitors);
            bookingBuilder.Property(b => b.DiscountVisitors);
            bookingBuilder.Property(b => b.SeatNumbers);
        });
    }

}