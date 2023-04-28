using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Bearz.Virtual;

public partial interface IPath
{
    char PathSeparator { get; }

    char AltDirectorySeparator { get; }

    char DirectorySeparator { get; }

    char VolumeSeparator { get; }

    string Resolve(string path);

    string Resolve(string path, string basePath);

    string? ChangeExtension(string? path, string? extension);

    string Combine(string path1, string path2);

    string Combine(string path1, string path2, string path3);

    string Combine(string path1, string path2, string path3, string path4);

    string Combine(params string[] paths);

    string? DirectoryName(string path);

    [return: NotNullIfNotNull("path")]
    string? BaseName(string path);

    [return: NotNullIfNotNull("path")]
    string? BaseNameWithoutExtension(string path);

    [Pure]
    [return: NotNullIfNotNull("path")]
    string? Extension(string? path);

    bool EndsWithDirectorySeparator(string path);

    bool IsPathRooted([NotNullWhen(true)] string? path);

    string TempDir();

    string Trim(string path);
}