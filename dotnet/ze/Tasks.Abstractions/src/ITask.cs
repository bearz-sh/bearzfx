namespace Ze.Tasks;

public interface ITask
{
    string Name { get; set; }

    string Id { get; }

    string? Description { get; set; }

    IReadOnlyList<string> Dependencies { get; set; }

    bool ContinueOnError { get; set; }

    int Timeout { get; set; }

    Task RunAsync(ITaskExecutionContext context, CancellationToken cancellationToken = default);
}