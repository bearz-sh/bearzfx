using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

namespace Plank.Commands.Compose;

[CommandHandler(typeof(ComposeCommandHandler))]
public class ComposeCommand : Command
{
    public ComposeCommand()
        : base("compose", "manages plank compose apps")
    {
        this.AddGlobalOption(new Option<string>(new[] { "--target", "-t" }, "The target to use"));
        this.AddCommand(new ExpandCommand());
        this.AddCommand(new InstallCommand());
        this.AddCommand(new UninstallCommand());
        this.AddCommand(new NetworkCommand());
    }
}

public class ComposeCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        throw new NotImplementedException();
    }
}