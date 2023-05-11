using Ze.Tasks.Runners;

namespace Ze.Tasks.Runner.Yaml;

public class YamlTaskRunOptions : TaskRunOptions, IYamlTaskRunOptions
{
    public string? TaskFile { get; set;  }
}