using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class PgCasaDbContext : CasaDbContext
{
    public PgCasaDbContext(DbContextOptions<PgCasaDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql();

        base.OnConfiguring(optionsBuilder);
    }
}