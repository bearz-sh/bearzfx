using Bearz.Std;

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