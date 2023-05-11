namespace Ze.Tasks;

public sealed class ActionTask : ZeTask
{
    private readonly Action<ITaskExecutionContext> action;

    public ActionTask(string name, Action action)
        : base(name)
    {
        this.action = _ => action();
    }

    public ActionTask(string name, string id, Action action)
        : base(name, id)
    {
        this.action = _ => action();
    }

    public ActionTask(string name, Action<ITaskExecutionContext> action)
        : base(name)
    {
        this.action = action;
    }

    public ActionTask(string name, string id, Action<ITaskExecutionContext> action)
        : base(name, id)
    {
        this.action = action;
    }

    protected override Task RunTaskAsync(ITaskExecutionContext context, CancellationToken cancellationToken = default)
    {
        var task = new Task(() => this.action(context));
        task.Start();
        return task;
    }
}