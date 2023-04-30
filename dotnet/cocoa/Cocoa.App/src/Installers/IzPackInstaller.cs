using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
/// izPack Installer Options.
/// </summary>
/// <remarks>
///   http://izpack.org/
///   https://izpack.atlassian.net/wiki/display/IZPACK/Installer+Runtime+Options
///   https://izpack.atlassian.net/wiki/display/IZPACK/Unattended+Installations.
/// </remarks>
public class IzPackInstaller : InstallerBase
{
    public IzPackInstaller(ILogger<IzPackInstaller> logger)
        : base(logger)
    {
        this.InstallExecutable = "java";
        this.SilentInstall = $"-jar \"{InstallTokens.InstallerLocation}\" -options-system";
        this.CustomInstallLocation = $"-DINSTALL_PATH=\"{InstallTokens.CustomInstallLocation}\"";
        this.UninstallExecutable = "java"; // currently untested
        this.SilentUninstall = $"-jar \"{InstallTokens.UninstallerLocation}\"";
    }

    public override InstallerType InstallerType => InstallerType.IzPack;
}