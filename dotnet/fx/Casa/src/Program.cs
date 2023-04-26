// See https://aka.ms/new-console-template for more information

using System.CommandLine.Parsing;

using Bearz.Extensions.Hosting.CommandLine;

using Casa.Commands.Age;
using Casa.Commands.MkCert;
using Casa.Commands.Sops;

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

    builder.AddCommand(new AgeCommand());
    builder.AddCommand(new MkCertCommand());
    builder.AddCommand(new SopsCommand());

    var app = builder.Build();

    await app.Parse(args).InvokeAsync();
}
catch (Exception ex)
{
    appLog.Fatal(ex, "Casa startup critical failure");
}