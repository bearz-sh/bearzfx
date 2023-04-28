using System.Diagnostics.CodeAnalysis;
using System.Text;

using Bearz.Extra.Strings;
using Bearz.Text;

// ReSharper disable ParameterHidesMember
namespace Bearz.Virtual;

// ReSharper disable once PartialTypeWithSinglePart
public partial class VirtualFileSystem : IFileSystem
{
    private readonly IPath path;

    public VirtualFileSystem(IPath path)
    {
        this.path = path;
    }

    public string CatFiles(params string[] fileNames)
        => this.CatFiles(false, fileNames);

    public string CatFiles(bool throwIfFileNotFound, params string[] fileNames)
        => this.CatFiles(false, null, fileNames);

    public string CatFiles(bool throwIfFileNotFound, Encoding? encoding, params string[] fileNames)
    {
        var sb = StringBuilderCache.Acquire();
        foreach (var fileName in fileNames)
        {
            var next = this.path.Resolve(fileName);
            if (!this.FileExists(next))
            {
                if (throwIfFileNotFound)
                    throw new FileNotFoundException(next);

                continue;
            }

            sb.Append(this.ReadFileText(next, encoding));
        }

        return StringBuilderCache.GetStringAndRelease(sb);
    }

    public void CopyFile(string source, string destination, bool overwrite = false)
    {
        source = this.path.Resolve(source);
        destination = this.path.Resolve(destination);
        File.Copy(source, destination, overwrite);
    }

    public void CopyDirectory(string source, string destination, bool overwrite = false)
    {
        throw new NotImplementedException();
    }

    public FileStream CreateFile(string path)
    {
        path = this.path.Resolve(path);
        return File.Create(path);
    }

    public FileStream CreateFile(string path, int bufferSize)
    {
        path = this.path.Resolve(path);
        return File.Create(path, bufferSize);
    }

    public FileStream CreateFile(string path, int bufferSize, FileOptions options)
    {
        path = this.path.Resolve(path);
        return File.Create(path, bufferSize, options);
    }

    public StreamWriter CreateTextFile(string path)
    {
        path = this.path.Resolve(path);
        return File.CreateText(path);
    }

    public bool FileExists([NotNullWhen(true)] string? path)
    {
        if (path.IsNullOrEmpty())
            return false;

        path = this.path.Resolve(path);
        return File.Exists(path);
    }

    public bool DirectoryExists([NotNullWhen(true)] string? path)
    {
        if (path.IsNullOrEmpty())
            return false;

        path = this.path.Resolve(path);
        return Directory.Exists(path);
    }

    public IEnumerable<string> EnumerateFiles(string path)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFiles(path);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFiles(path, searchPattern);
    }

    public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFiles(path, searchPattern, searchOption);
    }

    public IEnumerable<string> EnumerateDirectories(string path)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateDirectories(path);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateDirectories(path, searchPattern);
    }

    public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateDirectories(path, searchPattern, searchOption);
    }

    public bool IsDirectory(string path)
        => File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    public void MakeDirectory(string path)
    {
        path = this.path.Resolve(path);
        Directory.CreateDirectory(path);
    }

    public void MoveDirectory(string source, string destination)
    {
        source = this.path.Resolve(source);
        destination = this.path.Resolve(destination);
        Directory.Move(source, destination);
    }

    public void MoveFile(string source, string destination, bool overwrite = false)
    {
        source = this.path.Resolve(source);
        destination = this.path.Resolve(destination);
#if NETSTANDARD2_0
        File.Move(source, destination);
#else
        File.Move(source, destination, overwrite);
#endif
    }

    public FileStream OpenFile(string path)
    {
        path = this.path.Resolve(path);
        return File.OpenRead(path);
    }

    public FileStream OpenFile(string path, FileMode mode)
    {
        path = this.path.Resolve(path);
        return File.Open(path, mode);
    }

    public FileStream OpenFile(string path, FileMode mode, FileAccess access)
    {
        path = this.path.Resolve(path);
        return File.Open(path, mode, access);
    }

    public FileStream OpenFile(string path, FileMode mode, FileAccess access, FileShare share)
    {
        path = this.path.Resolve(path);
        return File.Open(path, mode, access, share);
    }

    public IEnumerable<string> ReadDirectory(string path)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFileSystemEntries(path);
    }

    public IEnumerable<string> ReadDirectory(string path, string searchPattern)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFileSystemEntries(path, searchPattern);
    }

    public IEnumerable<string> ReadDirectory(string path, string searchPattern, SearchOption searchOption)
    {
        path = this.path.Resolve(path);
        return Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
    }

    public string[] ReadDriveNames()
        => Directory.GetLogicalDrives();

    public byte[] ReadFile(string path)
    {
        path = this.path.Resolve(path);
        return File.ReadAllBytes(path);
    }

    public FileAttributes ReadFileAttributes(string path)
    {
        path = this.path.Resolve(path);
        return File.GetAttributes(path);
    }

    public string[] ReadFileLines(string path, Encoding? encoding = null)
    {
        path = this.path.Resolve(path);
        return encoding is null ? File.ReadAllLines(path) : File.ReadAllLines(path, encoding);
    }

    public string ReadFileText(string path, Encoding? encoding = null)
    {
        path = this.path.Resolve(path);
        return encoding is null ? File.ReadAllText(path) : File.ReadAllText(path, encoding);
    }

    public void RemoveFile(string path)
    {
        path = this.path.Resolve(path);
        File.Delete(path);
    }

    public void RemoveDirectory(string path)
    {
        path = this.path.Resolve(path);
        Directory.Delete(path);
    }

    public string ResolvePath(string path)
        => this.path.Resolve(path);

    public FileSystemInfo Stat(string path)
    {
        path = this.path.Resolve(path);
        return this.IsDirectory(path) ? new DirectoryInfo(path) : new FileInfo(path);
    }

    public void WriteFile(string path, byte[] data, bool append = false)
        => File.WriteAllBytes(path, data);

    public void WriteFileLines(string path, IEnumerable<string> lines, Encoding? encoding = null, bool append = false)
    {
        path = this.path.Resolve(path);

        if (append)
        {
            if (encoding is null)
                File.AppendAllLines(path, lines);
            else
                File.AppendAllLines(path, lines, encoding);

            return;
        }

        if (encoding is null)
            File.WriteAllLines(path, lines);
        else
            File.WriteAllLines(path, lines, encoding);
    }

    public void WriteFileText(string path, string? contents, Encoding? encoding = null, bool append = false)
    {
        path = this.path.Resolve(path);

        if (append)
        {
            if (encoding is null)
                File.AppendAllText(path, contents);
            else
                File.AppendAllText(path, contents, encoding);

            return;
        }

        if (encoding is null)
            File.WriteAllText(path, contents);
        else
            File.WriteAllText(path, contents, encoding);
    }
}