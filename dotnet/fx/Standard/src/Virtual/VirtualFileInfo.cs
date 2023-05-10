using System.Runtime.Versioning;

using Bearz.Std;

namespace Bearz.Virtual;

public class VirtualFileInfo : IFileInfo
{
    private readonly FileInfo fileInfo;

#if !NET7_0_OR_GREATER
    private UnixFileMode? unixFileMode;
#endif

    private IDirectoryInfo? directory;

    public VirtualFileInfo(string path)
    {
        this.fileInfo = new FileInfo(path);
    }

    public VirtualFileInfo(FileInfo fileInfo)
    {
        this.fileInfo = fileInfo;
    }

    public FileAttributes Attributes
    {
        get => this.fileInfo.Attributes;
        set => this.fileInfo.Attributes = value;
    }

    public DateTime CreationTime
    {
        get => this.fileInfo.CreationTime;
        set => this.fileInfo.CreationTime = value;
    }

    public DateTime CreationTimeUtc
    {
        get => this.fileInfo.CreationTimeUtc;
        set => this.fileInfo.CreationTimeUtc = value;
    }

    public bool Exists => this.fileInfo.Exists;

    public string Extension => this.fileInfo.Extension;

    public string FullName => this.fileInfo.FullName;

    public DateTime LastAccessTime
    {
        get => this.fileInfo.LastAccessTime;
        set => this.fileInfo.LastAccessTime = value;
    }

    public DateTime LastAccessTimeUtc
    {
        get => this.fileInfo.LastAccessTimeUtc;
        set => this.fileInfo.LastAccessTimeUtc = value;
    }

    public DateTime LastWriteTime
    {
        get => this.fileInfo.LastWriteTime;
        set => this.fileInfo.LastWriteTime = value;
    }

    public DateTime LastWriteTimeUtc
    {
        get => this.fileInfo.LastWriteTimeUtc;
        set => this.fileInfo.LastWriteTimeUtc = value;
    }

    public string Name => this.fileInfo.Name;

    [UnsupportedOSPlatform("windows")]
    public UnixFileMode UnixFileMode
    {
        get
        {
#if NET7_0_OR_GREATER
            return this.fileInfo.UnixFileMode;
#else
            if (Env.IsWindows)
                return UnixFileMode.None;

            if (this.unixFileMode is null)
            {
                if (!this.fileInfo.Exists)
                {
                    this.unixFileMode = UnixFileMode.None;
                }
                else
                {
                    Interop.Sys.Stat(this.fileInfo.FullName, out var stat);
                    this.unixFileMode = (UnixFileMode)(stat.Mode & (int)VirtualFileSystem.ValidUnixFileModes);
                }
            }

            return this.unixFileMode.Value;
#endif
        }

        set
        {
#if NET7_0_OR_GREATER
            this.fileInfo.UnixFileMode = value;
#else
            if (Env.IsWindows)
            {
                this.unixFileMode = value;
                return;
            }

            this.unixFileMode = value;
            if (this.fileInfo.Exists)
            {
                Fs.Chmod(this.fileInfo.FullName, (int)this.unixFileMode);
            }
#endif
        }
    }

    public string? DirectoryName => this.fileInfo.DirectoryName;

    public IDirectoryInfo? Directory
    {
        get
        {
            if (this.fileInfo.Directory is null)
                return null;

            return this.directory ??= new VirtualDirectoryInfo(this.fileInfo.Directory);
        }
    }

    public bool IsReadOnly
    {
        get => this.fileInfo.IsReadOnly;
        set => this.fileInfo.IsReadOnly = value;
    }

    public long Length => this.fileInfo.Length;

    public void Delete()
        => this.fileInfo.Delete();

    public void Refresh()
        => this.fileInfo.Refresh();

    public StreamWriter AppendText()
        => this.fileInfo.AppendText();

    public void Create()
        => this.fileInfo.Create();

    public StreamWriter CreateText()
        => this.fileInfo.CreateText();

    [SupportedOSPlatform("windows")]
    public void Decrypt()
        => this.fileInfo.Decrypt();

    [SupportedOSPlatform("windows")]
    public void Encrypt()
        => this.fileInfo.Encrypt();

    public void MoveTo(string destFileName)
        => this.fileInfo.MoveTo(destFileName);

    public FileStream Open(FileMode mode)
        => this.fileInfo.Open(mode);

    public FileStream Open(FileMode mode, FileAccess access)
        => this.fileInfo.Open(mode, access);

    public FileStream Open(FileMode mode, FileAccess access, FileShare share)
        => this.fileInfo.Open(mode, access, share);

    public FileStream OpenRead()
        => this.fileInfo.OpenRead();

    public StreamReader OpenText()
        => this.fileInfo.OpenText();

    public FileStream OpenWrite()
        => this.fileInfo.OpenWrite();

    public void Replace(string destinationFileName, string destinationBackupFileName)
        => this.fileInfo.Replace(destinationFileName, destinationBackupFileName);

    public void Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        => this.fileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
}