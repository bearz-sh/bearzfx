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

using System.Runtime.Serialization.Formatters.Binary;

namespace Cocoa;

public static class ObjectExtensions
{
    /// <summary>
    /// A non null safe ToString() extension method.
    /// </summary>
    /// <param name="input">The input object.</param>
    /// <returns>The string value or string.Empty.</returns>
    public static string ToSafeString(this object? input)
        => input?.ToString() ?? string.Empty;

    /// <summary>
    /// Performs a deep copy of an object using the BinaryFormatter.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>A deep copy of an object if its not null.</returns>
    public static T DeepCopy<T>(this T value)
    {
        // todo: https://github.com/Burtsev-Alexey/net-object-deep-copy/blob/master/ObjectExtensions.cs
        if (value is null)
            return value;

        using var ms = new MemoryStream();
        var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // TODO: figure out a suitable replacement
        formatter.Serialize(ms, value);
        ms.Position = 0;
#pragma warning disable S5773
        return (T)formatter.Deserialize(ms);
#pragma warning restore S5773
    }
}