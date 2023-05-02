using Cocoa.Domain;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class FeatureCommandConfiguration
{
    public string? Name { get; set; }

    public FeatureCommandType Command { get; set; }
}