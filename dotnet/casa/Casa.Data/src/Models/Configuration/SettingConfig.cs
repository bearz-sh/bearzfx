using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bearz.Casa.Data.Models.Configuration;

public class SettingConfig : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.Property(o => o.Name)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(o => o.Value)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasOne(o => o.Environment)
            .WithMany(o => o.Settings)
            .OnDelete(DeleteBehavior.Cascade);
    }
}