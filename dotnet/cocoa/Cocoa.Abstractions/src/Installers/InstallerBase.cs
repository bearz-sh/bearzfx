using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

public abstract class InstallerBase : IInstaller
{
    protected InstallerBase(ILogger logger)
    {
        this.InstallExecutable = $"\"{InstallTokens.InstallerLocation}\"";
        this.UninstallExecutable = $"\"{InstallTokens.UninstallerLocation}\"";

        this.Log = logger;
    }

    public abstract InstallerType InstallerType { get; }

    public string? InstallExecutable { get; protected set; }

    public string? SilentInstall { get; protected set; }

    public string? NoReboot { get; protected set; }

    public string? LogFile { get; protected set; }

    public string? OtherInstallOptions { get; protected set; }

    public string? CustomInstallLocation { get; protected set; } = string.Empty;

    public string? Language { get; protected set; } = string.Empty;

    public string? UninstallExecutable { get; protected set; }

    public string? SilentUninstall { get; protected set; } = string.Empty;

    public string? OtherUninstallOptions { get; protected set; }

    public IReadOnlyList<long> ValidInstallExitCodes { get; protected set; } = new[] { 0L };

    public IReadOnlyList<long> ValidUninstallExitCodes { get; protected set; } = new[] { 0L };

    protected ILogger Log { get; }

    public virtual CommandArgs BuildInstallArguments(bool logFileRequested, bool customInstallLocation, bool languageRequested)
    {
        var args = new CommandArgs();
        if (!this.SilentInstall.IsNullOrWhiteSpace())
            args.Add(this.SilentInstall);

        if (!this.NoReboot.IsNullOrWhiteSpace())
            args.Add(this.NoReboot);

        if (!this.OtherInstallOptions.IsNullOrWhiteSpace())
            args.Add(this.OtherInstallOptions);

        if (languageRequested && !this.Language.IsNullOrWhiteSpace())
        {
            if (this.Language.IsNullOrWhiteSpace())
                this.Log.LogDebug($"Language was requested for {this.InstallerType} but not set");
            else
                args.Add(this.Language);
        }

        if (logFileRequested)
        {
            if (this.LogFile.IsNullOrWhiteSpace())
                this.Log.LogDebug($"Logging was requested for {this.InstallerType} but the log file was not set");
            else
                args.Add(this.LogFile);
        }

        if (customInstallLocation && !this.CustomInstallLocation.IsNullOrWhiteSpace())
            args.Add(this.CustomInstallLocation);

        return args;
    }

    public virtual CommandArgs BuildUninstallArguments()
    {
        // MSI has issues with 1622 - opening a log file location
        var args = new CommandArgs();

        if (!this.SilentUninstall.IsNullOrWhiteSpace())
            args.Add(this.SilentUninstall);

        if (!this.NoReboot.IsNullOrWhiteSpace())
            args.Add(this.NoReboot);

        if (!this.OtherUninstallOptions.IsNullOrWhiteSpace())
            args.Add(this.OtherUninstallOptions);

        return args;
    }
}