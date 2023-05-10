namespace Plank.Tasks.Runner.Yaml;

public class InputBlock
{
    public string Name { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string DefaultValue { get; set; } = string.Empty;

    public bool Required { get; set; }
}