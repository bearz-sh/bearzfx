namespace Bearz.Virtual;

public interface IFileInfo : IFileSystemInfo
{
    string? DirectoryName { get; }

    IDirectoryInfo? Directory { get; }

    bool IsReadOnly { get; set; }

    long Length { get; }

    StreamWriter AppendText();

    void Create();

    StreamWriter CreateText();

    void Decrypt();

    void Encrypt();

    void MoveTo(string destFileName);

    FileStream Open(FileMode mode);

    FileStream Open(FileMode mode, FileAccess access);

    FileStream Open(FileMode mode, FileAccess access, FileShare share);

    FileStream OpenRead();

    StreamReader OpenText();

    FileStream OpenWrite();

    void Replace(string destinationFileName, string destinationBackupFileName);

    void Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);
}