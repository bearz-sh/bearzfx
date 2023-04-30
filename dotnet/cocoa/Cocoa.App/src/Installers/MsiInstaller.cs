// Copyright © 2017 - 2021 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/installers/MsiInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
///   Windows Installer (MsiExec) Options.
/// </summary>
/// <remarks>
///   http://msdn.microsoft.com/en-us/library/aa367988.aspx
///   http://msdn.microsoft.com/en-us/library/aa372024.aspx
///   http://support.microsoft.com/kb/227091
///   http://www.advancedinstaller.com/user-guide/msiexec.html
///   1603 search for return value 3 http://blogs.msdn.com/b/astebner/archive/2005/08/01/446328.aspx.
/// </remarks>
public class MsiInstaller : InstallerBase
{
    public MsiInstaller(ILogger<MsiInstaller> logger)
        : base(logger)
    {
        // todo: #2573 fully qualify msiexec
        this.InstallExecutable = "msiexec.exe";

        // /quiet
        this.SilentInstall = $"/i \"{InstallTokens.InstallerLocation}\" /qn";

        // http://msdn.microsoft.com/en-us/library/aa371101.aspx
        // REBOOT=ReallySuppress
        this.NoReboot = "/norestart";
        this.LogFile = $"/l*v \"{InstallTokens.PackageLocation}\\MSI.Install.log\"";

        // https://msdn.microsoft.com/en-us/library/aa372064.aspx
        // http://apprepack.blogspot.com/2012/08/installdir-vs-targetdir.html
        this.CustomInstallLocation = $"TARGETDIR=\"{InstallTokens.CustomInstallLocation}\"";

        // http://msdn.microsoft.com/en-us/library/aa370856.aspx
        this.Language = $"ProductLanguage={InstallTokens.Language}";

        // http://msdn.microsoft.com/en-us/library/aa367559.aspx
        this.OtherInstallOptions = "ALLUSERS=1 DISABLEDESKTOPSHORTCUT=1 ADDDESKTOPICON=0 ADDSTARTMENU=0";
        this.UninstallExecutable = "msiexec.exe";

        // todo: eventually will need this
        // SilentUninstall = "/qn /x{0}".format_with(InstallTokens.UNINSTALLER_LOCATION);
        this.SilentUninstall = "/qn";

        // https://msdn.microsoft.com/en-us/library/aa376931.aspx
        // https://support.microsoft.com/en-us/kb/290158
        this.ValidInstallExitCodes = new[] { 0L, 1641, 3010 };

        // we allow unknown 1605/1614 b/c it may have already been uninstalled
        // and that's okay
        this.ValidUninstallExitCodes = new[] { 0L, 1605, 1614, 1641, 3010 };
    }

    public override InstallerType InstallerType => InstallerType.Msi;
}