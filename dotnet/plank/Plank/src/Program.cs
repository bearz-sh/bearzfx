using System.CommandLine.Parsing;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Object;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Plank;
using Plank.Commands;
using Plank.Commands.Compose;
using Plank.Package.Actions;

using Serilog;
using Serilog.Events;
using Serilog.Filters;

var loggerConfig = new LoggerConfiguration().
    Filter.ByExcluding(Matching.FromSource("Microsoft"))
    .WriteTo.Console(LogEventLevel.Information);

var appLog = loggerConfig.CreateLogger();

try
{
    var builder = new CommandLineApplicationBuilder();

    builder.Services.AddLogging(lb =>
    {
        lb.ClearProviders();
        lb.AddSerilog(appLog);
    });

    builder.Configuration.AddJsonFile(
        Path.Join(Paths.ConfigDirectory, "plank.json"), true, false);

    builder.Configuration.AddJsonFile(
        Path.Join(Paths.UserConfigDirectory, "plank.json"), true, false);

    builder.UseDefaults();

    builder.AddCommand(new ComposeCommand());
    builder.AddCommand(new InitCommand());

    var app = builder.Build();

    await app.Parse(args).InvokeAsync();
}
catch (Exception ex)
{
    appLog.Fatal(ex, "Casa startup critical failure");
}