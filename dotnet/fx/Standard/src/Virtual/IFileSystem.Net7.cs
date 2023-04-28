#if NET7_0_OR_GREATER
using Microsoft.Win32.SafeHandles;

namespace Bearz.Virtual;

public partial interface IFileSystem
{
    UnixFileMode GetUnixFileMode(Microsoft.Win32.SafeHandles.SafeFileHandle fileHandle);

    void MakeDirectory(string path, UnixFileMode mode);

    void SetUnixFileMode(string path, UnixFileMode mode);

    void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode);
}

#endif