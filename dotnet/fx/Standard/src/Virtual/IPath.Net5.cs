#if NET5_0_OR_GREATER
namespace Bearz.Virtual;

public partial interface IPath
{
    ReadOnlySpan<char> BaseName(ReadOnlySpan<char> path);

    ReadOnlySpan<char> BaseNameWithoutExtension(ReadOnlySpan<char> path);

    ReadOnlySpan<char> ChangeExtension(ReadOnlySpan<char> path, ReadOnlySpan<char> extension);

    ReadOnlySpan<char> DirectoryName(ReadOnlySpan<char> path);

    bool EndsWithDirectorySeparator(ReadOnlySpan<char> path);

    string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2);

    string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3);

    string Join(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3, ReadOnlySpan<char> path4);

    string Join(string path1, string path2);

    string Join(string path1, string path2, string path3);

    string Join(string path1, string path2, string path3, string path4);

    string Join(params string[] paths);

    bool IsPathFullyQualified(string path);

    ReadOnlySpan<char> Resolve(ReadOnlySpan<char> path);

    ReadOnlySpan<char> Resolve(ReadOnlySpan<char> path, ReadOnlySpan<char> basePath);

    ReadOnlySpan<char> Trim(ReadOnlySpan<char> path);

    public bool TryJoin(
        ReadOnlySpan<char> path1,
        ReadOnlySpan<char> path2,
        Span<char> destination,
        out int charsWritten);

    public bool TryJoin(
        ReadOnlySpan<char> path1,
        ReadOnlySpan<char> path2,
        ReadOnlySpan<char> path3,
        Span<char> destination,
        out int charsWritten);
}

#endif