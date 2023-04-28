#if NET5_0_OR_GREATER
using System.Text;

using Bearz.Std;

// ReSharper disable ParameterHidesMember
namespace Bearz.Virtual;

public partial class VirtualFileSystem
{
    public void Chown(string path, int userId, int groupId)
    {
        path = this.path.Resolve(path);
        Fs.Chown(path, userId, groupId);
    }

    public FileStream OpenFile(string path, FileStreamOptions options)
    {
        path = this.path.Resolve(path);
        return Fs.Open(path, options);
    }

    public IEnumerable<string> ReadDirectory(string path, string searchPattern, EnumerationOptions enumerationOptions)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);
    }

    public Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default)
    {
        path = this.path.Resolve(path);
        return File.ReadAllBytesAsync(path, cancellationToken);
    }

    public Task<string[]> ReadFileLinesAsync(
        string path,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
        path = this.path.Resolve(path);
        if (encoding is null)
            return File.ReadAllLinesAsync(path, cancellationToken);

        return File.ReadAllLinesAsync(path, encoding, cancellationToken);
    }

    public Task<string> ReadFileTextAsync(
        string path,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
        path = this.path.Resolve(path);
        if (encoding is null)
            return File.ReadAllTextAsync(path, cancellationToken);

        return File.ReadAllTextAsync(path, encoding, cancellationToken);
    }

    public FileSystemInfo SymlinkFile(string path, string target)
    {
        path = this.path.Resolve(path);
        target = this.path.Resolve(target);
        return File.CreateSymbolicLink(path, target);
    }

    public FileSystemInfo SymlinkDirectory(string path, string target)
    {
        path = this.path.Resolve(path);
        target = this.path.Resolve(target);
        return Directory.CreateSymbolicLink(path, target);
    }
}

#endif