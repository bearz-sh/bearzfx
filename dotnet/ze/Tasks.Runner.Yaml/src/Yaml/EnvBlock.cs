namespace Ze.Tasks.Runner.Yaml;

public class EnvBlock
{
    public Dictionary<string, string?> Envs { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}