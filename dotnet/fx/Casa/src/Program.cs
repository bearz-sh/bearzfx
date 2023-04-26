// See https://aka.ms/new-console-template for more information

using System.CommandLine.Parsing;

using Bearz.Extensions.Hosting.CommandLine;

using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;

var loggerConfig = new Serilog.LoggerConfiguration().
    Filter.ByExcluding(e =>
        e.Properties.ContainsKey("SourceContext") &&
        e.Properties["SourceContext"].ToString().StartsWith("Microsoft"))
    .WriteTo.Console(LogEventLevel.Information);

var appLog = loggerConfig.CreateLogger();

try
{
    var builder = new ConsoleApplicationBuilder();

    builder.UseDefaults();

    var app = builder.Build();

    await app.Parse(args).InvokeAsync();
}
catch (Exception ex)
{
    appLog.Fatal(ex, "Casa startup critical failure");
}