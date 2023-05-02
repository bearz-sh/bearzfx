using NuGet.Packaging;
using NuGet.Versioning;

namespace Cocoa.Domain;

public sealed class ChocolateyPackageInformation
{
    public ChocolateyPackageInformation(IPackageMetadata package)
    {
        this.Package = package;
    }

    public IPackageMetadata Package { get; }

    public Registry.Registry? RegistrySnapshot { get; set; }

    public PackageFiles? FilesSnapshot { get; set; }

    public string? Arguments { get; set; }

    public NuGetVersion? VersionOverride { get; set; }

    public bool HasSilentUninstall { get; set; }

    public bool IsPinned { get; set; }

    public string? ExtraInformation { get; set; }
}