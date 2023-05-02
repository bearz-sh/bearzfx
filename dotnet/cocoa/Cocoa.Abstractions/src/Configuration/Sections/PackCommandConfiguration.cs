namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class PackCommandConfiguration
{
    public PackCommandConfiguration()
    {
        this.Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    public IDictionary<string, string> Properties { get; private set; }

    public bool PackThrowOnUnsupportedElements { get; } = true;
}