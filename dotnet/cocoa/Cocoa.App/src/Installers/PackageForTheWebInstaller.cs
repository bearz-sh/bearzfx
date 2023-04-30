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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/installers/PackageForTheWebInstaller.cs

using Microsoft.Extensions.Logging;

namespace Cocoa.Installers;

/// <summary>
///   PackageForTheWeb Options.
/// </summary>
public class PackageForTheWebInstaller : InstallerBase
{
    public PackageForTheWebInstaller(ILogger<PackageForTheWebInstaller> logger)
        : base(logger)
    {
        this.SilentInstall = "/s";
        this.SilentUninstall = "/s";
    }

    public override InstallerType InstallerType => InstallerType.PackageForTheWeb;
}