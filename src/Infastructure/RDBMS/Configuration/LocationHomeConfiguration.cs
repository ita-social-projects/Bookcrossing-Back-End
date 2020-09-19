using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.RDBMS.Configuration
{
    class LocationHomeConfiguration : IEntityTypeConfiguration<LocationHome>
    {
        public void Configure(EntityTypeBuilder<LocationHome> builder)
        {
            builder.ToTable("LocationHome");
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.City)
                .IsRequired()
                .HasColumnName("city")
                .HasMaxLength(30);

            builder.Property(e => e.Street)
                .IsRequired()
                .HasColumnName("street")
                .HasMaxLength(50);

            builder.Property(e => e.IsActive)
                .HasColumnName("is_active");

            builder.Property(e => e.Latitude)
                .IsRequired()
                .HasColumnName("latitude");

            builder.Property(e => e.Longitude)
                .IsRequired()
                .HasColumnName("longitude");

            builder.Property(e => e.UserId)
               .IsRequired()
               .HasColumnName("user_id");

            builder.HasOne(d => d.User)
               .WithOne(p => p.LocationHome)
               .HasForeignKey<LocationHome>(d => d.UserId)
               .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasQueryFilter(location => location.IsActive == true);
        }
    }
}
