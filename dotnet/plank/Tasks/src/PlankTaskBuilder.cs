namespace Plank.Tasks;

public class PlankTaskBuilder
{
    private readonly ITask task;

    public PlankTaskBuilder(ITask task)
    {
        this.task = task;
    }

    public PlankTaskBuilder WithName(string name)
    {
        this.task.Name = name;
        return this;
    }

    public PlankTaskBuilder WithDescription(string description)
    {
        this.task.Description = description;
        return this;
    }

    public PlankTaskBuilder WithDependencies(IEnumerable<string> dependencies)
    {
        this.task.Dependencies = dependencies.ToList();
        return this;
    }

    public PlankTaskBuilder WithDependencies(params string[] dependencies)
    {
        this.task.Dependencies = dependencies;
        return this;
    }

    public PlankTaskBuilder WithTimeout(TimeSpan timeout)
    {
        this.task.Timeout = (int)timeout.TotalMilliseconds;
        return this;
    }

    public PlankTaskBuilder WithTimeout(int timeout)
    {
        this.task.Timeout = timeout;
        return this;
    }

    public PlankTaskBuilder WithContinueOnError(bool continueOnError = true)
    {
        this.task.ContinueOnError = continueOnError;
        return this;
    }
}