namespace Ze.Tasks.Runner.Yaml;

public class DefaultsBlock
{
    public string? Run { get; set; }

    public string? WorkingDirectory { get; set; }

    public string? Stdio { get; set; } = "inherit";
}