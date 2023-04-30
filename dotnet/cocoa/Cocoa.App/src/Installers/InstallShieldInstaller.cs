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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/installers/InstallShieldInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
/// InstallShield Installer Options.
/// </summary>
/// <remarks>
///   http://helpnet.installshield.com/installshield18helplib/ihelpsetup_execmdline.htm.
/// </remarks>
public class InstallShieldInstaller : InstallerBase
{
    public InstallShieldInstaller(ILogger<InstallShieldInstaller> logger)
        : base(logger)
    {
        this.SilentInstall = "/s /v\"/qn\"";
        this.NoReboot = "/v\"REBOOT=ReallySuppress\"";
        this.LogFile = $"/f2\"{InstallTokens.PackageLocation}\\MSI.Install.log\"";
        this.CustomInstallLocation = $"/v\"INSTALLDIR=\\\"{InstallTokens.CustomInstallLocation}\\\"";
        this.Language = $"/l\"{InstallTokens.Language}\"";
        this.OtherInstallOptions = "/sms"; // pause
        this.SilentUninstall = "/uninst /s";
        this.OtherUninstallOptions = "/sms";

        // http://helpnet.installshield.com/installshield18helplib/IHelpSetup_EXEErrors.htm
        this.ValidInstallExitCodes = new[] { 0L, 1641, 3010 };
        this.ValidUninstallExitCodes = new[] { 0L, 1605, 1614, 1641, 3010 };
    }

    public override InstallerType InstallerType => InstallerType.InstallShield;
}