using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class MssqlCasaDbContext : CasaDbContext
{
    public MssqlCasaDbContext(DbContextOptions<MssqlCasaDbContext> options)
        : base(options)
    {
    }
}