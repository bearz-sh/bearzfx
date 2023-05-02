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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/NugetCommon.cs

using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Bearz.Extra.Strings;
using Chocolatey.NuGet.Frameworks;
using Cocoa.Adapters;
using Cocoa.Configuration;
using Cocoa.Logging;
using Cocoa.Results;
using Cocoa.Services;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Credentials;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

using ILogger = NuGet.Common.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Cocoa.Nuget;

public static class NugetCommon
{
    private static readonly ConcurrentDictionary<string, SourceRepository> repositories = new ConcurrentDictionary<string, SourceRepository>();

    [Obsolete("This overload is obsolete and will be removed in a future version.")]
    public static ChocolateyPackagePathResolver GetPathResolver(ChocolateyConfiguration configuration, IFileSystem nugetPackagesFileSystem)
        => GetPathResolver(nugetPackagesFileSystem);

    public static ChocolateyPackagePathResolver GetPathResolver(IFileSystem nugetPackagesFileSystem)
    {
        return new ChocolateyPackagePathResolver(ApplicationParameters.Paths.PackagesDirectory, nugetPackagesFileSystem);
    }

    public static void ClearRepositoriesCache()
    {
        repositories.Clear();
    }

    public static SourceRepository GetLocalRepository()
    {
        var nugetSource = new PackageSource(ApplicationParameters.Paths.PackagesDirectory);
        return Repository.Factory.GetCoreV3(nugetSource);
    }

    public static IEnumerable<SourceRepository> GetRemoteRepositories(ChocolateyConfiguration configuration, ILogger nugetLogger, IFileSystem filesystem, ICredentialsPromptProvider promptProvider)
    {
        // Set user agent for all NuGet library calls. Should not affect any HTTP calls that Chocolatey itself would make.
        UserAgent.SetUserAgentString(new UserAgentStringBuilder($"{ApplicationParameters.UserAgent}/{configuration.Information.ChocolateyProductVersion} via NuGet Client"));

        // ensure credentials can be grabbed from configuration
        SetHttpHandlerCredentialService(configuration, promptProvider);
        var log = Log.For("chocolatey");
        var debugEnabled = log.IsEnabled(LogLevel.Debug);

        if (!string.IsNullOrWhiteSpace(configuration.Proxy.Location))
        {
            if (debugEnabled)
                log.LogDebug($"Using proxy server '{configuration.Proxy.Location}'.");

            var proxy = new System.Net.WebProxy(configuration.Proxy.Location, true);

            if (!string.IsNullOrWhiteSpace(configuration.Proxy.User) && !string.IsNullOrWhiteSpace(configuration.Proxy.EncryptedPassword))
            {
                if (RuntimeEnvironmentHelper.IsWindows || RuntimeEnvironmentHelper.IsMono)
                {
                    proxy.Credentials = new NetworkCredential(
                        configuration.Proxy.User,
                        EncryptionUtility.DecryptString(configuration.Proxy.EncryptedPassword));
                }
                else
                {
                    proxy.Credentials = new NetworkCredential(
                        configuration.Proxy.User,
                        configuration.Proxy.EncryptedPassword);
                }
            }

            if (!configuration.Proxy.BypassList.IsNullOrWhiteSpace())
            {
                if (debugEnabled)
                    log.LogDebug($"Proxy has a bypass list of '{configuration.Proxy.BypassList}'.");

                proxy.BypassList = configuration.Proxy.BypassList.Replace("*", ".*").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            proxy.BypassProxyOnLocal = configuration.Proxy.BypassOnLocal;

            ProxyCache.Instance.Override(proxy);
        }
        else
        {
            ProxyCache.Instance.Override(new System.Net.WebProxy());
        }

        var sources = configuration.Sources.ToSafeString().Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !s.IsNullOrWhiteSpace()).ToList();

        IList<SourceRepository> repositories = new List<SourceRepository>();

        var updatedSources = new StringBuilder();
        foreach (var sourceValue in sources)
        {
            var source = sourceValue;

            var sourceClientCertificates = new List<X509Certificate>();
            if (!configuration.SourceCommand.Certificate.IsNullOrWhiteSpace())
            {
                if (debugEnabled)
                    log.LogDebug($"Using passed in certificate for source {source}");

                sourceClientCertificates.Add(new X509Certificate2(configuration.SourceCommand.Certificate, configuration.SourceCommand.CertificatePassword));
            }

            if (configuration.MachineSources.Any(m => m.Name.IsEqualTo(source) || m.Key.IsEqualTo(source)))
            {
                try
                {
                    var machineSource = configuration.MachineSources.FirstOrDefault(m => m.Key.IsEqualTo(source));
                    if (machineSource == null)
                    {
                        machineSource = configuration.MachineSources.FirstOrDefault(m => m.Name.IsEqualTo(source));
                        if (debugEnabled)
                            log.LogDebug($"Switching source name {source} to actual source value '{machineSource?.Key.ToSafeString()}'.");

                        if (!machineSource?.Key?.IsNullOrWhiteSpace() == true)
                            source = machineSource?.Key;
                    }

                    if (machineSource != null)
                    {
                        bool bypassProxy = machineSource.BypassProxy;
                        if (bypassProxy && debugEnabled)
                            log.LogDebug($"Source '{source}' is configured to bypass proxies.");

                        if (!machineSource.Certificate.IsNullOrWhiteSpace())
                        {
                            if (debugEnabled)
                                log.LogDebug($"Using configured certificate for source {source}");

                            X509Certificate2 cert;
                            if (RuntimeEnvironmentHelper.IsWindows || RuntimeEnvironmentHelper.IsMono)
                            {
                                cert = new X509Certificate2(
                                    machineSource.Certificate,
                                    EncryptionUtility.DecryptString(machineSource.EncryptedCertificatePassword));
                            }
                            else
                            {
                                cert = new X509Certificate2(
                                    machineSource.Certificate,
                                    machineSource.EncryptedCertificatePassword);
                            }

                            sourceClientCertificates.Add(cert);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (log.IsEnabled(LogLevel.Warning))
                        log.LogWarning($"Attempted to use a source name {sourceValue} to get default source but failed:{Environment.NewLine}{ex.Message}");
                }
            }

            if (source.IsNullOrWhiteSpace())
            {
                log.LogDebug("Skipping empty source");
                continue;
            }

            if (NugetCommon.repositories.ContainsKey(source))
            {
                repositories.Add(NugetCommon.repositories[source]);
            }
            else
            {
                var nugetSource = new PackageSource(source);

                // If not parsed as a http(s) or local source, let's try resolving the path
                // Since NuGet.Client is not able to parse all relative paths
                // Conversion to absolute paths is handled by clients, not by the libraries as per
                // https://github.com/NuGet/NuGet.Client/pull/3783
                if (nugetSource.TrySourceAsUri is null)
                {
                    string fullsource;
                    try
                    {
                        fullsource = filesystem.GetFullPath(source);
                    }
                    catch
                    {
                        // If an invalid source was passed in, we don't care here, pass it along
                        fullsource = source;
                    }

                    nugetSource = new PackageSource(fullsource);

                    if (!nugetSource.IsLocal)
                    {
                        throw new InvalidOperationException($"Source '{source}' is unable to be parsed");
                    }

                    if (debugEnabled)
                        log.LogDebug($"Updating Source path from {source} to {fullsource}");

                    updatedSources.AppendFormat("{0};", fullsource);
                }
                else
                {
                    updatedSources.AppendFormat("{0};", source);
                }

                nugetSource.ClientCertificates = sourceClientCertificates;
                var repo = Repository.Factory.GetCoreV3(nugetSource);

                if (nugetSource.IsHttp || nugetSource.IsHttps)
                {
#pragma warning disable RS0030 // Do not used banned APIs
                    var httpSourceResource = repo.GetResource<HttpSourceResource>();
#pragma warning restore RS0030 // Do not used banned APIs
                    if (httpSourceResource != null)
                    {
                        httpSourceResource.HttpSource.HttpCacheDirectory = ApplicationParameters.Paths.HttpCacheDirectory;
                    }
                }

                NugetCommon.repositories.TryAdd(source, repo);
                repositories.Add(repo);
            }
        }

        if (updatedSources.Length != 0)
        {
            configuration.Sources = updatedSources.Remove(updatedSources.Length - 1, 1).ToSafeString();
        }

        return repositories;
    }

    public static IReadOnlyList<NuGetEndpointResources> GetRepositoryResources(ChocolateyConfiguration configuration, ILogger nugetLogger, IFileSystem filesystem, ICredentialsPromptProvider promptProvider)
    {
        IEnumerable<SourceRepository> remoteRepositories = GetRemoteRepositories(configuration, nugetLogger, filesystem, promptProvider);
        return GetRepositoryResources(remoteRepositories);
    }

    public static IReadOnlyList<NuGetEndpointResources> GetRepositoryResources(IEnumerable<SourceRepository> packageRepositories)
    {
        return NuGetEndpointResources.GetResourcesBySource(packageRepositories).ToList();
    }

    public static void SetHttpHandlerCredentialService(ChocolateyConfiguration configuration, ICredentialsPromptProvider credentialsPromptProvider)
    {
        HttpHandlerResourceV3.CredentialService = new Lazy<ICredentialService>(
            () => new CredentialService(
                new AsyncLazy<IEnumerable<ICredentialProvider>>(
                    () => GetCredentialProvidersAsync(configuration, credentialsPromptProvider)),
                false,
                true));
    }

    public static void GetLocalPackageDependencies(
        PackageIdentity package,
        NuGetFramework framework,
        IEnumerable<PackageResult> allLocalPackages,
        ISet<SourcePackageDependencyInfo> dependencyInfos)
    {
        if (dependencyInfos.Contains(package))
            return;

        var packages = allLocalPackages.ToList();
        var metadata = packages
            .FirstOrDefault(p => p.PackageMetadata?.Id.Equals(package.Id, StringComparison.OrdinalIgnoreCase) == true
                                 && p.PackageMetadata.Version.Equals(package.Version))
            ?.PackageMetadata;

        var group = NuGetFrameworkUtility.GetNearest<PackageDependencyGroup>(metadata?.DependencyGroups, framework);
        var dependencies = (group?.Packages ?? Enumerable.Empty<PackageDependency>()).ToList();

        var result = new SourcePackageDependencyInfo(
            package,
            dependencies,
            true,
            null,
            null,
            null);

        dependencyInfos.Add(result);

        foreach (var dependency in dependencies)
        {
            GetLocalPackageDependencies(dependency.Id, dependency.VersionRange, framework, packages, dependencyInfos);
        }
    }

    public static void GetLocalPackageDependencies(
        string packageId,
        VersionRange versionRange,
        NuGetFramework framework,
        IEnumerable<PackageResult> allLocalPackages,
        ISet<SourcePackageDependencyInfo> dependencyInfos)
    {
        var packages = allLocalPackages.ToList();
        var versionsMetadata = packages
            .Where(p => p.PackageMetadata?.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase) == true &&
                        versionRange.Satisfies(p.PackageMetadata.Version))
            .Select(p => p.PackageMetadata);

        foreach (var metadata in versionsMetadata)
        {
            if (metadata is null)
                continue;

            var group = NuGetFrameworkUtility.GetNearest<PackageDependencyGroup>(metadata.DependencyGroups, framework);
            var dependencies = (group?.Packages ?? Enumerable.Empty<PackageDependency>()).ToList();

            var result = new SourcePackageDependencyInfo(
                metadata.Id,
                metadata.Version,
                dependencies,
                true,
                null,
                null,
                null);

            if (dependencyInfos.Contains(result)) return;
            dependencyInfos.Add(result);

            foreach (var dependency in dependencies)
            {
                GetLocalPackageDependencies(dependency.Id, dependency.VersionRange, framework, packages, dependencyInfos);
            }
        }
    }

    public static async Task GetPackageDependencies(
        PackageIdentity package,
        NuGetFramework framework,
        SourceCacheContext cacheContext,
        ILogger logger,
        IEnumerable<NuGetEndpointResources> resources,
        ISet<SourcePackageDependencyInfo> availablePackages,
        ISet<PackageDependency> dependencyCache,
        ChocolateyConfiguration configuration)
    {
        if (availablePackages.Contains(package))
            return;

        var resourceList = resources.ToList();
        var dependencyInfoResources = resourceList.DependencyInfoResources();
        var log = Log.For("chocolatey");
        foreach (var dependencyInfoResource in dependencyInfoResources)
        {
            SourcePackageDependencyInfo? dependencyInfo = null;

            try
            {
                dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None);
            }
            catch (AggregateException ex) when (ex.InnerException is not null)
            {
                if (log.IsEnabled(LogLevel.Warning))
                    log.LogWarning(ex, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (log.IsEnabled(LogLevel.Warning))
                    log.LogWarning(ex, ex.InnerException?.Message);
            }

            if (dependencyInfo == null) continue;

            availablePackages.Add(dependencyInfo);
            foreach (var dependency in dependencyInfo.Dependencies)
            {
                if (dependencyCache.Contains(dependency)) continue;
                dependencyCache.Add(dependency);
                await GetPackageDependencies(
                    dependency.Id, framework, cacheContext, logger, resourceList, availablePackages, dependencyCache, configuration);
            }
        }
    }

    public static async Task GetPackageDependencies(
        string packageId,
        NuGetFramework framework,
        SourceCacheContext cacheContext,
        ILogger logger,
        IEnumerable<NuGetEndpointResources> resources,
        ISet<SourcePackageDependencyInfo> availablePackages,
        ISet<PackageDependency> dependencyCache,
        ChocolateyConfiguration configuration)
    {
        var log = Log.For("chocolatey");
        var resourcesList = resources.ToList();
        var dependencyInfoResources = resourcesList.DependencyInfoResources();

        foreach (var dependencyInfoResource in dependencyInfoResources)
        {
            IEnumerable<SourcePackageDependencyInfo> dependencyInfos = Array.Empty<SourcePackageDependencyInfo>();

            try
            {
                dependencyInfos = await dependencyInfoResource.ResolvePackages(
                    packageId, configuration.Prerelease, framework, cacheContext, logger, CancellationToken.None);
            }
            catch (AggregateException ex) when (!(ex.InnerException is null))
            {
                if (log.IsEnabled(LogLevel.Warning))
                    log.LogWarning(ex, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                if (log.IsEnabled(LogLevel.Warning))
                    log.LogWarning(ex, ex.InnerException?.Message);
            }

            var deps = dependencyInfos.ToList();

            if (deps.Count == 0)
                continue;

            availablePackages.AddRange(deps);
            foreach (var dependency in deps.SelectMany(p => p.Dependencies))
            {
                if (dependencyCache.Contains(dependency)) continue;
                dependencyCache.Add(dependency);

                // Recursion is fun, kids
                await GetPackageDependencies(
                    dependency.Id, framework, cacheContext, logger, resourcesList, availablePackages, dependencyCache, configuration);
            }
        }
    }

    public static async Task GetPackageParents(
        string packageId,
        ISet<SourcePackageDependencyInfo> parentPackages,
        IEnumerable<SourcePackageDependencyInfo> locallyInstalledPackages)
    {
        var packages = locallyInstalledPackages.ToList();
        foreach (var package in packages.Where(p => !parentPackages.Contains(p)))
        {
            if (parentPackages.Contains(package)) continue;
            if (package.Dependencies.Any(p => p.Id.Equals(packageId, StringComparison.OrdinalIgnoreCase)))
            {
                parentPackages.Add(package);
                await GetPackageParents(package.Id, parentPackages, packages);
            }
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously.

    // We don't care about this method being synchronous because this is just used to pass in the credential provider to Lazy<ICredentialService>
    private static async Task<IEnumerable<ICredentialProvider>> GetCredentialProvidersAsync(ChocolateyConfiguration configuration, ICredentialsPromptProvider credentialsPromptProvider)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return new List<ICredentialProvider>() { new ChocolateyNugetCredentialProvider(configuration, credentialsPromptProvider) };
    }
}