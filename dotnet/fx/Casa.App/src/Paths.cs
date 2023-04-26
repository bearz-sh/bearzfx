using Bearz.Std;
using Bearz.Std.Unix;

namespace Bearz.Casa.App;

public static class Paths
{
    private static string? s_appDirectory;

    private static string? s_userConfigDirectory;

    private static string? s_userDataDirectory;

    private static string? s_globalConfigDirectory;

    private static string? s_globalDataDirectory;

    private static string? s_globalLogsDirectory;

    public static string UserConfigDirectory
    {
        get
        {
            if (s_userConfigDirectory is null)
            {
                if (Env.IsUserElevated && !Env.IsWindows())
                    s_userConfigDirectory = Path.Join("/home", Env.GetRequired("SUDO_USER"), ".config", "casa");
                else
                    s_userConfigDirectory = Env.Directory(SpecialDirectory.ApplicationData, "casa");
            }

            return s_userConfigDirectory;
        }
    }

    public static string UserDataDirectory
    {
        get
        {
            if (s_userDataDirectory is null)
            {
                if (Env.IsUserElevated && !Env.IsWindows())
                    s_userDataDirectory = Path.Join("/home", Env.GetRequired("SUDO_USER"), ".local", "share", "casa");
                else
                    s_userDataDirectory = Env.Directory(SpecialDirectory.LocalApplicationData, "casa");
            }

            return s_userDataDirectory;
        }
    }

    public static string AppDirectory =>
        s_appDirectory ??= Env.Directory(SpecialDirectory.Opt, "casa");

    public static string DataDirectory =>
        s_globalDataDirectory ??= Path.Join(AppDirectory, "data");

    public static string ComposeDirectory =>
        Path.Join(DataDirectory, "compose");

    public static string SharedComposeDataDirectory =>
        Path.Join(DataDirectory, "shared", "compose");

    public static string TemplatesDirectory =>
        Path.Join(AppDirectory, "templates");

    public static string LogsDirectory =>
        s_globalLogsDirectory ??= Path.Join(AppDirectory, "logs");

    public static string ConfigDirectory
    {
        get
        {
            if (s_globalConfigDirectory is null)
            {
                if (Env.IsWindows())
                {
                    s_globalConfigDirectory = Path.Join(AppDirectory, "etc");
                }
                else
                {
                    s_globalConfigDirectory = Path.Join("/etc", "casa");
                }
            }

            return s_globalConfigDirectory;
        }
    }
}