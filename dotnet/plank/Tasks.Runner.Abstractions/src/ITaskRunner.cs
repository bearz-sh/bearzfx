namespace Plank.Tasks.Runners;

public interface ITaskRunner
{
    Task<TaskRunnerResult> RunAsync(
        IDependencyCollection<ITask> tasks,
        ITaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default);
}