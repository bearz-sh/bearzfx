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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/results/PackageResult.cs

using Bearz.Extra.Strings;

using Cocoa.Logging;

using Microsoft.Extensions.Logging;

using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Cocoa.Results;

/// <summary>
///   Outcome of package installation.
/// </summary>
public sealed class PackageResult : Result
{
    public PackageResult(IPackageMetadata packageMetadata, string installLocation, string? source = null)
        : this(packageMetadata.Id, packageMetadata.Version.ToSafeString(), installLocation)
    {
        this.PackageMetadata = packageMetadata;
        this.Source = source ?? string.Empty;
    }

    public PackageResult(IPackageSearchMetadata packageSearch, string installLocation, string? source = null)
        : this(packageSearch.Identity.Id, packageSearch.Identity.Version.ToSafeString(), installLocation)
    {
        this.SearchMetadata = packageSearch;
        this.Source = string.Empty;
        var log = Log.For<PackageResult>();
        var sources = new List<Uri>();
        if (source.IsNullOrEmpty())
            return;
        try
        {
            sources.AddRange(
                source.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Uri(s)));
        }
        catch (Exception ex)
        {
            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug(ex, $"Unable to determine sources from '{source}'. Using value as is.");

            return;
        }

        this.Source = sources.FirstOrDefault(uri => uri.IsFile || uri.IsUnc).ToSafeString();
        /*
        var rp = Package as DataServicePackage;
        if (rp != null && rp.DownloadUrl != null)
        {
            SourceUri = rp.DownloadUrl.ToString();
            Source = sources.FirstOrDefault(uri => uri.IsBaseOf(rp.DownloadUrl)).to_string();
            if (string.IsNullOrEmpty(Source))
            {
                Source = sources.FirstOrDefault(uri => uri.DnsSafeHost == rp.DownloadUrl.DnsSafeHost).to_string();
            }
        }
        else
        {
            Source = sources.FirstOrDefault(uri => uri.IsFile || uri.IsUnc).to_string();
        }
        */
    }

    public PackageResult(
        IPackageMetadata packageMetadata,
        IPackageSearchMetadata packageSearch,
        string installLocation,
        string? source = null)
        : this(packageMetadata.Id, packageMetadata.Version.ToSafeString(), installLocation)
    {
        this.SearchMetadata = packageSearch;
        this.PackageMetadata = packageMetadata;
        this.Source = string.Empty;
        if (source.IsNullOrEmpty())
            return;

        var log = Log.For<PackageResult>();
        var sources = new List<Uri>();
        if (source.IsNullOrEmpty())
            return;
        try
        {
            sources.AddRange(
                source.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => new Uri(s)));
        }
        catch (Exception ex)
        {
            if (log.IsEnabled(LogLevel.Debug))
                log.LogDebug(ex, $"Unable to determine sources from '{source}'. Using value as is.");

            return;
        }

        this.Source = sources.FirstOrDefault(uri => uri.IsFile || uri.IsUnc).ToSafeString();
    }

    public PackageResult(string name, string version, string installLocation, string? source = null)
    {
        this.Name = name;
        this.Version = version;
        this.InstallLocation = installLocation;
        this.Source = source ?? string.Empty;
    }

    public bool Inconclusive
    {
        get { return this.Messages.Any(x => x.MessageType == ResultType.Inconclusive); }
    }

    public bool Warning
    {
        get { return this.Messages.Any(x => x.MessageType == ResultType.Warn); }
    }

    public string Name { get; private set; }

    public string Version { get; private set; }

    public IPackageMetadata? PackageMetadata { get; private set; }

    public IPackageSearchMetadata? SearchMetadata { get; private set; }

    public string InstallLocation { get; set; }

    public string Source { get; set; }

    public string? SourceUri { get; set; }

    public int ExitCode { get; set; }

    public PackageIdentity Identity
    {
        get { return new PackageIdentity(this.Name, NuGetVersion.Parse(this.Version));  }
    }

    public void ResetMetadata(IPackageMetadata metadata, IPackageSearchMetadata search)
    {
        this.PackageMetadata = metadata;
        this.SearchMetadata = search;
        this.Name = metadata.Id;
        this.Version = metadata.Version.ToSafeString();
    }
}