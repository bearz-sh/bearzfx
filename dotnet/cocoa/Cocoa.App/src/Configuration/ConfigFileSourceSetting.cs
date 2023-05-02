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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/configuration/ConfigFileSourceSetting.cs

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Cocoa.Configuration;

/// <summary>
///   XML config file sources element.
/// </summary>
[Serializable]
[XmlType("source")]
public sealed class ConfigFileSourceSetting : IEquatable<ConfigFileSourceSetting>
{
    [XmlAttribute(AttributeName = "id")]
    public string? Id { get; set; }

    [XmlAttribute(AttributeName = "value")]
    public string? Value { get; set; }

    [XmlAttribute(AttributeName = "disabled")]
    public bool Disabled { get; set; }

    [XmlAttribute(AttributeName = "bypassProxy")]
    public bool BypassProxy { get; set; }

    [XmlAttribute(AttributeName = "selfService")]
    public bool AllowSelfService { get; set; }

    [XmlAttribute(AttributeName = "adminOnly")]
    public bool VisibleToAdminsOnly { get; set; }

    [XmlAttribute(AttributeName = "user")]
    public string? UserName { get; set; }

    [XmlAttribute(AttributeName = "password")]
    public string? Password { get; set; }

    [XmlAttribute(AttributeName = "priority")]
    public int Priority { get; set; }

    [XmlAttribute(AttributeName = "certificate")]
    public string? Certificate { get; set; }

    [XmlAttribute(AttributeName = "certificatePassword")]
    public string? CertificatePassword { get; set; }

    public static bool operator ==(ConfigFileSourceSetting? left, ConfigFileSourceSetting? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConfigFileSourceSetting? left, ConfigFileSourceSetting? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return obj is ConfigFileSourceSetting other && this.Equals(other);
    }

    public bool Equals(ConfigFileSourceSetting? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Id == other.Id &&
               this.Value == other.Value &&
               this.Disabled == other.Disabled &&
               this.BypassProxy == other.BypassProxy &&
               this.AllowSelfService == other.AllowSelfService &&
               this.VisibleToAdminsOnly == other.VisibleToAdminsOnly &&
               this.UserName == other.UserName &&
               this.Password == other.Password &&
               this.Priority == other.Priority &&
               this.Certificate == other.Certificate &&
               this.CertificatePassword == other.CertificatePassword;
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        var hashCode = default(HashCode);
        hashCode.Add(this.Id);
        hashCode.Add(this.Value);
        hashCode.Add(this.Disabled);
        hashCode.Add(this.BypassProxy);
        hashCode.Add(this.AllowSelfService);
        hashCode.Add(this.VisibleToAdminsOnly);
        hashCode.Add(this.UserName);
        hashCode.Add(this.Password);
        hashCode.Add(this.Priority);
        hashCode.Add(this.Certificate);
        hashCode.Add(this.CertificatePassword);
        return hashCode.ToHashCode();
    }
}