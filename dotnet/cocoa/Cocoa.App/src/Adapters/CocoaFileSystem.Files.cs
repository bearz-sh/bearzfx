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

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Cocoa.Adapters;

public sealed partial class CocoaFileSystem
{
    public IEnumerable<string> GetFiles(string directoryPath, string pattern = "*.*", SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            return Array.Empty<string>();

        if (!this.DirectoryExists(directoryPath))
        {
            if (this.log.IsEnabled(LogLevel.Warning))
                this.log.LogWarning($"Directory '{directoryPath}' does not exist.");

            return Array.Empty<string>();
        }

        return Directory.EnumerateFiles(directoryPath, pattern, option);
    }

    public IEnumerable<string> GetFiles(string directoryPath, string[] extensions, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (string.IsNullOrWhiteSpace(directoryPath)) return new List<string>();

        return Directory.EnumerateFiles(directoryPath, "*.*", option)
                        .Where(f => extensions.Any(x => f.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
    }

    public bool FileExists(string filePath)
    {
#if NETFRAMEWORK
        try
        {
            return File.Exists(filePath);
        }
        catch (IOException)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Alphaleonis.Win32.Filesystem.File.Exists(filePath);

            return false;
        }
#else
        return File.Exists(filePath);
#endif
    }

    public string GetFileName(string filePath)
    {
        return Path.GetFileName(filePath);
    }

    public string GetFilenameWithoutExtension(string filePath)
    {
        filePath = this.NormalizePath(filePath);
        return Path.GetFileNameWithoutExtension(filePath);
    }

    public string GetFileExtension(string filePath)
    {
        filePath = this.NormalizePath(filePath);
        return Path.GetExtension(filePath);
    }

    public IFileInfo GetFileInfoFor(string filePath)
    {
#if !NETFRAMEWORK
        return new SystemFileInfo(filePath);
#else
        try
        {
            if (!string.IsNullOrWhiteSpace(filePath) && filePath.Length >= MaxPathFile)
            {
                return new AlphaFsFileInfo(filePath);
            }

            return new SystemFileInfo(filePath);
        }
        catch (IOException)
        {
            return new AlphaFsFileInfo(filePath);
        }
#endif
    }

    public DateTime GetFileModifiedDate(string filePath)
    {
        return new FileInfo(filePath).LastWriteTime;
    }

    public long GetFileSize(string filePath)
    {
        return new FileInfo(filePath).Length;
    }

    public string GetFileVersionFor(string filePath)
    {
        return FileVersionInfo.GetVersionInfo(this.GetFullPath(filePath))?.FileVersion ?? string.Empty;
    }

    public bool IsSystemFile(IFileInfo file)
    {
        bool isSystemFile = (file.Attributes & FileAttributes.System) == FileAttributes.System;
        if (!isSystemFile)
        {
            if (file.DirectoryName is not null)
            {
                var directoryInfo = this.GetDirectoryInfo(file.DirectoryName);
                isSystemFile = (directoryInfo.Attributes & FileAttributes.System) == FileAttributes.System;
            }
        }
        else
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug($"File \"{file.FullName}\" is a readonly file.");
        }

        return isSystemFile;
    }

    public bool IsReadOnlyFile(IFileInfo file)
    {
        bool isReadOnlyFile = (file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        if (!isReadOnlyFile)
        {
            if (file.Directory is not null)
            {
                var directoryInfo = file.Directory;
                isReadOnlyFile = (directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
        }
        else
        {
            string fullName = file.FullName;
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug($"File \"{fullName}\" is a readonly file.");
        }

        return isReadOnlyFile;
    }

    public bool IsHiddenFile(IFileInfo file)
    {
        bool isHiddenFile = (file.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
        if (!isHiddenFile)
        {
            if (file.Directory is not null)
            {
                var directoryInfo = file.Directory;
                isHiddenFile = (directoryInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            }
        }
        else
        {
            string fullName = file.FullName;
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug($"File \"{fullName}\" is a hidden file.");
        }

        return isHiddenFile;
    }

    public bool IsEncryptedFile(IFileInfo file)
    {
        bool isEncrypted = (file.Attributes & FileAttributes.Encrypted) == FileAttributes.Encrypted;
        string fullName = file.FullName;
        if (isEncrypted && this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"File \"{fullName}\" an encrypted file?");

        return isEncrypted;
    }

    public string GetFileDate(IFileInfo file)
    {
        return file.CreationTime < file.LastWriteTime
                   ? file.CreationTime.Date.ToString("yyyyMMdd")
                   : file.LastWriteTime.Date.ToString("yyyyMMdd");
    }

    public void MoveFile(string filePath, string newFilePath)
    {
        this.EnsureDirectoryExists(this.GetDirectoryName(newFilePath), ignoreError: true);

        this.AllowRetries(
            () =>
            {
#if NETFRAMEWORK
                try
                {
                    File.Move(filePath, newFilePath);
                }
                catch (IOException)
                {
                    Alphaleonis.Win32.Filesystem.File.Move(filePath, newFilePath);
                }
#else
                File.Move(filePath, newFilePath);
#endif
            });
    }

    public void CopyFile(string sourceFilePath, string destinationFilePath, bool overwriteExisting)
    {
        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to copy \"{sourceFilePath}\" to \"{destinationFilePath}\".");

        this.EnsureDirectoryExists(this.GetDirectoryName(destinationFilePath), ignoreError: true);

        this.AllowRetries(
            () =>
            {
#if NETFRAMEWORK
                try
                {
                    File.Copy(sourceFilePath, destinationFilePath, overwriteExisting);
                }
                catch (IOException)
                {
                    Alphaleonis.Win32.Filesystem.File.Copy(sourceFilePath, destinationFilePath, overwriteExisting);
                }
#else
                File.Copy(sourceFilePath, destinationFilePath, overwriteExisting);
#endif
            });
    }

    public bool CopyFileUnsafe(string sourceFilePath, string destinationFilePath, bool overwriteExisting)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            this.CopyFile(sourceFilePath, destinationFilePath, overwriteExisting);
            return true;
        }

        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to copy from \"{sourceFilePath}\" to \"{destinationFilePath}\".");

        this.EnsureDirectoryExists(this.GetDirectoryName(destinationFilePath), ignoreError: true);

        // Private Declare Function apiCopyFile Lib "kernel32" Alias "CopyFileA" _
        int success = CopyFileW(sourceFilePath, destinationFilePath, overwriteExisting ? 0 : 1);
        return success != 0;
    }

    public void ReplaceFile(string sourceFilePath, string destinationFilePath, string backupFilePath)
    {
        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to replace \"{destinationFilePath}\" with \"{sourceFilePath}\".");

        var traceEnabled = this.log.IsEnabled(LogLevel.Trace);
        var debugEnabled = this.log.IsEnabled(LogLevel.Debug);

        this.AllowRetries(
            () =>
            {
#if NETFRAMEWORK
                try
                {
                    // File.Replace is very sensitive to issues with file access
                    // the docs mention that using a backup fixes this, but this
                    // has not been the case with choco - the backup file has been
                    // the most sensitive to issues with file locking
                    // so we'll do our own backup and replace
                    // move existing file to backup location
                    if (!string.IsNullOrEmpty(backupFilePath) && this.FileExists(destinationFilePath))
                    {
                        try
                        {
                            if (traceEnabled)
                                this.log.LogTrace($"Backing up \"{destinationFilePath}\" to \"{backupFilePath}\".");

                            if (this.FileExists(backupFilePath))
                            {
                                if (traceEnabled)
                                    this.log.LogTrace($"Deleting existing backup file at '{backupFilePath}'.");

                                this.DeleteFile(backupFilePath);
                            }

                            if (traceEnabled)
                                this.log.LogTrace($"Moving \"{destinationFilePath}\" to \"{backupFilePath}\".");

                            this.MoveFile(destinationFilePath, backupFilePath);
                        }
                        catch (Exception ex)
                        {
                            if (debugEnabled)
                                this.log.LogDebug(ex, $"Error capturing backup of '{destinationFilePath}'");
                        }
                    }

                    // copy source file to destination
                    if (traceEnabled)
                        this.log.LogTrace($"Copying \"{sourceFilePath}\" to \"{destinationFilePath}\".");

                    this.CopyFile(sourceFilePath, destinationFilePath, overwriteExisting: true);

                    // delete source file
                    try
                    {
                        if (traceEnabled)
                            this.log.LogTrace($"Deleting \"{sourceFilePath}\".");

                        this.DeleteFile(sourceFilePath);
                    }
                    catch (Exception ex)
                    {
                        if (debugEnabled)
                            this.log.LogDebug(ex, $"    Error deleting '{sourceFilePath}'");
                    }
                }
                catch (IOException)
                {
                    Alphaleonis.Win32.Filesystem.File.Replace(sourceFilePath, destinationFilePath, backupFilePath);
                }
#else
                // File.Replace is very sensitive to issues with file access
                    // the docs mention that using a backup fixes this, but this
                    // has not been the case with choco - the backup file has been
                    // the most sensitive to issues with file locking
                    // so we'll do our own backup and replace
                    // move existing file to backup location
                    if (!string.IsNullOrEmpty(backupFilePath) && this.FileExists(destinationFilePath))
                    {
                        try
                        {
                            if (traceEnabled)
                                this.log.LogTrace($"Backing up \"{destinationFilePath}\" to \"{backupFilePath}\".");

                            if (this.FileExists(backupFilePath))
                            {
                                if (traceEnabled)
                                    this.log.LogTrace($"Deleting existing backup file at '{backupFilePath}'.");

                                this.DeleteFile(backupFilePath);
                            }

                            if (traceEnabled)
                                this.log.LogTrace($"Moving \"{destinationFilePath}\" to \"{backupFilePath}\".");

                            this.MoveFile(destinationFilePath, backupFilePath);
                        }
                        catch (Exception ex)
                        {
                            if (debugEnabled)
                                this.log.LogDebug(ex, $"Error capturing backup of '{destinationFilePath}'");
                        }
                    }

                    // copy source file to destination
                    if (traceEnabled)
                        this.log.LogTrace($"Copying \"{sourceFilePath}\" to \"{destinationFilePath}\".");

                    this.CopyFile(sourceFilePath, destinationFilePath, overwriteExisting: true);

                    // delete source file
                    try
                    {
                        if (traceEnabled)
                            this.log.LogTrace($"Deleting \"{sourceFilePath}\".");

                        this.DeleteFile(sourceFilePath);
                    }
                    catch (Exception ex)
                    {
                        if (debugEnabled)
                            this.log.LogDebug(ex, $"    Error deleting '{sourceFilePath}'");
                    }
#endif
            });
    }

    public void DeleteFile(string filePath)
    {
        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to delete file \"{filePath}\".");

        if (this.FileExists(filePath))
        {
            this.AllowRetries(
                () =>
                {
#if NETFRAMEWORK
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException)
                    {
                        Alphaleonis.Win32.Filesystem.File.Delete(filePath);
                    }
#else
                    File.Delete(filePath);
#endif
                });
        }
    }

    public FileStream CreateFile(string filePath)
    {
        return new FileStream(filePath, FileMode.OpenOrCreate);
    }

    public string ReadFile(string filePath)
    {
#if NETFRAMEWORK
        try
        {
            return File.ReadAllText(filePath, this.GetFileEncoding(filePath));
        }
        catch (IOException)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Alphaleonis.Win32.Filesystem.File.ReadAllText(filePath, this.GetFileEncoding(filePath));

            throw;
        }
#else
        return File.ReadAllText(filePath, this.GetFileEncoding(filePath));
#endif
    }

    public byte[] ReadFileBytes(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }

    public FileStream OpenFileReadonly(string filePath)
    {
        return File.OpenRead(filePath);
    }

    public FileStream OpenFileExclusive(string filePath)
    {
        return File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    public void WriteFile(string filePath, string fileText)
    {
        var encoding = this.FileExists(filePath) ? this.GetFileEncoding(filePath) : Encoding.UTF8;
        this.WriteFile(filePath, fileText, encoding);
    }

    public void WriteFile(string filePath, string fileText, Encoding encoding)
    {
        this.AllowRetries(() =>
        {
            using var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            using var streamWriter = new StreamWriter(fileStream, encoding);
            streamWriter.Write(fileText);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        });
    }

    public void WriteFile(string filePath, Stream stream)
    {
        using var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        stream.CopyTo(fileStream);
        fileStream.Close();
    }

    public void WriteFile(string filePath, Func<Stream> getStream)
    {
        using var incomingStream = getStream();
        using var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        incomingStream.CopyTo(fileStream);
        fileStream.Close();
    }

    // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363851.aspx
    // http://www.pinvoke.net/default.aspx/kernel32.copyfile
    /*
        BOOL WINAPI CopyFile(
          _In_  LPCTSTR lpExistingFileName,
          _In_  LPCTSTR lpNewFileName,
          _In_  BOOL bFailIfExists
        );
     */
    [DllImport("kernel32", SetLastError = true)]
    private static extern int CopyFileW(string lpExistingFileName, string lpNewFileName, int bFailIfExists);
}