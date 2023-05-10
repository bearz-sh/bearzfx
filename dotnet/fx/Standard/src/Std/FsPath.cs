using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

using P = System.IO.Path;

namespace Bearz.Std;

public static partial class FsPath
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Combine(string path1, string path2)
    {
        return P.Combine(path1, path2);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Combine(string path1, string path2, string path3)
    {
        return P.Combine(path1, path2, path3);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Combine(string path1, string path2, string path3, string path4)
    {
        return P.Combine(path1, path2, path3, path4);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Combine(params string[] paths)
    {
        return P.Combine(paths);
    }

    [Pure]
    public static string Resolve(string path)
        => Resolve(path, Env.Cwd);

    [Pure]
    public static string Resolve(string path, string basePath)
    {
        if (!P.IsPathRooted(basePath))
        {
            throw new InvalidOperationException("basePath must be an absolute path");
        }

        switch (path.Length)
        {
            case 0:
                return basePath;
            case 1:
                {
                    var c = path[0];
                    return c switch
                    {
                        '~' => Env.GetDirectory(SpecialDirectory.Home),
                        '.' => basePath,
                        _ => P.Combine(basePath, path),
                    };
                }

            default:
                {
                    var c1 = path[0];
                    var c2 = path[1];

                    switch (c1)
                    {
                        case '~' when c2 is '/' or '\\':
                            if (path.Length == 2)
                                return Env.GetDirectory(SpecialDirectory.Home);

                            path = path.Substring(2);
                            return P.GetFullPath(P.Combine(Env.GetDirectory(SpecialDirectory.Home), path));
                        case '.' when c2 is '/' or '\\':
                            if (path.Length == 2)
                                return basePath;

                            path = path.Substring(2);
                            return P.GetFullPath(P.Combine(basePath, path));
                        case '.' when c2 is '.':
                            // there could be wierd case when it starts with .. but has no separator
                            // this is treated as a relative path, otherwise this case ensures that .. and ../ is treated
                            // as a relative path.
                            return P.GetFullPath(P.Combine(basePath, path));

                        default:
                            if (P.IsPathRooted(path))
                                return P.GetFullPath(path);

                            return P.GetFullPath(c1 is not '.' and not '~' and not '/' and not '\\' ? P.Combine(basePath, path) : path);
                    }
                }
        }
    }

#if !NET7_0_OR_GREATER

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Exists([NotNullWhen(true)] string? path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }

#endif

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Basename(string? path)
    {
        return P.GetFileName(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetFileName(string? path)
    {
        return P.GetFileName(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? BasenameWithoutExtension(string? path)
    {
        return P.GetFileNameWithoutExtension(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetFileNameWithoutExtension(string? path)
    {
        return P.GetFileNameWithoutExtension(path);
    }

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ChangeExtension(string? path, string? extension)
    {
        return P.ChangeExtension(path, extension);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Dirname(string? path)
        => P.GetDirectoryName(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetDirectoryName(string? path)
        => P.GetDirectoryName(path);

    [Pure]
    [return: NotNullIfNotNull("path")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetExtension(string? path)
        => P.GetExtension(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPathRooted([NotNullWhen(true)] string? path)
        => P.IsPathRooted(path);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRandomFileName()
        => P.GetRandomFileName();

    [Pure]
    public static string GetNullDevice()
    {
        if (Env.IsWindows)
        {
            return @"\\.\NUL";
        }
        else
        {
            return "/dev/null";
        }
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetTempDir()
    {
        return P.GetTempPath();
    }
}