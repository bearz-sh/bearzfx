using Plank.Tasks.Runners;

namespace Plank.Tasks.Runner.Yaml;

public class YamlTaskRunOptions : TaskRunOptions, IYamlTaskRunOptions
{
    public string? TaskFile { get; set;  }
}