using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

using Ze.Tasks.Runner.Yaml;
using Ze.Tasks.Runners;

namespace Ze.Commands.Tasks;

[CommandHandler(typeof(TaskRunCommandHandler))]
public class TaskRunCommand : Command
{
    public TaskRunCommand()
        : base("run", "runs a plank task from a yaml file")
    {
        this.AddArgument(new Argument<string[]>("tasks", "One or more tasks to run"));
        this.AddOption(new Option<string>("--file", "The yaml file to load tasks from"));
    }
}

public class TaskRunCommandHandler : ICommandHandler
{
    private readonly IServiceProvider services;

    public TaskRunCommandHandler(IServiceProvider services)
    {
        this.services = services;
    }

    public string? File { get; set; }

    public string[]? Tasks { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var runner = new YamlTaskRunner(this.services);
        var targets = this.Tasks ?? new[] { "default" };
        var options = new YamlTaskRunOptions() { TaskFile = this.File, Targets = targets, };

        var result = await runner.RunAsync(options)
            .ConfigureAwait(false);

        if (result.Status != TaskRunnerStatus.Success)
            return 1;

        return 0;
    }
}