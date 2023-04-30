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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/installers/WiseInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
///   WISE Options.
/// </summary>
/// <remarks>
///   https://support.symantec.com/en_US/article.HOWTO5865.html
///   http://www.itninja.com/blog/view/wise-setup-exe-switches
///   While we can override the extraction path, it should already be overridden
///   because we are overriding the TEMP variable.
/// </remarks>
public class WiseInstaller : InstallerBase
{
    public WiseInstaller(ILogger<WiseInstaller> logger)
        : base(logger)
    {
        this.SilentInstall = "/s";
        this.SilentUninstall = "/s";

        // http://www.itninja.com/question/wise-package-install-switches-for-install-path
        this.CustomInstallLocation = null;

        // http://www.symantec.com/connect/blogs/wisescript-command-line-options
        this.OtherUninstallOptions = $"\"{InstallTokens.TempLocation}\\Uninstall.Log\"";
    }

    public override InstallerType InstallerType => InstallerType.Wise;
}