using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

using Bearz.Text;

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
        if (!Env.IsWindows())
        {
            ChOwn(path, userId, userId);
        }
    }

    [UnsupportedOSPlatform("windows")]
    public static void Chown(string path, int userId, int groupId)
    {
        if (!Env.IsWindows())
        {
            ChOwn(path, userId, groupId);
        }
    }

    [UnsupportedOSPlatform("windows")]
    public static void Chmod(string path, int mode)
    {
        if (!Env.IsWindows())
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
            string targetFilePath = Path.Combine(destinationDir, file.Name);
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
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
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
    public static FileSystemInfo Stat(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory) ? new DirectoryInfo(path) : new FileInfo(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileInfo StatFile(string path)
        => new(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DirectoryInfo StatDirectory(string path)
        => new(path);

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