using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace Bearz;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
        [DllImport(Libraries.Kernel32)]
        internal static extern int GetLastError();
    }
}