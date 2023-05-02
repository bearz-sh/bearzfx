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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/RegistryApplicationKey.cs

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using Bearz.Extra.Strings;

using Cocoa.Installers;
using Cocoa.Xml;

namespace Cocoa.Registry;

[Serializable]
[XmlType("key")]
public sealed class RegistryApplicationKey : IEquatable<RegistryApplicationKey>
{
    public string? KeyPath { get; set; }

    [XmlAttribute(AttributeName = "installerType")]
    public InstallerType InstallerType { get; set; }

    public string? DefaultValue { get; set; }

    [XmlAttribute(AttributeName = "displayName")]
    public string? DisplayName { get; set; } = string.Empty;

    public XmlCData? InstallLocation { get; set; }

    public XmlCData? UninstallString { get; set; }

    public bool HasQuietUninstall { get; set; }

    // informational
    public XmlCData? Publisher { get; set; }

    public string? InstallDate { get; set; }

    public XmlCData? InstallSource { get; set; }

    // uint
    public string? Language { get; set; }

    // version stuff
    [XmlAttribute(AttributeName = "displayVersion")]
    public string? DisplayVersion { get; set; } = string.Empty;

    // uint
    public string? Version { get; set; }

    // uint
    public string? VersionMajor { get; set; }

    // uint
    public string? VersionMinor { get; set; }

    // uint
    public string? VersionRevision { get; set; }

    // uint
    public string? VersionBuild { get; set; }

    // install information
    public bool SystemComponent { get; set; }

    public bool WindowsInstaller { get; set; }

    public bool NoRemove { get; set; }

    public bool NoModify { get; set; }

    public bool NoRepair { get; set; }

    // hotfix, securityupdate, update rollup, servicepack.
    public string? ReleaseType { get; set; }

    public string? ParentKeyName { get; set; }

    public XmlCData? LocalPackage { get; set; }

    /// <summary>
    ///  Determines if the key is listed as a program in Programs and Features.
    /// </summary>
    /// <returns>true if the key should be listed as a program.</returns>
    /// <remarks>
    ///   http://community.spiceworks.com/how_to/show/2238-how-add-remove-programs-works.
    /// </remarks>
    public bool IsInProgramsAndFeatures()
    {
        return !this.DisplayName.IsNullOrWhiteSpace()
               && this.UninstallString?.ToString().IsNullOrWhiteSpace() == false
               && this.InstallerType != InstallerType.HotfixOrSecurityUpdate
               && this.InstallerType != InstallerType.ServicePack
               && this.ParentKeyName.IsNullOrWhiteSpace()
               && !this.NoRemove
               && !this.SystemComponent;
    }

    public override string ToString()
    {
        return $"{this.DisplayName}|{this.DisplayVersion}|{this.InstallerType}|{this.UninstallString}|{this.KeyPath}";
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return HashCode.Combine(
            this.DisplayName.ToSafeString(),
            this.DisplayVersion.ToSafeString(),
            this.UninstallString.ToSafeString(),
            this.KeyPath.ToSafeString());
    }

    public bool Equals(RegistryApplicationKey? other)
    {
        if (ReferenceEquals(other, null))
            return false;

        if (ReferenceEquals(other, this))
            return true;

        if (!this.DisplayName.EqualsInvariant(other.DisplayName))
            return false;

        if (!this.DisplayVersion.EqualsInvariant(other.DisplayVersion))
            return false;

        if (!this.UninstallString.ToSafeString().EqualsInvariant(other.UninstallString.ToSafeString()))
            return false;

        return this.KeyPath.EqualsInvariant(other.KeyPath);
    }

    public override bool Equals(object? obj)
    {
        return obj is RegistryApplicationKey key && this.Equals(key);
    }
}