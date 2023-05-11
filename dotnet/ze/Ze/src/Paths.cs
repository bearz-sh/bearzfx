using Bearz.Std;

namespace Ze;

public static class Paths
{
    private static string? s_appDirectory;

    private static string? s_userConfigDirectory;

    private static string? s_userDataDirectory;

    private static string? s_globalConfigDirectory;

    public static string UserConfigDirectory
    {
        get
        {
            if (s_userConfigDirectory is null)
            {
                s_userConfigDirectory = Env.GetAppFolder(AppFolder.UserConfig, "plank", true, option: EnvFolderOption.DoNotVerify);
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
                s_userDataDirectory = Env.GetAppFolder(AppFolder.UserLocalShare, "plank", true, option: EnvFolderOption.DoNotVerify);
            }

            return s_userDataDirectory;
        }
    }

    public static string AppDirectory =>
        s_appDirectory ??= Env.GetAppFolder(AppFolder.Opt, "plank", true, option: EnvFolderOption.DoNotVerify);

    public static string ConfigDirectory
    {
        get
        {
            if (s_globalConfigDirectory is null)
            {
                s_globalConfigDirectory = Env.GetAppFolder(AppFolder.GlobalConfig, "plank", option: EnvFolderOption.DoNotVerify);
            }

            return s_globalConfigDirectory;
        }
    }
}