using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;

using Spectre.Console;

namespace Casa.Commands.Env.Secrets;

[CommandHandler(typeof(GetCommandHandler))]
public class GetCommand : Command
{
    public GetCommand()
        : base("get", "Gets the secret value")
    {
        this.AddArgument(new Argument<string>("name", "Name of the secret to get."));
    }
}

public class GetCommandHandler : ICommandHandler
{
    private readonly EnvironmentSet set;

    public GetCommandHandler(EnvironmentSet set)
    {
        this.set = set;
    }

    public string Store { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public int Invoke(InvocationContext context)
    {
        if (this.Store.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Environment store cannot be empty");
            return 1;
        }

        if (this.Name.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Secret name cannot be empty");
            return 1;
        }

        var env = this.set.Get(this.Store);
        if (env is null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Environment store does not exist");
            return 1;
        }

        Console.WriteLine(env.GetSecret(this.Name));
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}