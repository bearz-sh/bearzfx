namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class MachineSourceConfiguration
{
    public string? Name { get; set; }

    public string? Key { get; set; }

    public string? Username { get; set; }

    public string? EncryptedPassword { get; set; }

    public int Priority { get; set; }

    public string? Certificate { get; set; }

    public string? EncryptedCertificatePassword { get; set; }

    public bool BypassProxy { get; set; }

    public bool AllowSelfService { get; set; }

    public bool VisibleToAdminsOnly { get; set; }
}