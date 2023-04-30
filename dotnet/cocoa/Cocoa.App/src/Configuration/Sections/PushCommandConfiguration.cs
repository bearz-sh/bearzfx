namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class PushCommandConfiguration
{
    public string? Key { get; set; }

    public string? DefaultSource { get; set; }
}