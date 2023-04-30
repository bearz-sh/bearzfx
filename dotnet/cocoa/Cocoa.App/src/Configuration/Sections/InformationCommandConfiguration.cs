using Cocoa.Platform;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class InformationCommandConfiguration
{
    // application set variables
    public PlatformType PlatformType { get; set; }

    public Version PlatformVersion { get; set; } = new(0, 0);

    public string? PlatformName { get; set; }

    public string? ChocolateyVersion { get; set; }

    public string? ChocolateyProductVersion { get; set; }

    public string? FullName { get; set; }

    public bool Is64BitOperatingSystem { get; set; }

    public bool Is64BitProcess { get; set; }

    public bool IsInteractive { get; set; }

    public string? UserName { get; set; }

    public string? UserDomainName { get; set; }

    public bool IsUserAdministrator { get; set; }

    public bool IsUserSystemAccount { get; set; }

    public bool IsUserRemoteDesktop { get; set; }

    public bool IsUserRemote { get; set; }

    public bool IsProcessElevated { get; set; }

    public bool IsLicensedVersion { get; set; }

    public string? LicenseType { get; set; }

    public string? CurrentDirectory { get; set; }
}