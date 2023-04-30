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
// source: https://github.com/chocolatey/choco/blob/370e36632670d3e3d25a874ea8175551891bca50/src/chocolatey/infrastructure.app/domain/installers/InnoSetupInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
///   InnoSetup Installer Options.
/// </summary>
/// <remarks>
///   http://www.jrsoftware.org/ishelp/index.php?topic=setupcmdline.
/// </remarks>
public class InnoSetupInstaller : InstallerBase
{
    public InnoSetupInstaller(ILogger<InnoSetupInstaller> logger)
        : base(logger)
    {
        this.SilentInstall = "/VERYSILENT";
        this.NoReboot = "/NORESTART /RESTARTEXITCODE=3010";
        this.LogFile = $"/LOG=\"{InstallTokens.PackageLocation}\\InnoSetup.Install.log\"";
        this.CustomInstallLocation = "/DIR=\"{0}\"";
        this.Language = $"/LANG={InstallTokens.Language}";
        this.OtherInstallOptions = "/SP- /SUPPRESSMSGBOXES /CLOSEAPPLICATIONS /FORCECLOSEAPPLICATIONS /NOICONS";
        this.SilentUninstall = "/VERYSILENT";
        this.OtherUninstallOptions = "/SUPPRESSMSGBOXES";

        // http://www.jrsoftware.org/ishelp/index.php?topic=setupexitcodes
        this.ValidInstallExitCodes = new[] { 0L, 3010 };
        this.ValidUninstallExitCodes = new[] { 0L };
    }

    public override InstallerType InstallerType => InstallerType.InnoSetup;
}