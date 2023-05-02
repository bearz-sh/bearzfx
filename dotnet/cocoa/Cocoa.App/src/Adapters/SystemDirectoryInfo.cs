namespace Cocoa.Adapters;

public class SystemDirectoryInfo : IDirectoryInfo
{
    private readonly DirectoryInfo directoryInfo;

    private SystemDirectoryInfo? parent;

    private SystemDirectoryInfo? root;

    public SystemDirectoryInfo(string directory)
    {
        this.directoryInfo = new DirectoryInfo(directory);
    }

    public SystemDirectoryInfo(DirectoryInfo directoryInfo)
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

            return this.parent ??= new SystemDirectoryInfo(this.directoryInfo.Parent);
        }
    }

    public IDirectoryInfo Root => this.root ??= new SystemDirectoryInfo(this.directoryInfo.Root);

    public string Name => this.directoryInfo.Name;

    public void Refresh()
        => this.directoryInfo.Refresh();

    public void Create()
        => this.directoryInfo.Create();

    public IDirectoryInfo CreateSubdirectory(string path)
        => new SystemDirectoryInfo(this.directoryInfo.CreateSubdirectory(path));

    public void Delete()
        => this.directoryInfo.Delete();

    public void Delete(bool recursive)
        => this.directoryInfo.Delete(recursive);

    public void MoveTo(string destDirName)
        => this.directoryInfo.MoveTo(destDirName);

    public IEnumerable<IDirectoryInfo> EnumerateDirectories()
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories())
            yield return new SystemDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories(searchPattern))
            yield return new SystemDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
    {
        foreach (var directoryInfo in this.directoryInfo.EnumerateDirectories(searchPattern, searchOption))
            yield return new SystemDirectoryInfo(directoryInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles()
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles())
            yield return new SystemFileInfo(fileInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles(searchPattern))
            yield return new SystemFileInfo(fileInfo);
    }

    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        foreach (var fileInfo in this.directoryInfo.EnumerateFiles(searchPattern, searchOption))
            yield return new SystemFileInfo(fileInfo);
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos())
        {
            if (fi is DirectoryInfo di)
                yield return new SystemDirectoryInfo(di);
            else
                yield return new SystemFileInfo((FileInfo)fi);
        }
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos(searchPattern))
        {
            if (fi is DirectoryInfo di)
                yield return new SystemDirectoryInfo(di);
            else
                yield return new SystemFileInfo((FileInfo)fi);
        }
    }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        foreach (var fi in this.directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption))
        {
            if (fi is DirectoryInfo di)
                yield return new SystemDirectoryInfo(di);
            else
                yield return new SystemFileInfo((FileInfo)fi);
        }
    }

    public IDirectoryInfo[] GetDirectories()
    {
        var results = this.directoryInfo.GetDirectories();
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemDirectoryInfo(results[i]);

        return set;
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern)
    {
        var results = this.directoryInfo.GetDirectories(searchPattern);
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemDirectoryInfo(results[i]);

        return set;
    }

    public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
    {
        var results = this.directoryInfo.GetDirectories(searchPattern, searchOption);
        var set = new IDirectoryInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemDirectoryInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles()
    {
        var results = this.directoryInfo.GetFiles();
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemFileInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles(string searchPattern)
    {
        var results = this.directoryInfo.GetFiles(searchPattern);
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemFileInfo(results[i]);

        return set;
    }

    public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
    {
        var results = this.directoryInfo.GetFiles(searchPattern, searchOption);
        var set = new IFileInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
            set[i] = new SystemFileInfo(results[i]);

        return set;
    }

    public IFileSystemInfo[] GetFileSystemInfos()
    {
        var results = this.directoryInfo.GetFileSystemInfos();
        var set = new IFileSystemInfo[results.Length];
        for (var i = 0; i < results.Length; i++)
        {
            var next = results[i];
            if (next is DirectoryInfo di)
                set[i] = new SystemDirectoryInfo(di);
            else
                set[i] = new SystemFileInfo((FileInfo)next);
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
            if (next is DirectoryInfo di)
                set[i] = new SystemDirectoryInfo(di);
            else
                set[i] = new SystemFileInfo((FileInfo)next);
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
            if (next is DirectoryInfo di)
                set[i] = new SystemDirectoryInfo(di);
            else
                set[i] = new SystemFileInfo((FileInfo)next);
        }

        return set;
    }
}