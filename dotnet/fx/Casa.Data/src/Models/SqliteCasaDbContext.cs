using Microsoft.EntityFrameworkCore;

namespace Bearz.Casa.Data.Models;

public class SqliteCasaDbContext : CasaDbContext
{
    public SqliteCasaDbContext(DbContextOptions<SqliteCasaDbContext> options)
        : base(options)
    {
    }
}