#if NET5_0_OR_GREATER
using P = System.IO.Path;
using Path = Bearz.Std.Path;

namespace Bearz.Virtual;

public partial class VirtualPath
{
    public ReadOnlySpan<char> BaseName(ReadOnlySpan<char> path)
        => P.GetFileName(path);

    public ReadOnlySpan<char> BaseNameWithoutExtension(ReadOnlySpan<char> path)
        => P.GetFileNameWithoutExtension(path);

    public ReadOnlySpan<char> ChangeExtension(ReadOnlySpan<char> path, ReadOnlySpan<char> extension)
    {
        if (path.IsEmpty)
            return path;

        var ext = P.GetExtension(path);
        if (ext.IsEmpty)
            return path;
        Span<char> result = new char[(path.Length - ext.Length) + extension.Length];
        path.Slice(0, path.Length - ext.Length).CopyTo(result);
        extension.CopyTo(result.Slice(path.Length - ext.Length));
        return result;
    }

    public ReadOnlySpan<char> DirectoryName(ReadOnlySpan<char> path)
        => P.GetDirectoryName(path);

    public bool EndsWithDirectorySeparator(ReadOnlySpan<char> path)
        => path[^1] is '/' or '\\';

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2)
        => P.Join(path1, path2);

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3)
        => P.Join(path1, path2, path3);

    public string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3, ReadOnlySpan<char> path4)
        => P.Join(path1, path2, path3, path4);

    public string Join(string? path1, string? path2)
        => P.Join(path1, path2);

    public string Join(string? path1, string? path2, string? path3)
        => P.Join(path1, path2, path3);

    public string Join(string? path1, string? path2, string? path3, string? path4)
        => P.Join(path1, path2, path3, path4);

    public string Join(params string?[] paths)
        => P.Join(paths);

    public bool IsPathFullyQualified(ReadOnlySpan<char> path)
        => P.IsPathFullyQualified(path);

    public bool IsPathFullyQualified(string path)
        => P.IsPathFullyQualified(path);

    public ReadOnlySpan<char> Resolve(ReadOnlySpan<char> path)
        => this.Resolve(path, this.env.Cwd);

    public ReadOnlySpan<char> Resolve(ReadOnlySpan<char> path, ReadOnlySpan<char> basePath)
    {
        switch (path.Length)
        {
            case 0:
                return basePath;
            case 1:
                var c = path[0];
                return c switch
                {
                    '~' => this.env.HomeDir,
                    '.' => basePath,
                    _ => P.Join(basePath, path),
                };

            default:
                {
                    var c1 = path[0];
                    var c2 = path[1];
                    switch (c1)
                    {
                        case '~' when c2 is '/' or '\\':
                            if (path.Length == 2)
                                return this.env.HomeDir;

                            path = path.Slice(2);
                            return P.GetFullPath(P.Join(this.env.HomeDir, path));
                        case '.' when c2 is '/' or '\\':
                            if (path.Length == 2)
                                return basePath;

                            path = path.Slice(2);
                            return P.GetFullPath(P.Join(basePath, path));
                        case '.' when c2 is '.':
                            // there could be wierd case when it starts with .. but has no separator
                            // this is treated as a relative path, otherwise this case ensures that .. and ../ is treated
                            // as a relative path.
                            return P.GetFullPath(P.Join(basePath, path));

                        default:
                            if (P.IsPathRooted(path))
                                return P.GetFullPath(path.ToString());

                            return P.GetFullPath(c1 is not '.' and not '~' and not '/' and not '\\' ? P.Join(basePath, path) : path.ToString());
                    }
                }
        }
    }

    public ReadOnlySpan<char> Trim(ReadOnlySpan<char> path)
    {
        if (path.IsEmpty)
            return path;

        var c = path[^1];
        while (c is '/' or '\\')
        {
            path = path.Slice(0, path.Length - 1);
            if (path.IsEmpty)
                return path;

            c = path[^1];
        }

        Span<char> result = new char[path.Length];
        path.CopyTo(result);
        return result;
    }

    public bool TryJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, Span<char> destination, out int charsWritten)
        => P.TryJoin(path1, path2, destination, out charsWritten);

    public bool TryJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3, Span<char> destination, out int charsWritten)
        => P.TryJoin(path1, path2, path3, destination, out charsWritten);
}

#endif