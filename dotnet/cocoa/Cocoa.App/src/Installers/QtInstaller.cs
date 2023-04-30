using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
///   QT Installer Options.
/// </summary>
/// <remarks>
///   https://doc.qt.io/qtinstallerframework/index.html
///   http://doc.qt.io/qtinstallerframework/operations.html.
/// </remarks>
public class QtInstaller : InstallerBase
{
    public QtInstaller(ILogger<QtInstaller> logger)
        : base(logger)
    {
        // http://doc.qt.io/qtinstallerframework/ifw-globalconfig.html
        // CustomInstallLocation = "targetdir={0}".format_with(InstallTokens.CUSTOM_INSTALL_LOCATION);
    }

    public override InstallerType InstallerType => InstallerType.QtInstaller;
}