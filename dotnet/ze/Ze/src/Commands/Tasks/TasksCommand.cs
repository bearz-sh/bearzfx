using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

namespace Ze.Commands.Tasks;

[CommandHandler(typeof(TasksCommandHandler))]
public class TasksCommand : Command
{
    public TasksCommand()
        : base("tasks", "manages plank tasks")
    {
        this.AddCommand(new TaskRunCommand());
        this.AddCommand(new TaskListCommand());
    }
}

public class TasksCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        context.HelpBuilder.Write(new HelpContext(context.HelpBuilder, context.BindingContext.ParseResult.CommandResult.Command, Console.Out));
        return Task.FromResult(0);
    }
}