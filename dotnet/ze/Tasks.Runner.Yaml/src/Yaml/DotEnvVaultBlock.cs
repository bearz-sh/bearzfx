namespace Ze.Tasks.Runner.Yaml;

public class DotEnvVaultBlock : VaultBlock
{
    public string? Path { get; set; }

    public bool Sops { get; set; } = true;

    public string? Prefix { get; set; }
}