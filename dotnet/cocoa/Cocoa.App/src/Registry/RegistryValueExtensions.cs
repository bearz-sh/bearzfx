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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure.app/domain/RegistryValueExtensions.cs

using System.Security;

using Microsoft.Win32;

namespace Cocoa.Registry;

public static class RegistryValueExtensions
{
    public static string GetValueAsString(this RegistryKey? key, string name)
    {
        if (key == null)
            return string.Empty;

        // Since it is possible that registry keys contain characters that are not valid
        // in XML files, ensure that all content is escaped, prior to serialization
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.securityelement.escape?view=netframework-4.0
        return ObjectExtensions.ToSafeString(SecurityElement.Escape(key.GetValue(name).ToSafeString()))
            .Replace("&quot;", "\"")
            .Replace("&apos;", "'")
            .Replace("\0", string.Empty);
    }
}