namespace Cocoa.Adapters;

public interface IDirectoryInfo : IFileSystemInfo
{
    IDirectoryInfo? Parent { get; }

    IDirectoryInfo Root { get; }

    void Create();

    IDirectoryInfo CreateSubdirectory(string path);

    void Delete(bool recursive);

    void MoveTo(string destDirName);

    IEnumerable<IDirectoryInfo> EnumerateDirectories();

    IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern);

    IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption);

    IEnumerable<IFileInfo> EnumerateFiles();

    IEnumerable<IFileInfo> EnumerateFiles(string searchPattern);

    IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption);

    IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos();

    IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern);

    IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption);

    IDirectoryInfo[] GetDirectories();

    IDirectoryInfo[] GetDirectories(string searchPattern);

    IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption);

    IFileInfo[] GetFiles();

    IFileInfo[] GetFiles(string searchPattern);

    IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption);

    IFileSystemInfo[] GetFileSystemInfos();

    IFileSystemInfo[] GetFileSystemInfos(string searchPattern);

    IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption);
}