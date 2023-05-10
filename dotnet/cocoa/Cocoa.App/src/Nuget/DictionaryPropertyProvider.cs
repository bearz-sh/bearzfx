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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/DictionaryPropertyProvider.cs

namespace Cocoa.Nuget;

public sealed class DictionaryPropertyProvider
{
    private readonly IDictionary<string, string> properties;

    public DictionaryPropertyProvider(IDictionary<string, string> properties)
    {
        this.properties = properties;
    }

    public string? GetPropertyValue(string propertyName)
    {
        if (this.properties.TryGetValue(propertyName, out var value))
        {
            return value;
        }

        return default;
    }
}