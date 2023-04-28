using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Config;

[CommandHandler(typeof(ConfigCommandHandler))]
public class ConfigCommand : Command
{
    public ConfigCommand()
        : base("config", "Manage the configuration")
    {
        this.AddCommand(new SetupCommand());
    }
}

public class ConfigCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}