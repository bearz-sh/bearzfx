#if NETFRAMEWORK

using Alphaleonis.Win32.Filesystem;

namespace Cocoa.Adapters;

public class AlphaFsDirectoryInfo : IDirectoryInfo
{
    private readonly Alphaleonis.Win32.Filesystem.DirectoryInfo directoryInfo;

    private AlphaFsDirectoryInfo? parent;

    private AlphaFsDirectoryInfo? root;

    public AlphaFsDirectoryInfo(string directory)
    {
        this.directoryInfo = new Alphaleonis.Win32.Filesystem.DirectoryInfo(directory);
    }

    public AlphaFsDirectoryInfo(Alphaleonis.Win32.Filesystem.DirectoryInfo directoryInfo)
    {
        this.directoryInfo = directoryInfo;
    }

    public FileAttributes Attributes
    {
        get => this.directoryInfo.Attributes;
        set => this.directoryInfo.Attributes = value;
    }

    public DateTime CreationTime
    {
        get => this.directoryInfo.CreationTime;
        set => this.directoryInfo.CreationTime = value;
    }

    public DateTime CreationTimeUtc
    {
        get => this.directoryInfo.CreationTimeUtc;
        set => this.directoryInfo.CreationTimeUtc = value;
    }

    public bool Exists => this.directoryInfo.Exists;

    public string Extension => this.directoryInfo.Extension;

    public string FullName => this.directoryInfo.FullName;

    public DateTime LastAccessTime
    {
        get => this.directoryInfo.LastAccessTime;
        set => this.directoryInfo.LastAccessTime = value;
    }

    public DateTime LastAccessTimeUtc
    {
        get => this.directoryInfo.LastAccessTimeUtc;
        set => this.directoryInfo.LastAccessTimeUtc = value;
    }

    public DateTime LastWriteTime
    {
        get => this.directoryInfo.LastWriteTime;
        set => this.directoryInfo.LastWriteTime = value;
    }

    public DateTime LastWriteTimeUtc { get; set; }

    public IDirectoryInfo? Parent
    {
        get
        {
            if (this.directoryInfo.Parent is null)
                return null;

            return this.parent ??= new AlphaFsDirectoryInfo(this.directoryInfo.Parent);
        }
    }

    public IDirectoryInfo Root => this.root ??= new AlphaFsDirectoryInfo(this.directoryInfo.Root);

    public string Name => this.directoryInfo.Name;

    public void Refresh()
        => this.directoryInfo.Refresh();

    public void Create()
        => this.directoryInfo.Create();

    public IDirectoryInfo CreateSubdirectory(string path)
        => new AlphaFsDirectoryInfo(this.directoryInfo.CreateSubdirectory(path));

    public void Delete()
        => this.directoryInfo.Delete();

    public void Delete(bool recursive)
        => this.directoryInfo.Delete(recursive);

    public void MoveTo(string destDirName)
        => this.directoryInfo.MoveTo(destDirName);

    public void MoveTo(string destDirName, bool overwrite)
    {
        var options = overwrite ? MoveOptions.ReplaceExisting : MoveOptions.None;
        this.directoryInfo.MoveTo(destDirName, options);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories()
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories())
            yield return new AlphaFsDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories(searchPattern))
            yield return new AlphaFsDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories(searchPattern, searchOption))
            yield return new AlphaFsDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles()
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles())
            yield return new AlphaFsFileInfo(fileInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles(searchPattern))
            yield return new AlphaFsFileInfo(fileInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles(searchPattern, searchOption))
            yield return new AlphaFsFileInfo(fileInfo);
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos())
        {
            if (fi is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                yield return new AlphaFsDirectoryInfo(di);
            else
                yield return new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)fi);
        }
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos(searchPattern))
        {
            if (fi is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                yield return new AlphaFsDirectoryInfo(di);
            else
                yield return new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)fi);
        }
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption))
        {
            if (fi is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                yield return new AlphaFsDirectoryInfo(di);
            else
                yield return new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)fi);
        }
    }

    public IDirectoryInfo[] GetDirectories()
    {
        var results = this.directoryInfo.GetDirectories();
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsDirectoryInfo(results[i]);

        return set;
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern)
    {
        var results = this.directoryInfo.GetDirectories(searchPattern);
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsDirectoryInfo(results[i]);

        return set;
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
    {
        var results = this.directoryInfo.GetDirectories(searchPattern, searchOption);
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsDirectoryInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles()
    {
        var results = this.directoryInfo.GetFiles();
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsFileInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles(string searchPattern)
    {
        var results = this.directoryInfo.GetFiles(searchPattern);
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsFileInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
    {
        var results = this.directoryInfo.GetFiles(searchPattern, searchOption);
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new AlphaFsFileInfo(results[i]);

        return set;
    }

    public IFileSystemInfo[] GetFileSystemInfos()
    {
        var results = this.directoryInfo.GetFileSystemInfos();
        var set = new IFileSystemInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
        {
            var next = results[i];
            if (next is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                set[i] = new AlphaFsDirectoryInfo(di);
            else
                set[i] = new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)next);
        }

        return set;
    }

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
    {
        var results = this.directoryInfo.GetFileSystemInfos(searchPattern);
        var set = new IFileSystemInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
        {
            var next = results[i];
            if (next is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                set[i] = new AlphaFsDirectoryInfo(di);
            else
                set[i] = new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)next);
        }

        return set;
    }

    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        var results = this.directoryInfo.GetFileSystemInfos(searchPattern);
        var set = new IFileSystemInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
        {
            var next = results[i];
            if (next is Alphaleonis.Win32.Filesystem.DirectoryInfo di)
                set[i] = new AlphaFsDirectoryInfo(di);
            else
                set[i] = new AlphaFsFileInfo((Alphaleonis.Win32.Filesystem.FileInfo)next);
        }

        return set;
    }
}

#endif