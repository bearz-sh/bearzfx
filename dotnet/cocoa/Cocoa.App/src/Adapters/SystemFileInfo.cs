namespace Cocoa.Adapters;

public class SystemFileInfo : IFileInfo
{
    private readonly FileInfo fileInfo;

    private IDirectoryInfo? directory;

    public SystemFileInfo(string fileName)
    {
        this.fileInfo = new FileInfo(fileName);
    }

    public SystemFileInfo(FileInfo fileInfo)
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

    public IDirectoryInfo? Directory
    {
        get
        {
            if (this.fileInfo.Directory is null)
                return null;

            return this.directory ??= new SystemDirectoryInfo(this.fileInfo.Directory);
        }
    }

    public string? DirectoryName => this.fileInfo.DirectoryName;

    public bool IsReadOnly
    {
        get => this.fileInfo.IsReadOnly;
        set => this.fileInfo.IsReadOnly = value;
    }

    public long Length
    {
        get => this.fileInfo.Length;
    }

    public FileStream Create()
        => this.fileInfo.Create();

    public void Decrypt()
        => this.fileInfo.Decrypt();

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

    public void Delete()
        => this.fileInfo.Delete();

    public void CopyTo(string destFileName)
        => this.fileInfo.CopyTo(destFileName);

    public void CopyTo(string destFileName, bool overwrite)
        => this.fileInfo.CopyTo(destFileName, overwrite);

    public void Refresh()
        => this.fileInfo.Refresh();
}