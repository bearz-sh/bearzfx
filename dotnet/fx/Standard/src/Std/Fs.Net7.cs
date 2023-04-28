using Microsoft.Win32.SafeHandles;

#if NET7_0_OR_GREATER

namespace Bearz.Std;

public static partial class Fs
{
    public static UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle)
        => File.GetUnixFileMode(fileHandle);

    public static void MakeDirectory(string path, UnixFileMode mode)
    {
        Directory.CreateDirectory(path, mode);
    }

    public static void SetUnixFileMode(string path, UnixFileMode mode)
    {
        File.SetUnixFileMode(path, mode);
    }

    public static void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode)
        => File.SetUnixFileMode(fileHandle, mode);
}

#endif