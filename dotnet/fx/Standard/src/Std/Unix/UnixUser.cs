#if NETSTANDARD2_0 || NET6_0_OR_GREATER
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
#if NET7_0_OR_GREATER

#elif NET6_0 || NETSTANDARD2_0

using LibraryImport = System.Runtime.InteropServices.DllImportAttribute;
#endif

namespace Bearz.Std.Unix;

[SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1300:Element should begin with upper-case letter")]
public static partial class UnixUser
{
    private const string Libc = "libc";

    public static bool IsRoot => EffectiveUserId is 0;

    public static int? UserId
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return null;

            return (int)getuid();
        }
    }

    public static int? EffectiveUserId
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return null;

            return (int)getuid();
        }
    }

    [DllImport(Libc, SetLastError = true)]
    private static extern uint getuid();

    [DllImport(Libc, SetLastError = true)]
    private static extern uint geteuid();

    [DllImport(Libc, SetLastError = true)]
    private static extern uint getgid();

    [DllImport(Libc, SetLastError = true)]
    private static extern uint getegid();
}

#endif