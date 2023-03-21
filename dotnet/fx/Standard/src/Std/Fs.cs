using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

using Bearz.Text;

using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Bearz.Std;

public static partial class Fs
{
    public static byte[] ReadFile(string path)
        => File.ReadAllBytes(path);

    public static void Chown(string path, int userId, int groupId)
    {
        if (!Env.IsWindows())
        {
            ChOwn(path, userId, groupId);
        }
    }

    public static void Chmod(string path, int mode)
    {
        if (!Env.IsWindows())
        {
            ChMod(path, mode);
        }
    }

    public static FileAttributes Attr(string path)
        => File.GetAttributes(path);

#if NET6_0_OR_GREATER
    public static Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllBytesAsync(path, cancellationToken);
#endif

    public static string ReadTextFile(string path, Encoding? encoding = null)
        => File.ReadAllText(path, encoding ?? Encodings.Utf8NoBom);

    public static IReadOnlyList<string> ReadAllLines(string path, Encoding? encoding = null)
        => File.ReadAllLines(path, encoding ?? Encodings.Utf8NoBom);

#if NET6_0_OR_GREATER
    public static Task<string> ReadTextFileAsync(string path, Encoding? encoding = null, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, encoding ?? Encodings.Utf8NoBom, cancellationToken);
#endif

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

    public static bool FileExists([NotNullWhen(true)] string? path)
        => File.Exists(path);

    public static bool DirectoryExits([NotNullWhen(true)] string? path)
        => Directory.Exists(path);

    public static void WriteFile(string path, byte[] data)
        => File.WriteAllBytes(path, data);

    public static FileStream Open(string path)
        => File.Open(path, FileMode.OpenOrCreate);

    public static FileStream Open(string path, FileMode mode)
        => File.Open(path, mode);

    public static FileStream Open(string path, FileMode mode, FileAccess access)
        => File.Open(path, mode, access);

    public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        => File.Open(path, mode, access, share);

#if NET6_0_OR_GREATER
    public static FileStream Open(string path, FileStreamOptions options)
        => File.Open(path, options);
#endif

    public static void CopyFile(string source, string destination, bool overwrite = false)
        => File.Copy(source, destination, overwrite);

    public static void MoveFile(string source, string destination)
        => File.Move(source, destination);

    public static IEnumerable<string> ReadDirectory(string path)
        => Directory.EnumerateFileSystemEntries(path);

    public static IEnumerable<string> ReadDirectory(string path, string searchPattern)
        => Directory.EnumerateFileSystemEntries(path, searchPattern);

    public static IEnumerable<string> ReadDirectory(string path, string searchPattern, SearchOption searchOption)
        => Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);

#if NET6_0_OR_GREATER
    public static IEnumerable<string> ReadDirectory(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
#endif

#if NET5_0_OR_GREATER
    public static FileSystemInfo Symlink(string path, string target)
        => File.CreateSymbolicLink(path, target);
#endif

    public static void WriteAllLines(string path, IEnumerable<string> lines)
        => File.WriteAllLines(path, lines);

    public static void WriteTextFile(string path, string? contents, Encoding? encoding = null, bool append = false)
    {
        if (append)
            File.AppendAllText(path, contents, encoding ?? Encodings.Utf8NoBom);
        else
            File.WriteAllText(path, contents, encoding ?? Encodings.Utf8NoBom);
    }

    public static void MakeDirectory(string path)
        => Directory.CreateDirectory(path);

#if NET7_0_OR_GREATER
    public static void MakeDirectory(string path, UnixFileMode mode)
        => Directory.CreateDirectory(path, mode);
#endif

    public static void RemoveFile(string path)
        => File.Delete(path);

    public static void RemoveDirectory(string path, bool recursive = false)
        => Directory.Delete(path, recursive);

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