using Howestprime.Movies.Domain.Movies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Howestprime.Movies.Infrastructure.Persistence.EntityFramework.Configuration.Domain;
public sealed class RoomDataConfiguration : IEntityTypeConfiguration<RoomData>
{
    public void Configure(EntityTypeBuilder<RoomData> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(room => room.Id);
        builder.Property(room => room.Id).ValueGeneratedNever();

        builder.Property(room => room.Name).IsRequired();
        builder.Property(room => room.Capacity).IsRequired();
        
    }
}