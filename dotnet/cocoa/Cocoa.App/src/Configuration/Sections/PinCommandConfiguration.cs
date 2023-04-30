using Cocoa.Domain;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class PinCommandConfiguration
{
    public string? Name { get; set; }

    public PinCommandType Command { get; set; }
}