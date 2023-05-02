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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/configuration/ConfigFileConfigSetting.cs

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Cocoa.Configuration;

/// <summary>
///   XML config file config element.
/// </summary>
[Serializable]
[XmlType("add")]
public sealed class ConfigFileConfigSetting : IEquatable<ConfigFileConfigSetting>
{
    [XmlAttribute(AttributeName = "key")]
    public string? Key { get; set; }

    [XmlAttribute(AttributeName = "value")]
    public string? Value { get; set; }

    [XmlAttribute(AttributeName = "description")]
    public string? Description { get; set; }

    public static bool operator ==(ConfigFileConfigSetting? left, ConfigFileConfigSetting? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConfigFileConfigSetting? left, ConfigFileConfigSetting? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return obj is ConfigFileConfigSetting other && this.Equals(other);
    }

    public bool Equals(ConfigFileConfigSetting? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Key == other.Key &&
               this.Value == other.Value &&
               this.Description == other.Description;
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Key, this.Value, this.Description);
    }
}