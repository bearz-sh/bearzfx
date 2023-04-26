using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.App.Tasks;
using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Env;

[CommandHandler(typeof(SetCommandHandler))]
public class SetCommand : Command
{
    public SetCommand()
        : base("set", "Set the default environment")
    {
        this.AddArgument(new Argument<string>("name", "Name of the environment."));
    }
}

public class SetCommandHandler : ICommandHandler
{
    public string Name { get; set; } = string.Empty;

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var task = new SaveSettingsTask()
        {
            Settings = new Dictionary<string, object?>()
            {
                ["env"] = new Dictionary<string, object?>() { ["name"] = this.Name, },
            },
            Store = "user",
        };

        await task.RunAsync(context.GetCancellationToken());
        return 0;
    }
}