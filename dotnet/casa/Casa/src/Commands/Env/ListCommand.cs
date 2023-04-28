using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

using Bearz.Casa.Data.Services;
using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Env;

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

    public int Invoke(InvocationContext context)
    {
        foreach (var envName in this.set.ListNames())
        {
            Console.WriteLine(envName);
        }

        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}