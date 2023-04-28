using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;

using Spectre.Console;

namespace Casa.Commands.Env;

[CommandHandler(typeof(CreateCommandHandler))]
public class CreateCommand : Command
{
    public CreateCommand()
        : base("create", "Create a new environment")
    {
    }
}

public class CreateCommandHandler : ICommandHandler
{
    private readonly EnvironmentSet set;

    public CreateCommandHandler(EnvironmentSet set)
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

        this.set.Create(this.Store);
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        throw new NotImplementedException();
    }
}