using Plank.Tasks.Runners;

namespace Plank.Tasks.Runner.Yaml;

public interface IYamlTaskRunOptions : ITaskRunOptions
{
    string? TaskFile { get; }
}