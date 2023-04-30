using Cocoa.Domain;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class ConfigCommandConfiguration
{
    public string? Name { get; set; }

    public string? ConfigValue { get; set; }

    public ConfigCommandType Command { get; set; }
}