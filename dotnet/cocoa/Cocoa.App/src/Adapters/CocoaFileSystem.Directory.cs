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

using System.Runtime.InteropServices;

using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Logging;

namespace Cocoa.Adapters;

public sealed partial class CocoaFileSystem
{
    public string GetCurrentDirectory()
    {
        return Directory.GetCurrentDirectory();
    }

    public IEnumerable<string> GetDirectories(string directoryPath)
    {
        if (!this.DirectoryExists(directoryPath))
            return Array.Empty<string>();

        return Directory.EnumerateDirectories(directoryPath);
    }

    public IEnumerable<string> GetDirectories(
        string directoryPath,
        string pattern,
        SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (!this.DirectoryExists(directoryPath))
            return Array.Empty<string>();

        return Directory.EnumerateDirectories(directoryPath, pattern, option);
    }

    public bool DirectoryExists(string directoryPath)
    {
        return Directory.Exists(directoryPath);
    }

    public string GetDirectoryName(string filePath)
    {
#if NETFRAMEWORK
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                return Path.GetDirectoryName(filePath) ?? string.Empty;
            }
            catch (IOException)
            {
                return Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(filePath);
            }
        }

        filePath = this.NormalizePath(filePath);
        return Path.GetDirectoryName(filePath) ?? string.Empty;
#else
        filePath = this.NormalizePath(filePath);
        return Path.GetDirectoryName(filePath) ?? string.Empty;
#endif
    }

    public IDirectoryInfo GetDirectoryInfo(string directoryPath)
    {
#if !NETFRAMEWORK
        return new SystemDirectoryInfo(directoryPath);
#else
        try
        {
            if (directoryPath.Length >= MaxPathDirectory)
            {
                return new AlphaFsDirectoryInfo(directoryPath);
            }

            return new SystemDirectoryInfo(directoryPath);
        }
        catch (IOException)
        {
            return new AlphaFsDirectoryInfo(directoryPath);
        }
#endif
    }

    public IDirectoryInfo GetFileDirectoryInfo(string filePath)
    {
        if (filePath.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(filePath));

        var dir = FsPath.GetDirectoryName(filePath);
        if (dir.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(filePath));

        return this.GetDirectoryInfo(dir);
    }

    public void CreateDirectory(string directoryPath)
    {
        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to create directory \"{directoryPath}\".");
        this.AllowRetries(
            () =>
            {
#if NETFRAMEWORK
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (IOException)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        Alphaleonis.Win32.Filesystem.Directory.CreateDirectory(directoryPath);
                        return;
                    }

                    throw;
                }
#else
                Directory.CreateDirectory(directoryPath);
#endif
            });
    }

    public void MoveDirectory(string directoryPath, string newDirectoryPath)
    {
        this.MoveDirectory(directoryPath, newDirectoryPath, false);
    }

    public void MoveDirectory(string directoryPath, string newDirectoryPath, bool useFileMoveFallback)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath));

        if (string.IsNullOrWhiteSpace(newDirectoryPath))
            throw new ArgumentNullException(newDirectoryPath);

        // Linux / macOS do not have a SystemDrive environment variable, instead, everything is under "/"
        this.ThrowIfSystemDrive(directoryPath);

        try
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug($"Moving '{directoryPath}'{Environment.NewLine} to '{newDirectoryPath}'");

            this.AllowRetries(
                () =>
                {
#if NETFRAMEWORK
                    try
                    {
                        Directory.Move(directoryPath, newDirectoryPath);
                    }
                    catch (IOException)
                    {
                        Alphaleonis.Win32.Filesystem.Directory.Move(directoryPath, newDirectoryPath);
                    }
#else
                    Directory.Move(directoryPath, newDirectoryPath);
#endif
                });
        }
        catch (Exception ex)
        {
            // If we don't want to use the fallback, we will just rethrow the exception.
            if (!useFileMoveFallback)
            {
                throw;
            }

            if (this.log.IsEnabled(LogLevel.Warning))
                this.log.LogWarning(ex, $"Move failed with message:{directoryPath} {Environment.NewLine}{ex.Message} Attempting backup move method.");

            this.EnsureDirectoryExists(newDirectoryPath, ignoreError: true);
            foreach (var file in this.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories))
            {
                var destinationFile = file.Replace(directoryPath, newDirectoryPath);
                if (this.FileExists(destinationFile))
                    this.DeleteFile(destinationFile);

                var dir = FsPath.GetDirectoryName(destinationFile);
                if (dir.IsNullOrEmpty())
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Diredtory name is null or empty for '{destinationFile}'");

                    continue;
                }

                this.EnsureDirectoryExists(dir, ignoreError: true);
                if (this.log.IsEnabled(LogLevel.Debug))
                    this.log.LogDebug($"Moving '{directoryPath}'{Environment.NewLine} to '{newDirectoryPath}'");

                this.MoveFile(file, destinationFile);
            }

            Thread.Sleep(1000); // let the moving files finish up
            this.DeleteDirectoryChecked(directoryPath, recursive: true, overrideAttributes: false);
        }

        Thread.Sleep(2000); // sleep for enough time to allow the folder to be cleared
    }

    public void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, bool overwriteExisting)
    {
        this.EnsureDirectoryExists(destinationDirectoryPath, ignoreError: true);

        var exceptions = new List<Exception>();

        foreach (var file in this.GetFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories))
        {
            var destinationFile = file.Replace(sourceDirectoryPath, destinationDirectoryPath);
            this.EnsureDirectoryExists(this.GetDirectoryName(destinationFile), ignoreError: true);

            // this.Log().Debug(ChocolateyLoggers.Verbose, "Copying '{0}' {1} to '{2}'".format_with(file, Environment.NewLine, destinationFile));
            try
            {
                this.CopyFile(file, destinationFile, overwriteExisting);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        // TODO: determine if there is a better way to handle waiting for files to copy
        Thread.Sleep(1500); // sleep for enough time to allow the folder to finish copying

        if (exceptions.Count > 1)
        {
            throw new AggregateException(
                $"An exception occurred while copying files to '{destinationDirectoryPath}'", exceptions);
        }
        else if (exceptions.Count == 1)
        {
            throw exceptions[0];
        }
    }

    public void EnsureDirectoryExists(string directoryPath)
    {
        this.EnsureDirectoryExists(directoryPath, false);
    }

    public void DeleteDirectory(string directoryPath, bool recursive)
        => this.DeleteDirectory(directoryPath, recursive, overrideAttributes: false);

    public void DeleteDirectory(string directoryPath, bool recursive, bool overrideAttributes)
    {
        if (directoryPath.IsNullOrWhiteSpace())
            throw new ArgumentNullException(nameof(directoryPath), "A directory path must be specified");

        // Linux / macOS do not have a SystemDrive environment variable, instead, everything is under "/"
        var systemDrive = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.GetEnvironmentVariable("SystemDrive")
            : "/";

        directoryPath = this.NormalizePath(directoryPath);
        if (directoryPath.EqualsInvariant(systemDrive))
            throw new ArgumentException("Cannot move or delete the root of the system drive", nameof(directoryPath));

        if (overrideAttributes)
        {
            foreach (var file in this.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories))
            {
                var filePath = this.GetFullPath(file);
                var fileInfo = this.GetFileInfoFor(filePath);

                if (this.IsSystemFile(fileInfo))
                    this.EnsureFileAttributeRemoved(filePath, FileAttributes.System);
                if (this.IsReadOnlyFile(fileInfo))
                    this.EnsureFileAttributeRemoved(filePath, FileAttributes.ReadOnly);

                if (this.IsHiddenFile(fileInfo))
                    this.EnsureFileAttributeRemoved(filePath, FileAttributes.Hidden);
            }
        }

        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to delete directory \"{directoryPath}\".");

        this.AllowRetries(
            () =>
            {
#if !NETFRAMEWORK
                Directory.Delete(directoryPath, recursive);
#else
                try
                {
                    Directory.Delete(directoryPath, recursive);
                }
                catch (IOException)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        Alphaleonis.Win32.Filesystem.Directory.Delete(directoryPath, recursive);

                    throw;
                }
#endif
            });
    }

    public void DeleteDirectoryChecked(string directoryPath, bool recursive)
    {
        this.DeleteDirectoryChecked(directoryPath, recursive, overrideAttributes: false);
    }

    public void DeleteDirectoryChecked(string directoryPath, bool recursive, bool overrideAttributes)
    {
        if (this.DirectoryExists(directoryPath))
        {
            this.DeleteDirectory(directoryPath, recursive, overrideAttributes);
        }
    }

    public bool IsSystemDrive(string directoryPath)
    {
        // Linux / macOS do not have a SystemDrive environment variable, instead, everything is under "/"
        var systemDrive = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.GetEnvironmentVariable("SystemDrive")
            : "/";

        return directoryPath.EqualsInvariant(systemDrive);
    }

    public void ThrowIfSystemDrive(string directoryPath)
    {
        if (this.IsSystemDrive(directoryPath))
            throw new ArgumentException("Cannot move or delete the root of the system drive", nameof(directoryPath));
    }

    private void EnsureDirectoryExists(string directoryPath, bool ignoreError)
    {
        if (!this.DirectoryExists(directoryPath))
        {
            try
            {
                this.CreateDirectory(directoryPath);
            }
            catch (SystemException e)
            {
                if (!ignoreError)
                {
                    // TODO: determine if the error should be logged twice.
                    this.log.LogError(
                        e,
                        $"Cannot create directory \"{this.GetFullPath(directoryPath)}\". Error was:{Environment.NewLine}{e.Message}");
                    throw;
                }
            }
        }
    }
}