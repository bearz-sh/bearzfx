using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;

using Spectre.Console;

namespace Casa.Commands.Env.Secrets;

[CommandHandler(typeof(ListCommandHandler))]
public class ListCommand : Command
{
    public ListCommand()
        : base("list", "List all secrets in the vault")
    {
    }
}

public class ListCommandHandler : ICommandHandler
{
    private readonly EnvironmentSet set;

    public ListCommandHandler(EnvironmentSet set)
    {
        this.set = set;
    }

    public string Store { get; set; } = string.Empty;

    public int Invoke(InvocationContext context)
    {
        if (this.Store.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Environment store cannot be empty");
            return 1;
        }

        var env = this.set.Get(this.Store);
        if (env is null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Environment store does not exist");
            return 1;
        }

        foreach (var secret in env.Secrets)
        {
            Console.WriteLine(secret.Name);
        }

        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}