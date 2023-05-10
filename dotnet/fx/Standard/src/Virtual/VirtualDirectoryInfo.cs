using System.Runtime.Versioning;

using Bearz.Std;

namespace Bearz.Virtual;

public class VirtualDirectoryInfo : IDirectoryInfo
{
    private readonly DirectoryInfo directoryInfo;

#if !NET7_0_OR_GREATER
    private UnixFileMode? unixFileMode;
#endif

    private IDirectoryInfo? root;

    private IDirectoryInfo? parent;

    public VirtualDirectoryInfo(string path)
    {
        this.directoryInfo = new DirectoryInfo(path);
    }

    public VirtualDirectoryInfo(DirectoryInfo directoryInfo)
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

    public string FullName => this.directoryInfo.Name;

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

    public DateTime LastWriteTimeUtc
    {
        get => this.directoryInfo.LastWriteTimeUtc;
        set => this.directoryInfo.LastWriteTimeUtc = value;
    }

    public string Name => this.directoryInfo.Name;

    [UnsupportedOSPlatform("windows")]
    public UnixFileMode UnixFileMode
    {
        get
        {
#if NET7_0_OR_GREATER
            return this.directoryInfo.UnixFileMode;
#else
            if (Env.IsWindows)
                return UnixFileMode.None;

            if (this.unixFileMode is null)
            {
                if (!this.directoryInfo.Exists)
                {
                    this.unixFileMode = UnixFileMode.None;
                }
                else
                {
                    Interop.Sys.Stat(this.directoryInfo.FullName, out var stat);
                    this.unixFileMode = (UnixFileMode)(stat.Mode & (int)VirtualFileSystem.ValidUnixFileModes);
                }
            }

            return this.unixFileMode.Value;
#endif
        }

        set
        {
#if NET7_0_OR_GREATER
            this.directoryInfo.UnixFileMode = value;
#else
            if (Env.IsWindows)
            {
                this.unixFileMode = value;
                return;
            }

            this.unixFileMode = value;
            if (this.directoryInfo.Exists)
            {
                Fs.Chmod(this.directoryInfo.FullName, (int)this.unixFileMode);
            }
#endif
        }
    }

    public IDirectoryInfo? Parent
    {
        get
        {
            if (this.directoryInfo.Parent is null)
                return null;

            return this.parent ??= new VirtualDirectoryInfo(this.directoryInfo.Parent);
        }
    }

    public IDirectoryInfo Root => this.root ??= new VirtualDirectoryInfo(this.directoryInfo.Root);

    public virtual void Delete()
        => this.directoryInfo.Delete();

    public virtual void Refresh()
    {
#if !NET7_0_OR_GREATER
        this.unixFileMode = null;
#endif
        this.directoryInfo.Refresh();
    }

    public virtual void Create()
    {
        throw new NotImplementedException();
    }

    public virtual IDirectoryInfo CreateSubdirectory(string path)
        => new VirtualDirectoryInfo(this.directoryInfo.CreateSubdirectory(path));

    public virtual void Delete(bool recursive)
        => this.directoryInfo.Delete(recursive);

    public virtual IEnumerable<IDirectoryInfo> EnumerateDirectories()
    {
        foreach (var info in this.directoryInfo.EnumerateDirectories())
        {
            yield return new VirtualDirectoryInfo(info);
        }
    }

    public virtual IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
    {
        foreach (var info in this.directoryInfo.EnumerateDirectories(searchPattern))
        {
            yield return new VirtualDirectoryInfo(info);
        }
    }

    public virtual IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
    {
        foreach (var info in this.directoryInfo.EnumerateDirectories(searchPattern, searchOption))
        {
            yield return new VirtualDirectoryInfo(info);
        }
    }

    public virtual IEnumerable<IFileInfo> EnumerateFiles()
    {
        foreach (var info in this.directoryInfo.EnumerateFiles())
        {
            yield return new VirtualFileInfo(info);
        }
    }

    public virtual IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
    {
        foreach (var info in this.directoryInfo.EnumerateFiles(searchPattern))
        {
            yield return new VirtualFileInfo(info);
        }
    }

    public virtual IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
    {
        foreach (var info in this.directoryInfo.EnumerateFiles(searchPattern, searchOption))
        {
            yield return new VirtualFileInfo(info);
        }
    }

    public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
    {
        foreach (var info in this.directoryInfo.EnumerateFileSystemInfos())
        {
            if (info is DirectoryInfo dirInfo)
                yield return new VirtualDirectoryInfo(dirInfo);
            else if (info is FileInfo fileInfo)
                yield return new VirtualFileInfo(fileInfo);
            else
                throw new NotSupportedException();
        }
    }

    public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
    {
        foreach (var info in this.directoryInfo.EnumerateFileSystemInfos(searchPattern))
        {
            if (info is DirectoryInfo dirInfo)
                yield return new VirtualDirectoryInfo(dirInfo);
            else if (info is FileInfo fileInfo)
                yield return new VirtualFileInfo(fileInfo);
            else
                throw new NotSupportedException();
        }
    }

    public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
    {
        foreach (var info in this.directoryInfo.EnumerateFileSystemInfos(searchPattern, searchOption))
        {
            if (info is DirectoryInfo dirInfo)
                yield return new VirtualDirectoryInfo(dirInfo);
            else if (info is FileInfo fileInfo)
                yield return new VirtualFileInfo(fileInfo);
            else
                throw new NotSupportedException();
        }
    }

    public virtual void MoveTo(string destDirName)
        => this.directoryInfo.MoveTo(destDirName);
}