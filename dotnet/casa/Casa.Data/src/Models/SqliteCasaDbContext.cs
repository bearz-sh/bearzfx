using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class SqliteCasaDbContext : CasaDbContext
{
    public SqliteCasaDbContext(DbContextOptions<SqliteCasaDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=casa.db");

        base.OnConfiguring(optionsBuilder);
    }
}