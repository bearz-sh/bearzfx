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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure/filesystem/FileSystem.cs

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text;

namespace Cocoa.Adapters;

public sealed partial class CocoaFileSystem
{
    public string CombinePaths(string? leftItem, params string[] rightItems)
    {
        if (leftItem == null)
        {
            var stackFrame = new System.Diagnostics.StackFrame(1);
            var methodName = stackFrame.GetMethod()?.Name ?? string.Empty;
            var methodMessage = methodName.IsNullOrWhiteSpace() ? string.Empty : $" Method called from '{methodName}'";
            throw new InvalidOperationException(
                $"Path to combine cannot be null. Tried to combine null with '{string.Join(",", rightItems)}'.{methodMessage}");
        }

        var combinedPath = this.NormalizePath(leftItem);
        foreach (var rightItem in rightItems)
        {
            if (rightItem.IsNullOrWhiteSpace())
            {
                continue;
            }

            if (rightItem.Contains(":"))
            {
                throw new ArgumentException(
                    nameof(rightItems),
                    $"Cannot combine a path with ':' attempted to combine '{rightItem}' with '{combinedPath}'");
            }

            var rightSide = this.NormalizePath(rightItem);
            if (rightSide[0] is '/' or '\\')
            {
                combinedPath = Path.Combine(combinedPath, rightSide.Substring(1));
            }
            else
            {
                combinedPath = Path.Combine(combinedPath, rightSide);
            }
        }

        return combinedPath;
    }

    public string GetFullPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return path;
#if NETFRAMEWORK
        try
        {
            return Path.GetFullPath(path);
        }
        catch (IOException)
        {
            return Alphaleonis.Win32.Filesystem.Path.GetFullPath(path);
        }
#else
        return Path.GetFullPath(path);
#endif
    }

    public string GetTempPath()
    {
        var path = Path.GetTempPath();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return path;

        if (System.Environment.UserName.Contains(EnvironmentKeys.SystemUserName, StringComparison.InvariantCultureIgnoreCase)
            || path.Contains("config\\systemprofile", StringComparison.InvariantCultureIgnoreCase))
        {
            path = System.Environment.ExpandEnvironmentVariables(System.Environment.GetEnvironmentVariable(EnvironmentKeys.Temp, EnvironmentVariableTarget.Machine).ToSafeString());
        }

        return path;
    }

    public char GetPathDirectorySeparatorChar()
    {
        return Path.DirectorySeparatorChar;
    }

    public char GetPathSeparator()
    {
        return Path.PathSeparator;
    }

    public string GetExecutablePath(string executableName)
    {
        if (executableName.IsNullOrWhiteSpace())
            return string.Empty;

        var ext = FsPath.GetDirectoryName(executableName);
        if (ext == ".exe")
            executableName = FsPath.GetFileNameWithoutExtension(executableName);

        // Gets the path to an executable based on looking in current
        // working directory, next to the running process, then among the
        // derivatives of Path and Pathext variables, applied in order.
        var searchPaths = new List<string>()
        {
            Environment.CurrentDirectory,
            this.GetDirectoryName(this.GetCurrentAssemblyPath()),
        };

        var path = Env.Process.Which(executableName, searchPaths, false);

        // If not found, return the same as passed in - it may work,
        // but possibly not.
        return path ?? string.Empty;
    }

    public string NormalizePath([NotNullIfNotNull("path")] string? path)
    {
        if (path.IsNullOrWhiteSpace())
            return string.Empty;

        var l = path.Length;
        var lastChar = path[l - 1];
        if (lastChar is '/' or '\\')
        {
            while (lastChar is '/' or '\\')
            {
                l--;
                lastChar = path[l - 1];
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (path.IndexOf('/') > -1)
            {
                var sb = StringBuilderCache.Acquire();
                for (var i = 0; i < l; i++)
                {
                    var c = path[i];
                    sb.Append(c is '/' ? '\\' : c);
                }

                return StringBuilderCache.GetStringAndRelease(sb);
            }
            else if (l != path.Length)
            {
                return path.Substring(0, l);
            }

            return path;
        }

        if (path.IndexOf('\\') > -1)
        {
            var sb = StringBuilderCache.Acquire();
            for (var i = 0; i < l; i++)
            {
                var c = path[i];
                sb.Append(c is '\\' ? '/' : c);
            }

            return StringBuilderCache.GetStringAndRelease(sb);
        }

        if (l != path.Length)
        {
            return path.Substring(0, l);
        }

        return path;
    }

    public string GetCurrentAssemblyPath()
    {
        return Assembly.GetExecutingAssembly().Location;
    }
}