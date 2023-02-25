using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

using P = System.IO.Path;

namespace Bearz.Std;

public static partial class Path
{
    [Pure]
    public static string Combine(string path1, string path2)
    {
        return P.Combine(path1, path2);
    }

    [Pure]
    public static string Combine(string path1, string path2, string path3)
    {
        return P.Combine(path1, path2, path3);
    }

    [Pure]
    public static string Combine(string path1, string path2, string path3, string path4)
    {
        return P.Combine(path1, path2, path3, path4);
    }

    [Pure]
    public static string Combine(params string[] paths)
    {
        return P.Combine(paths);
    }

#if NETLEGACY || NET6_0

    public static bool Exists([NotNullWhen(true)] string? path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }

#endif

    [Pure]
    [return: NotNullIfNotNull("path")]
    public static string? Basename(string? path)
    {
        return P.GetFileName(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    public static string? BasenameWithoutExtension(string? path)
    {
        return P.GetFileNameWithoutExtension(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    public static string? ChangeExtension(string? path, string? extension)
    {
        return P.ChangeExtension(path, extension);
    }

    [Pure]
    public static string? Dirname(string? path)
        => P.GetDirectoryName(path);

    [Pure]
    [return: NotNullIfNotNull("path")]
    public static string? Extension(string? path)
        => P.GetExtension(path);

    [Pure]
    public static bool IsPathRooted([NotNullWhen(true)] string? path)
        => P.IsPathRooted(path);

    [Pure]
    public static string RandomFileName()
        => P.GetRandomFileName();

    public static string TempDir()
    {
        return P.GetTempPath();
    }
}