namespace Ze.Tasks.Runner.Yaml;

public class VarsBlock
{
    public Dictionary<string, object?> Values { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}