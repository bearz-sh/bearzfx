namespace Bearz.Text.DotEnv;

public class DotEnvSerializerOptions
{
    public bool AllowBackticks { get; set; } = true;

    public bool AllowJson { get; set; }

    public bool AllowYaml { get; set; }

    public bool Expand { get; set; } = true;

    public IDictionary<string, string>? ExpandVariables { get; set; }
}