using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.RDBMS.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(e => e.Id);
            builder
            .HasOne(c => c.User)
            .WithMany(p => p.RefreshTokens)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Token)
                .IsRequired();
        }
    }
}
