using Cocoa.Domain;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class SourcesCommandConfiguration
{
    public string? Name { get; set; }

    public SourceCommandType Command { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public int Priority { get; set; }

    public string? Certificate { get; set; }

    public string? CertificatePassword { get; set; }

    public bool BypassProxy { get; set; }

    public bool AllowSelfService { get; set; }

    public bool VisibleToAdminsOnly { get; set; }
}