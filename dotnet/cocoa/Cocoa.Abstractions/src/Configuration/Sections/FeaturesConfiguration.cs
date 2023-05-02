namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class FeaturesConfiguration
{
    public bool AutoUninstaller { get; set; }

    public bool ChecksumFiles { get; set; }

    public bool AllowEmptyChecksums { get; set; }

    public bool AllowEmptyChecksumsSecure { get; set; }

    public bool FailOnAutoUninstaller { get; set; }

    public bool FailOnStandardError { get; set; }

    public bool UsePowerShellHost { get; set; }

    public bool LogEnvironmentValues { get; set; }

    public bool LogWithoutColor { get; set; }

    public bool VirusCheck { get; set; }

    public bool FailOnInvalidOrMissingLicense { get; set; }

    public bool IgnoreInvalidOptionsSwitches { get; set; }

    public bool UsePackageExitCodes { get; set; }

    public bool UseEnhancedExitCodes { get; set; }

    public bool UseFipsCompliantChecksums { get; set; }

    public bool ShowNonElevatedWarnings { get; set; }

    public bool ShowDownloadProgress { get; set; }

    public bool StopOnFirstPackageFailure { get; set; }

    public bool UseRememberedArgumentsForUpgrades { get; set; }

    public bool IgnoreUnfoundPackagesOnUpgradeOutdated { get; set; }

    public bool SkipPackageUpgradesWhenNotInstalled { get; set; }

    public bool RemovePackageInformationOnUninstall { get; set; }

    public bool ExitOnRebootDetected { get; set; }

    public bool LogValidationResultsOnWarnings { get; set; }

    public bool UsePackageRepositoryOptimizations { get; set; }
}