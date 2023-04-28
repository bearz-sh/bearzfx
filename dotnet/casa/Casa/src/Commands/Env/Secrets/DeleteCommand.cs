using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;

using Spectre.Console;

namespace Casa.Commands.Env.Secrets;

[CommandHandler(typeof(DeleteCommandHandler))]
public class DeleteCommand : Command
{
    public DeleteCommand()
        : base("delete", "Delete a secret")
    {
        this.AddArgument(new Argument<string>("name", "Name of the secret to delete."));
    }
}

public class DeleteCommandHandler : ICommandHandler
{
    private readonly EnvironmentSet set;

    public DeleteCommandHandler(EnvironmentSet set)
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

        env.DeleteSecret(this.Name);
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}