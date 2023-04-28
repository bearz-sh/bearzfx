using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.App.Tasks;
using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Config;

[CommandHandler(typeof(SetupCommandHandler))]
public class SetupCommand : Command
{
    public SetupCommand()
        : base("setup", "Setup the configuration")
    {
        this.AddOption(
            new Option<bool>(new[] { "--migrate", "-m" }, "run database migrations"));
    }
}

public class SetupCommandHandler : ICommandHandler
{
    public bool Migrate { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var task = new LocalSetupTask()
        {
            RunMigrations = this.Migrate,
        };

        await task.RunAsync(context.GetCancellationToken());
        return 0;
    }
}