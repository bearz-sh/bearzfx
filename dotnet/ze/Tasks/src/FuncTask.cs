namespace Ze.Tasks;

public class FuncTask : ZeTask
{
    private readonly Func<ITaskExecutionContext, Task> action;

    public FuncTask(string name, Func<ITaskExecutionContext, Task> action)
        : base(name)
    {
        this.action = action;
    }

    public FuncTask(string name, string id, Func<ITaskExecutionContext, Task> action)
        : base(name, id)
    {
        this.action = action;
    }

    protected override Task RunTaskAsync(
        ITaskExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        return this.action(context);
    }
}