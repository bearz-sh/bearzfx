// Copyright © 2023-Present Chocolatey Software, Inc
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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/NuGetEndpointResources.cs

using System.Collections.Concurrent;

using Cocoa.Logging;

using Microsoft.Extensions.Logging;

using NuGet.Protocol.Core.Types;

namespace Cocoa.Nuget;

public sealed class NuGetEndpointResources
{
    private static readonly ConcurrentDictionary<SourceRepository, NuGetEndpointResources> cachedResources = new();

    private readonly Lazy<DependencyInfoResource> dependencyInfoResource;
    private readonly Lazy<DownloadResource> downloadResource;
    private readonly Lazy<FindPackageByIdResource> findPackageResource;
    private readonly Lazy<ListResource> listResource;
    private readonly Lazy<PackageMetadataResource> packageMetadataResource;
    private readonly Lazy<PackageUpdateResource> packageUpdateResource;
    private readonly Lazy<PackageSearchResource> searchResource;
    private readonly ILogger log;

    private bool resolvingFailed;

    private NuGetEndpointResources(SourceRepository sourceRepository, ILogger? logger = null)
    {
        this.Source = sourceRepository;

        this.dependencyInfoResource = new Lazy<DependencyInfoResource>(this.ResolveResource<DependencyInfoResource>);
        this.downloadResource = new Lazy<DownloadResource>(this.ResolveResource<DownloadResource>);
        this.findPackageResource = new Lazy<FindPackageByIdResource>(this.ResolveResource<FindPackageByIdResource>);
        this.listResource = new Lazy<ListResource>(this.ResolveResource<ListResource>);
        this.packageMetadataResource = new Lazy<PackageMetadataResource>(this.ResolveResource<PackageMetadataResource>);
        this.packageUpdateResource = new Lazy<PackageUpdateResource>(this.ResolveResource<PackageUpdateResource>);
        this.searchResource = new Lazy<PackageSearchResource>(this.ResolveResource<PackageSearchResource>);
        this.log = logger ?? Log.For<NuGetEndpointResources>();
    }

    public DependencyInfoResource DependencyInfoResource
        => this.dependencyInfoResource.Value;

    public DownloadResource DownloadResource
        => this.downloadResource.Value;

    public FindPackageByIdResource FindPackageResource
        => this.findPackageResource.Value;

    public ListResource ListResource
        => this.listResource.Value;

    public PackageMetadataResource PackageMetadataResource
        => this.packageMetadataResource.Value;

    public PackageUpdateResource PackageUpdateResource
        => this.packageUpdateResource.Value;

    public PackageSearchResource SearchResource
        => this.searchResource.Value;

    public SourceRepository Source { get; private set; }

    public static NuGetEndpointResources GetResourcesBySource(SourceRepository source)
    {
        return cachedResources.GetOrAdd(source, (key) =>
        {
            var endpointResource = new NuGetEndpointResources(key);

            return endpointResource;
        });
    }

    public static IEnumerable<NuGetEndpointResources> GetResourcesBySource(IEnumerable<SourceRepository> sources)
    {
        foreach (SourceRepository source in sources)
        {
            yield return GetResourcesBySource(source);
        }
    }

    private T ResolveResource<T>()
        where T : class, INuGetResource
    {
        T resource = default!;

        try
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug("Resolving resource {0} for source {1}", typeof(T).Name, this.Source.PackageSource.Source);

            resource = this.Source.GetResource<T>();
        }
        catch (AggregateException ex) when (!(ex.InnerException is null))
        {
            if (!this.resolvingFailed)
            {
                if (this.log.IsEnabled(LogLevel.Warning))
                    this.log.LogWarning(ex, ex.InnerException.Message);

                this.resolvingFailed = true;
            }
        }

        if (resource == default && this.log.IsEnabled(LogLevel.Warning))
        {
            using var scope = this.log.BeginScope(new Dictionary<string, string>()
            {
                ["ui"] = "false",
            });

            this.log.LogWarning(
                "The source {0} failed to get a {1} resource",
                this.Source.PackageSource.Source,
                typeof(T));
        }

        return resource!;
    }
}