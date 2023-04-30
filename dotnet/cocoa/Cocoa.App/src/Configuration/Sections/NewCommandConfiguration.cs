namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class NewCommandConfiguration
{
    public NewCommandConfiguration()
    {
        this.TemplateProperties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    }

    public string? TemplateName { get; set; }

    public string? Name { get; set; }

    public bool AutomaticPackage { get; set; }

    public IDictionary<string, string> TemplateProperties { get; private set; }

    public bool UseOriginalTemplate { get; set; }
}