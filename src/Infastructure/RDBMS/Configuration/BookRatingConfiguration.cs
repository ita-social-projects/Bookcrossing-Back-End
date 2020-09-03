using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.RDBMS.Configuration
{
    class BookRatingConfiguration : IEntityTypeConfiguration<BookRating>
    {
        public void Configure(EntityTypeBuilder<BookRating> builder)
        {
            builder.ToTable("BookRating");
            builder.HasKey(e => new { e.BookId, e.UserId });

            builder.Property(e => e.BookId).HasColumnName("book_id");

            builder.Property(e => e.UserId).HasColumnName("user_id");

            builder.HasOne(d => d.Book)
                .WithMany(p => p.BookRating)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
