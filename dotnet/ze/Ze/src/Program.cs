using System.CommandLine.Parsing;

using Bearz.Extensions.Hosting.CommandLine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Templates;
using Serilog.Templates.Themes;

using Ze;
using Ze.Commands;
using Ze.Commands.Compose;
using Ze.Commands.Tasks;
using Ze.Tasks.Runner.Runners;

var loggerConfig = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.Console(
        new ExpressionTemplate(
            "{#if @l <> 'Information'}{@l:u3}: {#end}{@m}\n{@x}",
            theme: TemplateTheme.Code));

var appLog = loggerConfig.CreateLogger();

try
{
    var builder = new CommandLineApplicationBuilder();

    builder.Services.AddLogging(lb =>
        {
            lb.ClearProviders();
            lb.AddSerilog(appLog);
        })
        .AddRunnerCore();

    builder.Configuration.AddJsonFile(
        Path.Join(Paths.ConfigDirectory, "plank.json"), true, false);

    builder.Configuration.AddJsonFile(
        Path.Join(Paths.UserConfigDirectory, "plank.json"), true, false);

    builder.UseDefaults();

    builder.AddCommand(new ComposeCommand());
    builder.AddCommand(new TasksCommand());
    builder.AddCommand(new InitCommand());

    var app = builder.Build();

    return await app.Parse(args).InvokeAsync();
}
catch (Exception ex)
{
    appLog.Fatal(ex, "Ze startup critical failure");
    return 1;
}