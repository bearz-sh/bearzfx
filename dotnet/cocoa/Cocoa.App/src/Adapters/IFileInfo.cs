namespace Cocoa.Adapters;

public interface IFileInfo : IFileSystemInfo
{
    IDirectoryInfo? Directory { get;  }

    string? DirectoryName { get;  }

    bool IsReadOnly { get; set; }

    long Length { get;  }

    FileStream Create();

    void Decrypt();

    void Encrypt();

    FileStream Open(FileMode mode);

    FileStream Open(FileMode mode, FileAccess access);

    FileStream Open(FileMode mode, FileAccess access, FileShare share);

    FileStream OpenRead();

    StreamReader OpenText();

    FileStream OpenWrite();

    void Replace(string destinationFileName, string destinationBackupFileName);

    void Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);

    void MoveTo(string destFileName);

    void CopyTo(string destFileName);

    void CopyTo(string destFileName, bool overwrite);
}