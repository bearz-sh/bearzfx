using System.Runtime.InteropServices;

namespace Bearz.Std;

#if STD
public
#else
internal
#endif
    static partial class Env
{
    private static string? s_homeDirectory = null;

    private static Lazy<bool> s_isMono = new Lazy<bool>(() => Type.GetType("Mono.Runtime") != null);

    private static Lazy<bool> s_isWindows = new Lazy<bool>(GetIsWindows);

    private static Lazy<bool> s_isMacOSX = new Lazy<bool>(() => GetIsMacOSX());

    private static Lazy<bool> s_isLinux = new Lazy<bool>(GetIsLinux);

    public static bool IsUserElevated => IsWindows ? Win32.Win32User.IsAdmin : Unix.UnixUser.IsRoot;

    public static string HomeDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static string SudoUserHomeDirectory
    {
        get
        {
            if (s_homeDirectory is not null)
                return s_homeDirectory;

            if (!IsUserElevated || IsWindows)
            {
                s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return s_homeDirectory;
            }

            if (IsLinux)
            {
                s_homeDirectory = $"/home/{Env.GetRequired("SUDO_USER")}";
                return s_homeDirectory;
            }

            if (IsMacOS)
            {
                s_homeDirectory = $"/Users/{Env.GetRequired("SUDO_USER")}";
                return s_homeDirectory;
            }

            s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return s_homeDirectory;
        }
    }

    public static bool IsMono => s_isMono.Value;

    public static bool IsWindows => s_isWindows.Value;

    public static bool IsLinux => s_isLinux.Value;

    public static bool IsMacOS => s_isMacOSX.Value;

    private static bool GetIsLinux()
    {
#if !NETLEGACY
            // This API does work on full framework but it requires a newer nuget client (RID aware)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return true;
            }

            // The OSPlatform.FreeBSD property only exists in .NET Core 3.1 and higher, whereas this project is also
            // compiled for .NET Standard and .NET Framework, where an OSPlatform for FreeBSD must be created manually
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD")))
            {
                return true;
            }

            return false;
#else
        var platform = (int)Environment.OSVersion.Platform;
        return platform == 4;
#endif
    }

    private static bool GetIsMacOSX()
    {
#if !NETLEGACY
            // This API does work on full framework but it requires a newer nuget client (RID aware)
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                return true;
            }

            return false;
#else
        var buf = IntPtr.Zero;

        try
        {
            buf = Marshal.AllocHGlobal(8192);

            // This is a hacktastic way of getting sysname from uname ()
            if (Uname(buf) == 0)
            {
                var os = Marshal.PtrToStringAnsi(buf);

                if (os == "Darwin")
                {
                    return true;
                }
            }
        }
        catch
        {
            // eating the exception because if it fails we just assume it isn't a mac
        }
        finally
        {
            if (buf != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(buf);
            }
        }

        return false;
#endif
    }

    private static bool GetIsWindows()
    {
#if !NETLEGACY
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        var platform = (int)Environment.OSVersion.Platform;
        return (platform != 4) && (platform != 6) && (platform != 128);
#endif
    }

    [DllImport("libc", EntryPoint = "uname")]
    private static extern int Uname(IntPtr buf);
}