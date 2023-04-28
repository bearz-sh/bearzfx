using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.App.Tasks;
using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;

using Microsoft.Extensions.Configuration;

using Spectre.Console;

namespace Casa.Commands.Compose;

[CommandHandler(typeof(EvaluateCommandHandler))]
public class EvaluateCommand : Command
{
    public EvaluateCommand()
        : base("evaluate", "Evaluate the templates for the compose stack")
    {
        this.AddOption(new Option<bool>(new[] { "--overwrite", "-o" }, "Overwrite existing files"));
        this.AddOption(new Option<bool>(new[] { "--import", "-i" }, "Import the evaluated files into the compose stack"));
    }
}

public class EvaluateCommandHandler : ICommandHandler
{
    private readonly EnvironmentSet set;

    private readonly IConfiguration config;

    public EvaluateCommandHandler(EnvironmentSet set, IConfiguration config)
    {
        this.set = set;
        this.config = config;
    }

    public string? Env { get; set; }

    public string? ProjectDirectory { get; set; }

    public bool Overwrite { get; set; }

    public bool Import { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        if (this.ProjectDirectory.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine("[red]No template directory specified[/]");
            return 1;
        }

        var env = this.Env ?? this.config.GetValue<string?>("env:default") ?? "default";
        var task = new EvaluateTemplateTask(this.set)
        {
            Environment = env,
            TemplateDirectory = this.ProjectDirectory,
            Import = this.Import,
            Overwrite = this.Overwrite,
        };

        await task.RunAsync(context.GetCancellationToken());
        return 0;
    }
}