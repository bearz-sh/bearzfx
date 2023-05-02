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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/ChocolateySourceCacheContext.cs

using System.Diagnostics;

using Bearz.Extra.Strings;

using Cocoa.Configuration;

using NuGet.Protocol.Core.Types;

namespace Cocoa.Nuget;

public sealed class ChocolateySourceCacheContext : SourceCacheContext
{
    private readonly string chocolateyCacheLocation;

    /// <summary>
    /// Path of temp folder if requested by GeneratedTempFolder.
    /// </summary>
    private string? generatedChocolateyTempFolder = null;

    public ChocolateySourceCacheContext(ChocolateyConfiguration config)
    {
        if (config.CacheLocation.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(config), "CacheLocation cannot be null or whitespace.");

        this.chocolateyCacheLocation = config.CacheLocation;
    }

    public override string GeneratedTempFolder
    {
        get
        {
            if (this.generatedChocolateyTempFolder == null)
            {
                var newTempFolder = Path.Combine(
                    this.chocolateyCacheLocation,
                    "SourceTempCache",
                    Guid.NewGuid().ToString());

                Interlocked.CompareExchange(ref this.generatedChocolateyTempFolder, newTempFolder, comparand: null);
            }

            return this.generatedChocolateyTempFolder;
        }

        set => Interlocked.CompareExchange(ref this.generatedChocolateyTempFolder, value, comparand: null);
    }

    /// <summary>
    /// Clones the current SourceCacheContext.
    /// </summary>
    /// <returns>A clone.</returns>
    public override SourceCacheContext Clone()
    {
        return new SourceCacheContext()
        {
            DirectDownload = this.DirectDownload,
            IgnoreFailedSources = this.IgnoreFailedSources,
            MaxAge = this.MaxAge,
            NoCache = this.NoCache,
            GeneratedTempFolder = this.generatedChocolateyTempFolder,
            RefreshMemoryCache = this.RefreshMemoryCache,
            SessionId = this.SessionId,
        };
    }

    protected override void Dispose(bool disposing)
    {
        var currentTempFolder = Interlocked.CompareExchange(ref this.generatedChocolateyTempFolder, value: null, comparand: null);

        if (currentTempFolder != null)
        {
            try
            {
                Directory.Delete(currentTempFolder, recursive: true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        base.Dispose(disposing);
    }
}