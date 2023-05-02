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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/configuration/ConfigFileSettings.cs

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using Bearz.Extra.Collections;

namespace Cocoa.Configuration;

/// <summary>
///  XML configuration file.
/// </summary>
[Serializable]
[XmlRoot("chocolatey")]
public sealed class ConfigFileSettings : IEquatable<ConfigFileSettings>
{
    [XmlArray("config")]
    public HashSet<ConfigFileConfigSetting> ConfigSettings { get; set; } = new();

    [XmlArray("sources")]
    public HashSet<ConfigFileSourceSetting> Sources { get; set; } = new();

    [XmlArray("features")]
    public HashSet<ConfigFileFeatureSetting> Features { get; set; } = new();

    [XmlArray("apiKeys")]
    public HashSet<ConfigFileApiKeySetting> ApiKeys { get; set; } = new();

    public static bool operator ==(ConfigFileSettings? left, ConfigFileSettings? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ConfigFileSettings? left, ConfigFileSettings? right)
    {
        return !Equals(left, right);
    }

    public override bool Equals(object? obj)
    {
        return obj is ConfigFileSettings other && this.Equals(other);
    }

    public bool Equals(ConfigFileSettings? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.ConfigSettings.EqualTo(other.ConfigSettings)
               && this.Sources.EqualTo(other.Sources)
               && this.Features.EqualTo(other.Features)
               && this.ApiKeys.EqualTo(other.ApiKeys);
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        var hash = default(HashCode);

        if (this.ConfigSettings != null)
        {
            foreach (var item in this.ConfigSettings)
            {
                hash.Add(item);
            }
        }

        if (this.Sources != null)
        {
            foreach (var item in this.Sources)
            {
                hash.Add(item);
            }
        }

        if (this.Features != null)
        {
            foreach (var item in this.Features)
            {
                hash.Add(item);
            }
        }

        if (this.ApiKeys != null)
        {
            foreach (var item in this.ApiKeys)
            {
                hash.Add(item);
            }
        }

        return hash.ToHashCode();
    }
}