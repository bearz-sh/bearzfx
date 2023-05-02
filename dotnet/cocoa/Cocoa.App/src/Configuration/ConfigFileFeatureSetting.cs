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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/configuration/ConfigFileFeatureSetting.cs

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Cocoa.Configuration;

/// <summary>
///   XML config file features element.
/// </summary>
[Serializable]
[XmlType("feature")]
public sealed class ConfigFileFeatureSetting : IEquatable<ConfigFileFeatureSetting>
{
    [XmlAttribute(AttributeName = "name")]
    public string? Name { get; set; }

    [XmlAttribute(AttributeName = "enabled")]
    public bool Enabled { get; set; }

    [XmlAttribute(AttributeName = "setExplicitly")]
    public bool SetExplicitly { get; set; }

    [XmlAttribute(AttributeName = "description")]
    public string? Description { get; set; }

    public static bool operator ==(ConfigFileFeatureSetting? left, ConfigFileFeatureSetting? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConfigFileFeatureSetting? left, ConfigFileFeatureSetting? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return obj is ConfigFileFeatureSetting other && this.Equals(other);
    }

    public bool Equals(ConfigFileFeatureSetting? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.Name == other.Name &&
               this.Enabled == other.Enabled &&
               this.SetExplicitly == other.SetExplicitly &&
               this.Description == other.Description;
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return HashCode.Combine(
            this.Name,
            this.Enabled,
            this.SetExplicitly,
            this.Description);
    }
}