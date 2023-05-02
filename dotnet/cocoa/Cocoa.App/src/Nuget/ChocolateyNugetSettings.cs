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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/ChocolateyNuGetSettings.cs

using Cocoa.Logging;

using Microsoft.Extensions.Logging;

using NuGet.Configuration;

namespace Cocoa.Nuget;

public class ChocolateyNuGetSettings : ISettings
{
    private const string ConfigSectionName = "config";

    private readonly ILogger log;

    public ChocolateyNuGetSettings(ILogger<ChocolateyNuGetSettings>? logger = null)
    {
        this.log = logger ?? Log.For<ChocolateyNuGetSettings>();
    }

    public event EventHandler SettingsChanged = (_, _) => { };

    public void AddOrUpdate(string sectionName, SettingItem item)
    {
        if (this.log.IsEnabled(LogLevel.Warning))
            this.log.LogWarning($"NuGet tried to add an item to section {sectionName}");
    }

    public IList<string> GetConfigFilePaths() => Enumerable.Empty<string>().ToList();

    public IList<string> GetConfigRoots() => Enumerable.Empty<string>().ToList();

    public SettingSection? GetSection(string sectionName)
    {
        return null;
    }

    public void Remove(string sectionName, SettingItem item)
    {
        if (this.log.IsEnabled(LogLevel.Warning))
            this.log.LogWarning($"NuGet tried to remove an item from section {sectionName}");
    }

    public void SaveToDisk()
    {
        if (this.log.IsEnabled(LogLevel.Warning))
            this.log.LogWarning($"NuGet tried to save settings to disk");
    }
}