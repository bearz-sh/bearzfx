namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class UpgradeCommandConfiguration
{
    public bool FailOnUnfound { get; set; }

    public bool FailOnNotInstalled { get; set; }

    public bool NotifyOnlyAvailableUpgrades { get; set; }

    public string? PackageNamesToSkip { get; set; }

    public bool ExcludePrerelease { get; set; }
}