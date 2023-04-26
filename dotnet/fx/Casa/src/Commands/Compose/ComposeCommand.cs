using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Compose;

[CommandHandler(typeof(ComposeCommandHandler))]
public class ComposeCommand : Command
{
    public ComposeCommand()
        : base("compose", "Compose the environment")
    {
        this.AddCommand(new EvaluateCommand());
    }
}

public class ComposeCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}