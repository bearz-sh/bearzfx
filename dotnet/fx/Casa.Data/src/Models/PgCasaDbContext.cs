using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class PgCasaDbContext : CasaDbContext
{
    public PgCasaDbContext(DbContextOptions<PgCasaDbContext> options)
        : base(options)
    {
    }
}