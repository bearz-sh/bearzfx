using System.Diagnostics.CodeAnalysis;

using P = System.IO.Path;
using Path = Bearz.Std.Path;

namespace Bearz.Virtual;

public partial class VirtualPath : IPath
{
    private readonly IEnvironment env;

    public VirtualPath(IEnvironment env)
    {
        this.env = env;
    }

    public char PathSeparator => P.PathSeparator;

    public char AltDirectorySeparator => P.AltDirectorySeparatorChar;

    public char DirectorySeparator => P.DirectorySeparatorChar;

    public char VolumeSeparator => P.VolumeSeparatorChar;

    public string Resolve(string path)
        => this.Resolve(path, this.env.Cwd);

    public string Resolve(string path, string basePath)
    {
        switch (path.Length)
        {
            case 0:
                return basePath;
            case 1:
                {
                    var c = path[0];
                    return c switch
                    {
                        '~' => this.env.HomeDir,
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
                                return this.env.HomeDir;

                            path = path.Substring(2);
                            return P.GetFullPath(P.Combine(this.env.HomeDir, path));
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

    [return: NotNullIfNotNull("path")]
    public string? ChangeExtension(string? path, string? extension)
        => P.ChangeExtension(path, extension);

    public string Combine(string path1, string path2)
        => P.Combine(path1, path2);

    public string Combine(string path1, string path2, string path3)
        => P.Combine(path1, path2, path3);

    public string Combine(string path1, string path2, string path3, string path4)
        => P.Combine(path1, path2, path3, path3);

    public string Combine(params string[] paths)
        => P.Combine(paths);

    public string? DirectoryName(string? path)
        => P.GetDirectoryName(path);

    [return: NotNullIfNotNull("path")]
    public string? BaseName(string? path)
        => P.GetFileName(path);

    [return: NotNullIfNotNull("path")]
    public string? BaseNameWithoutExtension(string? path)
        => P.GetFileNameWithoutExtension(path);

    [return: NotNullIfNotNull("path")]
    public string? Extension(string? path)
        => P.GetExtension(path);

    public bool EndsWithDirectorySeparator(string path)
        => path[path.Length - 1] is '/' or '\\';

    public bool IsPathRooted([NotNullWhen(true)] string? path)
        => P.IsPathRooted(path);

    public string TempDir()
        => P.GetTempPath().TrimEnd(P.DirectorySeparatorChar);

    public string Trim(string path)
        => path.TrimEnd('/').TrimEnd('\\');
}