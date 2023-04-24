using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bearz.Casa.Data.Models.Configuration;

public class EnvironmentConfig : IEntityTypeConfiguration<Environment>
{
    public void Configure(EntityTypeBuilder<Environment> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(128);
    }
}