using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;
using Bearz.Virtual;

using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Bearz.Std;

public static partial class Fs
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ReadFile(string path)
        => File.ReadAllBytes(path);

    [UnsupportedOSPlatform("windows")]
    public static void Chown(string path, int userId)
    {
        if (!Env.IsWindows)
        {
            ChOwn(path, userId, userId);
        }
    }

    [UnsupportedOSPlatform("windows")]
    public static void Chown(string path, int userId, int groupId)
    {
        if (!Env.IsWindows)
        {
            ChOwn(path, userId, groupId);
        }
    }

    [UnsupportedOSPlatform("windows")]
    public static void Chmod(string path, int mode)
    {
        if (!Env.IsWindows)
        {
            ChMod(path, mode);
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream CreateFile(string path)
        => File.Create(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream CreateFile(string path, int bufferSize)
        => File.Create(path, bufferSize);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream CreateFile(string path, int bufferSize, System.IO.FileOptions options)
        => File.Create(path, bufferSize, options);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamWriter CreateTextFile(string path)
        => File.CreateText(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileAttributes Attr(string path)
        => File.GetAttributes(path);

    public static void EnsureDirectory(string path)
    {
        if (!DirectoryExists(path))
            Directory.CreateDirectory(path);
    }

    public static void EnsureDirectoryForFile(string path)
    {
        path = FsPath.Resolve(path);
        var dir = Path.GetDirectoryName(path);
        if (dir.IsNullOrWhiteSpace())
            throw new ArgumentException("Path has no parent directory", nameof(path));

        if (!DirectoryExists(dir))
            Directory.CreateDirectory(dir);
    }

    public static bool TryEnsureDirectoryForFile(string path, [NotNullWhen(true)] out string? parentDirectory)
    {
        path = FsPath.Resolve(path);
        var dir = Path.GetDirectoryName(path);
        parentDirectory = dir;
        if (dir.IsNullOrWhiteSpace())
            return false;

        try
        {
            if (!DirectoryExists(dir))
                Directory.CreateDirectory(dir);

            parentDirectory = dir;
            return true;
        }
        catch
        {
            return false;
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDirectory(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFile(string path)
        => !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadTextFile(string path, Encoding? encoding = null)
        => File.ReadAllText(path, encoding ?? Encodings.Utf8NoBom);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<string> ReadAllLines(string path, Encoding? encoding = null)
        => File.ReadAllLines(path, encoding ?? Encodings.Utf8NoBom);

    public static string? GetExistingFile(string file, IReadOnlyCollection<string>? extensions)
    {
        if (!FsPath.IsPathRooted(file))
            file = FsPath.Resolve(file);

        if (FileExists(file))
            return file;

        if (extensions is not null)
        {
            var ext1 = FsPath.GetExtension(file);
            foreach (var ext in extensions)
            {
                if (ext == ext1)
                    continue;

                file = FsPath.ChangeExtension(file, ext);
                if (FileExists(file))
                    return file;
            }
        }

        return null;
    }

    [Pure]
    public static IReadOnlyList<string> GetExistingFiles(IEnumerable<string> files, IReadOnlyCollection<string>? extensions)
    {
        var existingFiles = new List<string>();
        foreach (var file in files)
        {
            if (file.IsNullOrWhiteSpace())
                continue;

            if (!FsPath.IsPathRooted(file))
            {
                var resolvedFile = FsPath.Resolve(file);
                if (FileExists(resolvedFile))
                {
                    existingFiles.Add(resolvedFile);
                    continue;
                }

                if (extensions is not null)
                {
                    var ext1 = FsPath.GetExtension(resolvedFile);
                    foreach (var ext in extensions)
                    {
                        if (ext1 == ext)
                            continue;

                        var resolvedFile2 = FsPath.ChangeExtension(resolvedFile, ext);
                        if (FileExists(resolvedFile2))
                        {
                            existingFiles.Add(resolvedFile2);
                            break;
                        }
                    }
                }

                continue;
            }

            if (FileExists(file))
                existingFiles.Add(file);

            if (extensions is not null)
            {
                var ext1 = FsPath.GetExtension(file);
                foreach (var ext in extensions)
                {
                    if (ext1 == ext)
                        continue;

                    var resolvedFile2 = FsPath.ChangeExtension(file, ext);
                    if (FileExists(resolvedFile2))
                    {
                        existingFiles.Add(resolvedFile2);
                        break;
                    }
                }
            }
        }

        return existingFiles;
    }

    [Pure]
    public static IReadOnlyList<string> GetExistingFiles(params string[] files)
    {
        var existingFiles = new List<string>();
        foreach (var file in files)
        {
            if (file.IsNullOrWhiteSpace())
                continue;

            if (!FsPath.IsPathRooted(file))
            {
                var resolvedFile = FsPath.Resolve(file);
                if (FileExists(resolvedFile))
                {
                    existingFiles.Add(resolvedFile);
                    continue;
                }
            }

            if (FileExists(file))
                existingFiles.Add(file);
        }

        return existingFiles;
    }

    public static string CatFiles(bool throwIfNotFound, params string[] files)
    {
        var sb = StringBuilderCache.Acquire();
        foreach (var file in files)
        {
            if (throwIfNotFound && !File.Exists(file))
                throw new FileNotFoundException($"File not found: {file}");

            if (sb.Length > 0)
                sb.Append('\n');

            sb.Append(ReadTextFile(file));
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool force = false)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        if (!DirectoryExists(destinationDir))
            Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = FsPath.Combine(destinationDir, file.Name);
            if (FileExists(targetFilePath))
            {
                if (force)
                    File.Delete(targetFilePath);
                else
                    throw new IOException($"File already exists: {targetFilePath}");
            }

            file.CopyTo(targetFilePath);
        }

        if (!recursive)
        {
            return;
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = FsPath.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, true, force);
        }
    }

    public static void MoveDirectory(string sourceDir, string destinationDir, bool force = false)
    {
        if (force && DirectoryExists(destinationDir))
            Directory.Delete(destinationDir, true);

        Directory.Move(sourceDir, destinationDir);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool DirectoryExists([NotNullWhen(true)] string? path)
        => Directory.Exists(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool FileExists([NotNullWhen(true)] string? path)
        => File.Exists(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteFile(string path, byte[] data)
        => File.WriteAllBytes(path, data);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream Open(string path)
        => File.Open(path, FileMode.OpenOrCreate);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream Open(string path, FileMode mode)
        => File.Open(path, mode);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream Open(string path, FileMode mode, FileAccess access)
        => File.Open(path, mode, access);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        => File.Open(path, mode, access, share);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyFile(string source, string destination, bool overwrite = false)
        => File.Copy(source, destination, overwrite);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MoveFile(string source, string destination)
        => File.Move(source, destination);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> ReadDirectory(string path)
        => Directory.EnumerateFileSystemEntries(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> ReadDirectory(string path, string searchPattern)
        => Directory.EnumerateFileSystemEntries(path, searchPattern);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> ReadDirectory(string path, string searchPattern, SearchOption searchOption)
        => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteAllLines(string path, IEnumerable<string> lines)
        => File.WriteAllLines(path, lines);

    public static void WriteTextFile(string path, string? contents, Encoding? encoding = null, bool append = false)
    {
        if (append)
            File.AppendAllText(path, contents, encoding ?? Encodings.Utf8NoBom);
        else
            File.WriteAllText(path, contents, encoding ?? Encodings.Utf8NoBom);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MakeDirectory(string path)
        => Directory.CreateDirectory(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFile(string path)
        => File.Delete(path);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveDirectory(string path, bool recursive = false)
        => Directory.Delete(path, recursive);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFileSystemInfo Stat(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory) ? new VirtualDirectoryInfo(path) : new VirtualFileInfo(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFileInfo StatFile(string path)
        => new VirtualFileInfo(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDirectoryInfo StatDirectory(string path)
        => new VirtualDirectoryInfo(path);

#if NET7_0_OR_GREATER
    [LibraryImport("libc", EntryPoint = "chown", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int ChOwn(string path, int owner, int group);

    [LibraryImport("libc", EntryPoint = "lchown", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int LChOwn(string path, int owner, int group);

    [LibraryImport("libSystem.Native", EntryPoint = "SystemNative_ChMod", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int ChMod(string path, int mode);
#else
    [DllImport("libc", EntryPoint = "chown", SetLastError = true)]
    internal static extern int ChOwn(string path, int owner, int group);

    [DllImport("libc", EntryPoint = "lchown", SetLastError = true)]
    internal static extern int LChOwn(string path, int owner, int group);

    [DllImport("libSystem.Native", EntryPoint = "SystemNative_ChMod", SetLastError = true)]
    internal static extern int ChMod(string path, int mode);
#endif
}