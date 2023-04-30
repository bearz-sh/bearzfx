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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/attributes/MultiServiceAttribute.cs

namespace Cocoa.Attributes;

[AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
internal sealed class MultiServiceAttribute : Attribute
{
    // This is a positional argument
    public MultiServiceAttribute()
        : this(isMultiService: true)
    {
    }

    public MultiServiceAttribute(bool isMultiService)
    {
        this.IsMultiService = isMultiService;
    }

    public bool IsMultiService { get; }
}