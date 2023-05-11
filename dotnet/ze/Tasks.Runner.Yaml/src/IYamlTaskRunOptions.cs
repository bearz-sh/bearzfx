using Ze.Tasks.Runners;

namespace Ze.Tasks.Runner.Yaml;

public interface IYamlTaskRunOptions : ITaskRunOptions
{
    string? TaskFile { get; }
}