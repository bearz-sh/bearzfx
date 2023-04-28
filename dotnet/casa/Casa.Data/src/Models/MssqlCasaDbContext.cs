using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class MssqlCasaDbContext : CasaDbContext
{
    public MssqlCasaDbContext(DbContextOptions<MssqlCasaDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer();

        base.OnConfiguring(optionsBuilder);
    }
}