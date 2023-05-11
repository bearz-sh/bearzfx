namespace Ze.Tasks.Runners;

public interface IJobRunner
{
    Task<JobRunnerResult> RunAsync(
        IDependencyCollection<IJob> jobs,
        IJobRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default);
}