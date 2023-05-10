using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Plank.Tasks.Internal;

public class TaskExecutionContext : ExecutionContext, ITaskExecutionContext
{
    public TaskExecutionContext(ITask task, IServiceProvider services)
        : base(services)
    {
        this.Task = task;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        this.Log = loggerFactory.CreateLogger(task.Id);
        this.Outputs = new Outputs(task.Id, "tasks", this.Env.SecretMasker);
    }

    public TaskExecutionContext(ITask task, IExecutionContext context)
        : base(context)
    {
        this.Task = task;
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        this.Log = loggerFactory.CreateLogger(task.Id);

        if (context is IActionExecutionContext tec)
        {
            this.Outputs = new Outputs(task.Id, "tasks", this.Env.SecretMasker, tec.Outputs);
        }
        else
        {
            this.Outputs = new Outputs(task.Id, "tasks", this.Env.SecretMasker);
        }
    }

    public IOutputs Outputs { get; }

    public ITask Task { get; }

    public TaskStatus Status { get; set; }
}