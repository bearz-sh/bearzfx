namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class ProxyConfiguration
{
    public string? Location { get; set; }

    public string? User { get; set; }

    public string? EncryptedPassword { get; set; }

    public string? BypassList { get; set; }

    public bool BypassOnLocal { get; set; }
}