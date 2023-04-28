using Bearz.Std;
using Bearz.Std.Unix;

using Microsoft.Extensions.Options;

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
                s_userConfigDirectory = Env.GetAppFolder(AppFolder.UserConfig, "casa", true, option: EnvFolderOption.DoNotVerify);
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
                s_userDataDirectory = Env.GetAppFolder(AppFolder.UserLocalShare, "casa", true, option: EnvFolderOption.DoNotVerify);
            }

            return s_userDataDirectory;
        }
    }

    public static string AppDirectory =>
        s_appDirectory ??= Env.GetAppFolder(AppFolder.Opt, "casa");

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
                s_globalConfigDirectory = Env.GetAppFolder(AppFolder.GlobalConfig, "casa", option: EnvFolderOption.DoNotVerify);
            }

            return s_globalConfigDirectory;
        }
    }
}