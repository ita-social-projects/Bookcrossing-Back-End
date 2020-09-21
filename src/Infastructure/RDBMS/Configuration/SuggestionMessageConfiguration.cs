using System;
using System.Collections.Generic;
using System.Text;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.RDBMS.Configuration
{
    class SuggestionMessageConfiguration : IEntityTypeConfiguration<SuggestionMessage>
    {
        public void Configure(EntityTypeBuilder<SuggestionMessage> builder)
        {
            builder.ToTable("SuggestionMessage");
            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.State)
                .HasMaxLength(50)
                .HasConversion(x => x.ToString(),
                    x => (MessageState)Enum.Parse(typeof(MessageState), x))
                .HasDefaultValue(MessageState.Unread);

            builder.Property(e => e.Summary)
                .HasColumnName("summary")
                .HasMaxLength(150);

            builder.Property(e => e.Text)
                .HasColumnName("text")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasColumnName("user_id");


            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);
        }
    }
}
