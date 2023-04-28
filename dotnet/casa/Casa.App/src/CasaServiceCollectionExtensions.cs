using System.Security.Cryptography;

using Bearz.Casa.Data.Models;
using Bearz.Casa.Data.Services;
using Bearz.Security.Cryptography;
using Bearz.Std;
using Bearz.Text;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Bearz.Casa.App;

public static class CasaServiceCollectionExtensions
{
    public static IServiceCollection AddCasa(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptionProvider>(o =>
        {
            var key = Fs.ReadFile(Path.Join(Paths.UserDataDirectory, "casa.key"));
            var realKey = new byte[32];
            Rfc2898DeriveBytes.Pbkdf2(
                key,
                Encodings.Utf8NoBom.GetBytes("salt with fries on casa"),
                realKey,
                60001,
                HashAlgorithmName.SHA256);
            return new AesGcmEncryptionProvider(realKey);
        });
        services.AddSqlite<SqliteCasaDbContext>($"Data Source={Path.Join(Paths.UserDataDirectory, "casa.db")}");
        services.AddTransient<CasaDbContext>(s => s.GetRequiredService<SqliteCasaDbContext>());
        services.AddTransient<EnvironmentSet>();
        services.AddLogging(lb =>
        {
            lb.ClearProviders();
            var cfg = new LoggerConfiguration()
                .Filter.ByExcluding(e =>
                    e.Properties.ContainsKey("SourceContext") &&
                    e.Properties["SourceContext"].ToString().StartsWith("Microsoft"))
                .WriteTo.Console(LogEventLevel.Information);

            if (Fs.DirectoryExists(Paths.LogsDirectory))
            {
                cfg.WriteTo.File(Path.Join(Paths.LogsDirectory, "casa.log"), LogEventLevel.Debug);
            }

            var logger = cfg.CreateLogger();

            lb.AddSerilog(logger);
        });

        return services;
    }
}