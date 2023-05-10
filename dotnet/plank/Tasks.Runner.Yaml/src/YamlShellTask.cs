namespace Plank.Tasks.Runner.Yaml;

public class YamlShellTask : ShellTask
{
    public YamlShellTask(string name)
        : base(name)
    {
    }

    public YamlShellTask(string name, string id)
        : base(name, id)
    {
    }

    public Dictionary<string, InputBlock> Inputs { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}