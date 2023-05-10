namespace Bearz.Std;

#if STD
public
#else
internal
#endif
static partial class Env
{
    public static string GetAppFolder(AppFolder folder, string applicationName, bool sudoUserHomeDirectory = false, EnvFolderOption option = EnvFolderOption.None)
    {
        var dir = GetAppFolderInternal(folder, applicationName, sudoUserHomeDirectory);
        switch (option)
        {
            case EnvFolderOption.DoNotVerify:
                return dir;

            case EnvFolderOption.None:
                if (Fs.DirectoryExists(dir))
                    return dir;

                return string.Empty;

            case EnvFolderOption.Create:
                if (!Fs.DirectoryExists(dir))
                    Fs.MakeDirectory(dir);

                return dir;

            default:
                throw new NotSupportedException();
        }
    }

    private static string GetAppFolderInternal(AppFolder folder, string applicationName, bool sudoUserHomeDirectory)
    {
        switch (folder)
        {
            case AppFolder.Opt:
                {
                    if (IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName);
                    }

                    return $"/opt/{applicationName}";
                }

            case AppFolder.GlobalBin:
                {
                    if (IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        return Path.Combine(common, applicationName, "bin");
                    }

                    return "/usr/bin";
                }

            case AppFolder.GlobalLocalBin:
                {
                    if (IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                        return Path.Combine(common, "bin");
                    }

                    return "/usr/local/bin";
                }

            case AppFolder.UserLocalBin:
                {
                    if (sudoUserHomeDirectory)
                    {
                        var home = SudoUserHomeDirectory;
                        return Path.Combine(home, ".local", "bin");
                    }

                    var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    return Path.Combine(local, "bin");
                }

            case AppFolder.GlobalLocalShare:
                {
                    if (IsWindows)
                    {
                        var local = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                        return Path.Combine(local, applicationName);
                    }

                    return $"/usr/local/share/{applicationName}";
                }

            case AppFolder.UserLocalShare:
                {
                    if (sudoUserHomeDirectory)
                    {
                        var home = SudoUserHomeDirectory;
                        return Path.Combine(home, ".local", "share", applicationName);
                    }

                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        applicationName);
                }

            case AppFolder.GlobalCache:
                {
                    if (IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "cache");
                    }

                    return $"/var/cache/{applicationName}";
                }

            case AppFolder.UserCache:
                {
                    if (IsWindows)
                    {
                        var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        return Path.Combine(local, applicationName, "cache");
                    }

                    var home = sudoUserHomeDirectory ? SudoUserHomeDirectory : HomeDirectory;
                    return Path.Combine(home, ".cache", applicationName);
                }

            case AppFolder.OptCache:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "cache");
                    }

                    return $"/opt/{applicationName}/cache";
                }

            case AppFolder.GlobalConfig:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "etc");
                    }

                    return $"/etc/{applicationName}";
                }

            case AppFolder.UserConfig:
                {
                    if (!sudoUserHomeDirectory || IsWindows)
                    {
                        return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        applicationName);
                    }

                    return Path.Combine(SudoUserHomeDirectory, ".config", applicationName);
                }

            case AppFolder.OptConfig:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "etc");
                    }

                    return $"/opt/{applicationName}/etc";
                }

            case AppFolder.GlobalLogs:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "log");
                    }

                    return $"/var/log/{applicationName}";
                }

            case AppFolder.UserLogs:
                {
                    if (!sudoUserHomeDirectory || IsWindows)
                    {
                        return Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            applicationName,
                            "logs");
                    }

                    return Path.Combine(SudoUserHomeDirectory, ".local", "share", applicationName, "logs");
                }

            case AppFolder.OptLogs:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "log");
                    }

                    return $"/opt/{applicationName}/log";
                }

            case AppFolder.GlobalTemp:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, "Temp", applicationName);
                    }

                    return $"/tmp/{applicationName}";
                }

            case AppFolder.UserTemp:
                {
                    if (Env.IsWindows)
                        return Path.Combine(Path.GetTempPath(), applicationName);

                    var home = sudoUserHomeDirectory ? SudoUserHomeDirectory : HomeDirectory;
                    return $"{home}/.cache/{applicationName}/tmp";
                }

            case AppFolder.OptTemp:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, "Temp", applicationName);
                    }

                    return $"/opt/{applicationName}/tmp";
                }

            case AppFolder.GlobalData:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "data");
                    }

                    return $"/var/lib/{applicationName}/data";
                }

            case AppFolder.UserData:
                {
                    var common = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    return Path.Combine(common, applicationName, "data");
                }

            case AppFolder.OptData:
                {
                    if (Env.IsWindows)
                    {
                        var common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        return Path.Combine(common, applicationName, "data");
                    }

                    return $"/opt/{applicationName}/data";
                }

            case AppFolder.UserDocuments:
                {
                    if (!sudoUserHomeDirectory || IsWindows)
                    {
                        return Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            applicationName);
                    }

                    return Path.Combine(SudoUserHomeDirectory, "Documents", applicationName);
                }

            case AppFolder.UserDownloads:
                {
                    var home = sudoUserHomeDirectory ? SudoUserHomeDirectory : HomeDirectory;
                    return Path.Combine(home, "Downloads", applicationName);
                }

            case AppFolder.UserMusic:
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                        applicationName);
                }

            case AppFolder.UserPictures:
                {
                    if (!sudoUserHomeDirectory || IsWindows)
                    {
                        return Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                            applicationName);
                    }

                    return Path.Combine(SudoUserHomeDirectory, "Pictures", applicationName);
                }

            case AppFolder.UserVideos:
                {
                    if (!sudoUserHomeDirectory || IsWindows)
                    {
                        return Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                            applicationName);
                    }

                    return Path.Combine(SudoUserHomeDirectory, "Videos", applicationName);
                }

            default:
                {
                    throw new ArgumentOutOfRangeException(nameof(folder), folder, null);
                }
        }
    }
}

#pragma warning disable SA1201 // keep enum and class in same file
public enum AppFolder
{
    Opt = 3000,

    GlobalBin = 1001,

    UserLocalBin = 1002,

    GlobalLocalBin = 1003,

    GlobalLocalShare = 1010,

    UserLocalShare = 1011,

    GlobalCache = 1020,

    UserCache = 1021,

    OptCache = 1022,

    UserConfig = 1031,

    GlobalConfig = 1030,

    OptConfig = 1032,

    UserLogs = 1041,

    GlobalLogs = 1040,

    OptLogs = 1042,

    UserTemp = 1051,

    GlobalTemp = 1050,

    OptTemp = 1052,

    UserData = 1061,

    GlobalData = 1060,

    OptData = 1062,

    UserDocuments = 9000,

    UserDownloads = 9001,

    UserMusic = 9002,

    UserPictures = 9003,

    UserVideos = 9004,
}