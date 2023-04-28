using System.Diagnostics;
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
}