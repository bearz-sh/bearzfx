using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class CasaDbContext : DbContext
{
    public CasaDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Secret> Secrets => this.Set<Secret>();

    public DbSet<Environment> Environments => this.Set<Environment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CasaDbContext).Assembly);
    }
}