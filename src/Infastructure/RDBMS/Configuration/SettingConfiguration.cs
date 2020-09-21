using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.RDBMS.Configuration
{
    internal class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(setting => new
            {
                setting.Namespace,
                Name = setting.Key
            });

            builder.Property(setting => setting.Namespace)
                .HasDefaultValue(Setting.DefaultNamespace);

            builder.Property(setting => setting.Key)
                .HasConversion<string>();
        }
    }
}
