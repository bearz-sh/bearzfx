namespace Plank.Tasks;

public abstract class PlankTask : ITask
{
    protected PlankTask(string name)
        : this(name, name)
    {
    }

    protected PlankTask(string name, string id)
    {
        this.Name = name;
        this.Id = IdGenerator.Instance.FromName(id.AsSpan());
    }

    public string Id { get; }

    public string Name { get; set; }

    public virtual IReadOnlyList<string> Dependencies { get; set; } = Array.Empty<string>();

    public int Timeout { get; set; }

    public string? Description { get; set; }

    public bool ContinueOnError { get; set; }

    public async Task RunAsync(
        ITaskExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            context.Status = TaskStatus.Cancelled;
            return;
        }

        try
        {
            await this.RunTaskAsync(context, cancellationToken)
                .ConfigureAwait(false);

            if (context.Status == TaskStatus.None)
                context.Status = TaskStatus.Completed;
        }
        catch (Exception e)
        {
            context.Error(e);
        }
    }

    protected abstract Task RunTaskAsync(
        ITaskExecutionContext context,
        CancellationToken cancellationToken = default);
}