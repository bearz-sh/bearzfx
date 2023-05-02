// Copyright © 2017 - 2022 Chocolatey Software, Inc
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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/domain/ChocolateyPackageMetadata.cs

using Cocoa.Adapters;

using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Cocoa.Domain;

public class ChocolateyPackageMetadata : IPackageMetadata
{
    public ChocolateyPackageMetadata(NuspecReader reader)
    {
        this.ProjectSourceUrl = GetUriSafe(reader.GetProjectSourceUrl());
        this.PackageSourceUrl = GetUriSafe(reader.GetPackageSourceUrl());
        this.DocsUrl = GetUriSafe(reader.GetDocsUrl());
        this.WikiUrl = GetUriSafe(reader.GetWikiUrl());
        this.MailingListUrl = GetUriSafe(reader.GetMailingListUrl());
        this.BugTrackerUrl = GetUriSafe(reader.GetBugTrackerUrl());
        this.Replaces = reader.GetReplaces().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        this.Provides = reader.GetProvides().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        this.Conflicts = reader.GetConflicts().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        this.SoftwareDisplayName = reader.GetSoftwareDisplayName();
        this.SoftwareDisplayVersion = reader.GetSoftwareDisplayVersion();
        this.Id = reader.GetId();
        this.Version = reader.GetVersion();
        this.Title = reader.GetTitle();
        this.Authors = reader.GetAuthors().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        this.Owners = reader.GetOwners().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        this.IconUrl = GetUriSafe(reader.GetIconUrl());
        this.LicenseUrl = GetUriSafe(reader.GetLicenseUrl());
        this.ProjectUrl = GetUriSafe(reader.GetProjectUrl());
        this.RequireLicenseAcceptance = reader.GetRequireLicenseAcceptance();
        this.DevelopmentDependency = reader.GetDevelopmentDependency();
        this.Description = reader.GetDescription();
        this.Summary = reader.GetSummary();
        this.ReleaseNotes = reader.GetReleaseNotes();
        this.Language = reader.GetLanguage();
        this.Tags = reader.GetTags();
        this.Serviceable = reader.IsServiceable();
        this.Copyright = reader.GetCopyright();
        this.Icon = reader.GetIcon();
        this.Readme = reader.GetReadme();
        this.DependencyGroups = reader.GetDependencyGroups();
        this.PackageTypes = reader.GetPackageTypes();
        this.Repository = reader.GetRepositoryMetadata();
        this.LicenseMetadata = reader.GetLicenseMetadata();
        this.FrameworkReferenceGroups = reader.GetFrameworkRefGroups();
    }

    public ChocolateyPackageMetadata(string packagePath, IFileSystem filesystem)
    {
        if (filesystem.GetFileExtension(packagePath) == NuGetConstants.PackageExtension)
        {
            using PackageArchiveReader archiveReader = new(packagePath);
            NuspecReader? reader = archiveReader.NuspecReader;

            this.ProjectSourceUrl = GetUriSafe(reader.GetProjectSourceUrl());
            this.PackageSourceUrl = GetUriSafe(reader.GetPackageSourceUrl());
            this.DocsUrl = GetUriSafe(reader.GetDocsUrl());
            this.WikiUrl = GetUriSafe(reader.GetWikiUrl());
            this.MailingListUrl = GetUriSafe(reader.GetMailingListUrl());
            this.BugTrackerUrl = GetUriSafe(reader.GetBugTrackerUrl());
            this.Replaces = reader.GetReplaces().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Provides = reader.GetProvides().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Conflicts = reader.GetConflicts().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.SoftwareDisplayName = reader.GetSoftwareDisplayName();
            this.SoftwareDisplayVersion = reader.GetSoftwareDisplayVersion();
            this.Id = reader.GetId();
            this.Version = reader.GetVersion();
            this.Title = reader.GetTitle();
            this.Authors = reader.GetAuthors().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Owners = reader.GetOwners().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.IconUrl = GetUriSafe(reader.GetIconUrl());
            this.LicenseUrl = GetUriSafe(reader.GetLicenseUrl());
            this.ProjectUrl = GetUriSafe(reader.GetProjectUrl());
            this.RequireLicenseAcceptance = reader.GetRequireLicenseAcceptance();
            this.DevelopmentDependency = reader.GetDevelopmentDependency();
            this.Description = reader.GetDescription();
            this.Summary = reader.GetSummary();
            this.ReleaseNotes = reader.GetReleaseNotes();
            this.Language = reader.GetLanguage();
            this.Tags = reader.GetTags();
            this.Serviceable = reader.IsServiceable();
            this.Copyright = reader.GetCopyright();
            this.Icon = reader.GetIcon();
            this.Readme = reader.GetReadme();
            this.DependencyGroups = reader.GetDependencyGroups();
            this.PackageTypes = reader.GetPackageTypes();
            this.Repository = reader.GetRepositoryMetadata();
            this.LicenseMetadata = reader.GetLicenseMetadata();
            this.FrameworkReferenceGroups = reader.GetFrameworkRefGroups();
        }
        else if (filesystem.GetFileExtension(packagePath) == NuGetConstants.ManifestExtension)
        {
            NuspecReader reader = new(packagePath);

            this.ProjectSourceUrl = GetUriSafe(reader.GetProjectSourceUrl());
            this.PackageSourceUrl = GetUriSafe(reader.GetPackageSourceUrl());
            this.DocsUrl = GetUriSafe(reader.GetDocsUrl());
            this.WikiUrl = GetUriSafe(reader.GetWikiUrl());
            this.MailingListUrl = GetUriSafe(reader.GetMailingListUrl());
            this.BugTrackerUrl = GetUriSafe(reader.GetBugTrackerUrl());
            this.Replaces = reader.GetReplaces().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Provides = reader.GetProvides().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Conflicts = reader.GetConflicts().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.SoftwareDisplayName = reader.GetSoftwareDisplayName();
            this.SoftwareDisplayVersion = reader.GetSoftwareDisplayVersion();
            this.Id = reader.GetId();
            this.Version = reader.GetVersion();
            this.Title = reader.GetTitle();
            this.Authors = reader.GetAuthors().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Owners = reader.GetOwners().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.IconUrl = GetUriSafe(reader.GetIconUrl());
            this.LicenseUrl = GetUriSafe(reader.GetLicenseUrl());
            this.ProjectUrl = GetUriSafe(reader.GetProjectUrl());
            this.RequireLicenseAcceptance = reader.GetRequireLicenseAcceptance();
            this.DevelopmentDependency = reader.GetDevelopmentDependency();
            this.Description = reader.GetDescription();
            this.Summary = reader.GetSummary();
            this.ReleaseNotes = reader.GetReleaseNotes();
            this.Language = reader.GetLanguage();
            this.Tags = reader.GetTags();
            this.Serviceable = reader.IsServiceable();
            this.Copyright = reader.GetCopyright();
            this.Icon = reader.GetIcon();
            this.Readme = reader.GetReadme();
            this.DependencyGroups = reader.GetDependencyGroups();
            this.PackageTypes = reader.GetPackageTypes();
            this.Repository = reader.GetRepositoryMetadata();
            this.LicenseMetadata = reader.GetLicenseMetadata();
            this.FrameworkReferenceGroups = reader.GetFrameworkRefGroups();
        }
        else
        {
            throw new ArgumentException("Package Path not a .nupkg or .nuspec");
        }
    }

    public Uri? ProjectSourceUrl { get; }

    public Uri? PackageSourceUrl { get; }

    public Uri? DocsUrl { get; }

    public Uri? WikiUrl { get; }

    public Uri? MailingListUrl { get; }

    public Uri? BugTrackerUrl { get; }

    public IEnumerable<string> Replaces { get; }

    public IEnumerable<string> Provides { get; }

    public IEnumerable<string> Conflicts { get; }

    public string SoftwareDisplayName { get; }

    public string SoftwareDisplayVersion { get; }

    public string Id { get; }

    public NuGetVersion Version { get; private set; }

    public string Title { get; }

    public IEnumerable<string> Authors { get; }

    public IEnumerable<string> Owners { get; }

    public Uri? IconUrl { get; }

    public Uri? LicenseUrl { get; }

    public Uri? ProjectUrl { get; }

    public bool RequireLicenseAcceptance { get; }

    public bool DevelopmentDependency { get; }

    public string Description { get; }

    public string Summary { get; }

    public string ReleaseNotes { get; }

    public string Language { get; }

    public string Tags { get; }

    public bool Serviceable { get; }

    public string Copyright { get; }

    public string Icon { get; }

    public string Readme { get; }

    public IEnumerable<FrameworkAssemblyReference> FrameworkReferences => Array.Empty<FrameworkAssemblyReference>();

    public IEnumerable<PackageReferenceSet> PackageAssemblyReferences => Array.Empty<PackageReferenceSet>();

    public IEnumerable<PackageDependencyGroup> DependencyGroups { get; }

    public Version? MinClientVersion => null;

    public IEnumerable<ManifestContentFiles> ContentFiles => Array.Empty<ManifestContentFiles>();

    public IEnumerable<PackageType> PackageTypes { get; }

    public RepositoryMetadata Repository { get; }

    public LicenseMetadata LicenseMetadata { get; }

    public IEnumerable<FrameworkReferenceGroup> FrameworkReferenceGroups { get; }

    public void OverrideOriginalVersion(NuGetVersion overrideVersion)
    {
        this.Version = overrideVersion;
    }

    private static Uri? GetUriSafe(string url)
    {
        Uri.TryCreate(url, UriKind.Absolute, out var uri);
        return uri;
    }
}