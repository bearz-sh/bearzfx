using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Std;

using Ze.Tasks.Runner.Yaml;

using Command = System.CommandLine.Command;

namespace Ze.Commands.Tasks;

[CommandHandler(typeof(TaskListCommandHandler))]
public class TaskListCommand : Command
{
    public TaskListCommand()
        : base("list", "list the available tasks")
    {
        this.AddOption(new Option<string>("--file", "The yaml file to load tasks from"));
    }
}

public class TaskListCommandHandler : ICommandHandler
{
    public string? File { get; set; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        var wf = TasksYamlFileParser.ParseFile(this.File ?? FsPath.Combine(Env.Cwd, "tasks.yaml"));
        var tasks = wf.Tasks;
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id} - {task.Name}");
        }

        return Task.FromResult(0);
    }
}