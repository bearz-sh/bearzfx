#if NETSTANDARD2_0 || NET6_0_OR_GREATER
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#if NET7_0_OR_GREATER

#elif NET6_0 || NETSTANDARD2_0

using LibraryImport = System.Runtime.InteropServices.DllImportAttribute;
#endif

namespace Bearz.Std.Unix;

[SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1300:Element should begin with upper-case letter")]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:Tuple element names should use correct casing")]
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

    [CLSCompliant(false)]
    public static unsafe (uint? userId, uint? groupId) GetUserAndGroupIds(string userName)
    {
        Interop.Sys.Passwd? passwd;

        // First try with a buffer that should suffice for 99% of cases.
        // Note: on CentOS/RedHat 7.1 systems, getpwnam_r returns 'user not found' if the buffer is too small
        // see https://bugs.centos.org/view.php?id=7324
        const int BufLen = Interop.Sys.Passwd.InitialBufferSize;
        byte* stackBuf = stackalloc byte[BufLen];
        if (TryGetPasswd(userName, stackBuf, BufLen, out passwd))
        {
            if (passwd == null)
            {
                return (null, null);
            }

            return (passwd.Value.UserId, passwd.Value.GroupId);
        }

        // Fallback to heap allocations if necessary, growing the buffer until
        // we succeed.  TryGetPasswd will throw if there's an unexpected error.
        int lastBufLen = BufLen;
        while (true)
        {
            lastBufLen *= 2;
            byte[] heapBuf = new byte[lastBufLen];
            fixed (byte* buf = &heapBuf[0])
            {
                if (TryGetPasswd(userName, buf, heapBuf.Length, out passwd))
                {
                    if (passwd == null)
                    {
                        return (null, null);
                    }

                    return (passwd.Value.UserId, passwd.Value.GroupId);
                }
            }
        }
    }

    private static unsafe bool TryGetPasswd(string name, byte* buf, int bufLen, out Interop.Sys.Passwd? passwd)
    {
        // Call getpwnam_r to get the passwd struct
        Interop.Sys.Passwd tempPasswd;
        int error = Interop.Sys.GetPwNamR(name, out tempPasswd, buf, bufLen);

        // If the call succeeds, give back the passwd retrieved
        if (error == 0)
        {
            Console.WriteLine("success with GetPwNamR");
            passwd = tempPasswd;
            return true;
        }

        // If the current user's entry could not be found, give back null,
        // but still return true as false indicates the buffer was too small.
        if (error == -1)
        {
            Console.WriteLine("errorwith GetPwNamR");
            passwd = null;
            return true;
        }

        var errorInfo = new Interop.ErrorInfo(error);

        // If the call failed because the buffer was too small, return false to
        // indicate the caller should try again with a larger buffer.
        if (errorInfo.Error == Interop.Error.ERANGE)
        {
            passwd = null;
            return false;
        }

        // Otherwise, fail.
        throw new Win32Exception(errorInfo.RawErrno, errorInfo.GetErrorMessage());
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