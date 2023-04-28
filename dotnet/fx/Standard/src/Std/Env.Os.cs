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

    public static bool IsUserElevated => IsWindows() ? Win32.Win32User.IsAdmin : Unix.UnixUser.IsRoot;

    public static string HomeDirectory { get; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public static string SudoUserHomeDirectory
    {
        get
        {
            if (s_homeDirectory is not null)
                return s_homeDirectory;

            if (!IsUserElevated || IsWindows())
            {
                s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return s_homeDirectory;
            }

            if (IsLinux())
            {
                s_homeDirectory = $"/home/{Env.GetRequired("SUDO_USER")}";
                return s_homeDirectory;
            }

            if (IsMacOS())
            {
                s_homeDirectory = $"/Users/{Env.GetRequired("SUDO_USER")}";
                return s_homeDirectory;
            }

            s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return s_homeDirectory;
        }
    }

    public static bool IsWindows()
    {
#if NETLEGACY
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        return OperatingSystem.IsWindows();
#endif
    }

    public static bool IsLinux()
    {
#if NETLEGACY
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

#else
        return OperatingSystem.IsLinux();
#endif
    }

    public static bool IsMacOS()
    {
#if NETLEGACY
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
        return OperatingSystem.IsMacOS();
#endif
    }
}