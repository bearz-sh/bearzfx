using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Bearz.Virtual;

public partial interface IFileSystem
{
    string CatFiles(params string[] fileNames);

    string CatFiles(bool throwIfFileNotFound, params string[] fileNames);

    void CopyFile(string source, string destination, bool overwrite = false);

    void CopyDirectory(string source, string destination, bool overwrite = false);

    FileStream CreateFile(string path);

    FileStream CreateFile(string path, int bufferSize);

    FileStream CreateFile(string path, int bufferSize, System.IO.FileOptions options);

    StreamWriter CreateTextFile(string path);

    bool FileExists([NotNullWhen(true)] string? path);

    bool DirectoryExists([NotNullWhen(true)] string? path);

    void MakeDirectory(string path);

    void MoveDirectory(string source, string destination);

    void MoveFile(string source, string destination, bool overwrite = false);

    FileStream OpenFile(string path);

    FileStream OpenFile(string path, FileMode mode);

    FileStream OpenFile(string path, FileMode mode, FileAccess access);

    FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share);

    IEnumerable<string> ReadDirectory(string path);

    IEnumerable<string> ReadDirectory(string path, string searchPattern);

    IEnumerable<string> ReadDirectory(string path, string searchPattern, SearchOption searchOption);

    byte[] ReadFile(string path);

    string ReadFileText(string path, Encoding? encoding = null);

    string[] ReadFileLines(string path, Encoding? encoding = null);

    void RemoveFile(string path);

    void RemoveDirectory(string path);

    FileSystemInfo Stat(string path);

    void WriteFile(string path, byte[] data, bool append = false);

    void WriteFileLines(string path, IEnumerable<string> lines, Encoding? encoding = null, bool append = false);

    void WriteFileText(string path, string? contents, Encoding? encoding = null, bool append = false);
}