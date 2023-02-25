#if NET5_0_OR_GREATER
using System.Text;

namespace Bearz.Virtual;

public partial interface IFileSystem
{
    void Chown(string path, int userId, int groupId);

    FileStream OpenFile(string path, FileStreamOptions options);

    IEnumerable<string> ReadDirectory(string path, string searchPattern, EnumerationOptions enumerationOptions);

    Task<byte[]> ReadFileAsync(string path, CancellationToken cancellationToken = default);

    Task<string[]> ReadFileLinesAsync(string path, Encoding? encoding = null, CancellationToken cancellationToken = default);

    Task<string> ReadFileTextAsync(string path, Encoding? encoding = null, CancellationToken cancellationToken = default);

    FileSystemInfo SymlinkFile(string path, string target);

    FileSystemInfo SymlinkDirectory(string path, string target);
}

#endif