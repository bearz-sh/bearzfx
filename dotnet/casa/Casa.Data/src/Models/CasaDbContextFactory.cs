using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Bearz.Casa.Data.Models;

public class CasaDbContextFactory : IDesignTimeDbContextFactory<CasaDbContext>
{
    public CasaDbContext CreateDbContext(string[] args)
    {
        var provider = args.FirstOrDefault() ?? "sqlite";

        switch (provider)
        {
            case "pg":
            case "postgres":
                {
                    var builder = new DbContextOptionsBuilder<PgCasaDbContext>();
                    return new PgCasaDbContext(builder.Options);
                }

            case "mssql":
            case "sqlserver":
                {
                    var builder = new DbContextOptionsBuilder<MssqlCasaDbContext>();
                    return new MssqlCasaDbContext(builder.Options);
                }

            default:
                {
                    var builder = new DbContextOptionsBuilder<SqliteCasaDbContext>();
                    return new SqliteCasaDbContext(builder.Options);
                }
        }
    }
}