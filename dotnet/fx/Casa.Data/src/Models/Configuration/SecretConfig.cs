using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bearz.Casa.Data.Models.Configuration;

public class SecretConfig : IEntityTypeConfiguration<Secret>
{
    public void Configure(EntityTypeBuilder<Secret> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasOne(o => o.Environment)
            .WithMany(o => o.Secrets)
            .OnDelete(DeleteBehavior.Cascade);
    }
}