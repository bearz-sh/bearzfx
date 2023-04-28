#if NET7_0_OR_GREATER
using Microsoft.Win32.SafeHandles;

namespace Bearz.Virtual;

// ReSharper disable ParameterHidesMember
public partial class VirtualFileSystem
{
    public UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle)
        => File.GetUnixFileMode(fileHandle);

    public void MakeDirectory(string path, UnixFileMode mode)
    {
        path = this.path.Resolve(path);
        Directory.CreateDirectory(path, mode);
    }

    public void SetUnixFileMode(string path, UnixFileMode mode)
    {
        path = this.path.Resolve(path);
        File.SetUnixFileMode(path, mode);
    }

    public void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode)
        => File.SetUnixFileMode(fileHandle, mode);
}

#endif