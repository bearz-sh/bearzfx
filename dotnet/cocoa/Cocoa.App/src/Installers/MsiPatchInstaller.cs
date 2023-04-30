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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/installers/MsiPatchInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

public class MsiPatchInstaller : InstallerBase
{
    public MsiPatchInstaller(ILogger<MsiPatchInstaller> logger)
        : base(logger)
    {
        // todo: #2573 fully qualify msiexec
        this.InstallExecutable = "msiexec.exe";
        this.SilentInstall = $"/p \"{InstallTokens.InstallerLocation}\" /qn";

        // http://msdn.microsoft.com/en-us/library/aa371101.aspx
        this.NoReboot = "/norestart";
        this.LogFile = $"/l*v \"{InstallTokens.PackageLocation}\\MSP.Install.log\"";

        // https://msdn.microsoft.com/en-us/library/aa372064.aspx
        // http://apprepack.blogspot.com/2012/08/installdir-vs-targetdir.html
        this.CustomInstallLocation = string.Empty;

        // http://msdn.microsoft.com/en-us/library/aa370856.aspx
        this.Language = string.Empty;

        // http://msdn.microsoft.com/en-us/library/aa367559.aspx
        this.OtherInstallOptions = "REINSTALLMODE=sumo REINSTALL=ALL";
        this.UninstallExecutable = "msiexec.exe";
        this.SilentUninstall = $"/package {InstallTokens.UninstallerLocation} /qn";
        this.OtherUninstallOptions = "MSIPATCHREMOVE={PATCH_GUID_HERE}";

        // https://msdn.microsoft.com/en-us/library/aa376931.aspx
        // https://support.microsoft.com/en-us/kb/290158
        this.ValidInstallExitCodes = new[] { 0L, 1641, 3010 };

        // we allow unknown 1605/1614 b/c it may have already been uninstalled
        // and that's okay
        this.ValidUninstallExitCodes = new[] { 0L, 1605, 1614, 1641, 3010 };
    }

    public override InstallerType InstallerType => InstallerType.MsiPatch;
}