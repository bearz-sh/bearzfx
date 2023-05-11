using Plank.Tasks.Runners;

namespace Plank.Tasks.Runner.Yaml;

public interface IYamlTaskRunner : ITaskRunner
{
    Task<TaskRunnerResult> RunAsync(
        IYamlTaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default);
}