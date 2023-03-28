#if NET5_0_OR_GREATER

using System.Text;

using Bearz.Text;

namespace Bearz.Std;

public static partial class Fs
{
    public static Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
        => File.ReadAllBytesAsync(path, cancellationToken);

    public static IEnumerable<string> ReadDirectory(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);

    public static FileSystemInfo Symlink(string path, string target)
        => File.CreateSymbolicLink(path, target);

    public static FileSystemInfo SymlinkDirectory(string path, string target)
        => Directory.CreateSymbolicLink(path, target);

    public static FileSystemInfo? ResolveSymlink(string linkPath, bool returnFinalTarget = false)
        => File.ResolveLinkTarget(linkPath, returnFinalTarget);

    public static FileSystemInfo? ResolveSymlinkDirectory(string linkPath, bool returnFinalTarget = false)
        => Directory.ResolveLinkTarget(linkPath, returnFinalTarget);

    public static FileStream Open(string path, FileStreamOptions options)
        => File.Open(path, options);

    public static Task<string> ReadTextFileAsync(string path, Encoding? encoding = null, CancellationToken cancellationToken = default)
        => File.ReadAllTextAsync(path, encoding ?? Encoding.UTF8, cancellationToken);
}

#endif