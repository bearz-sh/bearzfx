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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/ChocolateyPackagePathResolver.cs

using Cocoa.Adapters;

using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Cocoa.Nuget;

public sealed class ChocolateyPackagePathResolver : PackagePathResolver
{
    private readonly IFileSystem filesystem;

    public ChocolateyPackagePathResolver(string rootDirectory, IFileSystem filesystem)
        : base(rootDirectory, useSideBySidePaths: false)
    {
        this.RootDirectory = rootDirectory;
        this.filesystem = filesystem;
    }

    public string RootDirectory { get; set; }

    public override string GetInstallPath(PackageIdentity packageIdentity)
        => this.GetInstallPath(packageIdentity.Id);

    public string GetInstallPath(string packageId)
        => this.filesystem.CombinePaths(this.RootDirectory, packageId);

    [Obsolete("This overload will be removed in a future version.")]
    public string GetInstallPath(string id, NuGetVersion version)
        => this.GetInstallPath(id);

    public override string GetPackageFileName(PackageIdentity packageIdentity)
        => packageIdentity.Id + NuGetConstants.PackageExtension;
}