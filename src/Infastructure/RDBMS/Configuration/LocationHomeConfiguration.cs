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
                .HasMaxLength(30)
                .HasColumnType("nvarchar");

            builder.Property(e => e.Street)
                .IsRequired()
                .HasColumnName("street")
                .HasMaxLength(50)
                .HasColumnType("nvarchar");

            builder.Property(e => e.IsActive)
                .HasColumnName("is_active");

            builder.Property(e => e.Latitude)
                .IsRequired()
                .HasColumnName("latitude");

            builder.Property(e => e.Longitude)
                .IsRequired()
                .HasColumnName("longitude");
        }
    }
}
