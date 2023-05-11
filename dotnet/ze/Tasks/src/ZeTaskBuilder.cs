namespace Ze.Tasks;

public class ZeTaskBuilder
{
    private readonly ITask task;

    public ZeTaskBuilder(ITask task)
    {
        this.task = task;
    }

    public ZeTaskBuilder WithName(string name)
    {
        this.task.Name = name;
        return this;
    }

    public ZeTaskBuilder WithDescription(string description)
    {
        this.task.Description = description;
        return this;
    }

    public ZeTaskBuilder WithDependencies(IEnumerable<string> dependencies)
    {
        this.task.Dependencies = dependencies.ToList();
        return this;
    }

    public ZeTaskBuilder WithDependencies(params string[] dependencies)
    {
        this.task.Dependencies = dependencies;
        return this;
    }

    public ZeTaskBuilder WithTimeout(TimeSpan timeout)
    {
        this.task.Timeout = (int)timeout.TotalMilliseconds;
        return this;
    }

    public ZeTaskBuilder WithTimeout(int timeout)
    {
        this.task.Timeout = timeout;
        return this;
    }

    public ZeTaskBuilder WithContinueOnError(bool continueOnError = true)
    {
        this.task.ContinueOnError = continueOnError;
        return this;
    }
}