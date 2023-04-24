using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#if NETLEGACY
using Bearz.Extra.Strings;
#endif

// ReSharper disable InconsistentNaming
namespace Bearz.Std;

#if STD
public
#else
internal
#endif
static partial class Env
{
    private static bool? s_userInteractive;

    public static IReadOnlyList<string> Keys
    {
#pragma warning disable S2365
        get => Environment.GetEnvironmentVariables().Keys.Cast<string>().ToList();
#pragma warning restore S2365
    }

    public static string Cwd
    {
        get => Environment.CurrentDirectory;
        set => Environment.CurrentDirectory = value;
    }

    public static string User => Environment.UserName;

    public static string UserDomain => Environment.UserDomainName;

    public static string HostName => Environment.MachineName;

    public static Architecture OsArch => RuntimeInformation.OSArchitecture;

    public static Architecture ProcessArch => RuntimeInformation.ProcessArchitecture;

    public static bool IsProcess64Bit => ProcessArch is Architecture.X64 or Architecture.Arm64;

    public static bool IsOs64Bit => OsArch is Architecture.X64 or Architecture.Arm64;

    public static bool IsUserInteractive
    {
        get
        {
            if (s_userInteractive.HasValue)
                return s_userInteractive.Value;

            s_userInteractive = Environment.UserInteractive;
            return s_userInteractive.Value;
        }

        set => s_userInteractive = value;
    }

    public static bool IsUserElevated => IsWindows() ? Win32.Win32User.IsAdmin : Unix.UnixUser.IsRoot;

    public static void Unset(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        Environment.SetEnvironmentVariable(name, null, target);
    }

    public static string Directory(string directoryName)
    {
        if (!Enum.TryParse<SpecialDirectory>(directoryName, true, out var folder))
        {
            throw new InvalidCastException($"Unable to cast '{directoryName}' to {nameof(SpecialDirectory)}");
        }

        return Directory(folder);
    }

    public static string Directory(SpecialDirectory directory)
    {
        var path = SpecialFolderExtended(directory);
        return path ?? Environment.GetFolderPath((Environment.SpecialFolder)directory);
    }

    public static string Directory(SpecialDirectory directory, Environment.SpecialFolderOption option)
    {
        var path = SpecialFolderExtended(directory);
        return path ?? Environment.GetFolderPath((Environment.SpecialFolder)directory, option);
    }

    public static string HomeDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    public static string TempDir()
    {
        return FsPath.TempDir();
    }

    public static string HomeConfigDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }

    public static string HomeDataDir()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
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

    private static string? SpecialFolderExtended(SpecialDirectory folder)
    {
        switch (folder)
        {
            case SpecialDirectory.Downloads:
                return FsPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            case SpecialDirectory.HomeCache:
                if (IsWindows())
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }

                var cache = Env.Get("XDG_CACHE_HOME") ??
                            FsPath.Combine(Env.Directory(SpecialDirectory.Home), ".cache");
                return cache;

            case SpecialDirectory.Mnt:
                return IsWindows() ? string.Empty : "/mnt";

            case SpecialDirectory.Null:
                return IsWindows() ? "NUL" : "/dev/null";

            case SpecialDirectory.Cache:
                return IsWindows() ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) : "/var/cache";

            case SpecialDirectory.Opt:
                return IsWindows() ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) : "/opt";

            case SpecialDirectory.Etc:
                if (IsWindows())
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                }

                return "/etc";
            default:
                return null;
        }
    }
}