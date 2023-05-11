using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ze.Tasks.Internal;

public class TaskExecutionContext : ExecutionContext, ITaskExecutionContext
{
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

        if (this.Variables is IMutableVariables mut)
        {
            mut["task"] = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            {
                ["name"] = task.Name,
                ["id"] = task.Id,
                ["description"] = task.Description,
                ["timeout"] = task.Timeout,
                ["continueOnError"] = task.ContinueOnError,
                ["dependencies"] = task.Dependencies,
            };
        }
    }

    public IOutputs Outputs { get; }

    public ITask Task { get; }

    public TaskStatus Status { get; set; }
}