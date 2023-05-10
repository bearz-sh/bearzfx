using Bearz.Std;

namespace Plank.Package.Actions;

public class PathSpec : IPathSpec
{
    public PathSpec(string rootDir, string? configDir = null, string? volumesDir = null)
    {
        this.PlankRootDir = rootDir;
        volumesDir ??= FsPath.Combine(this.PlankRootDir, "volumes");
        configDir ??= FsPath.Combine(Env.GetAppFolder(AppFolder.UserConfig, "plank", true, option: EnvFolderOption.DoNotVerify));
        this.VolumesDir = volumesDir;
        this.EtcDir = Path.Combine(volumesDir, "etc");
        this.BinDir = Path.Combine(volumesDir, "bin");
        this.DataDir = Path.Combine(volumesDir, "data");
        this.RunDir = Path.Combine(volumesDir,  "run");
        this.LogDir = Path.Combine(volumesDir, "log");
        this.ComposeDir = Path.Combine(rootDir, "compose");
    }

    public string PlankRootDir { get; }

    public string ConfigDir { get; }

    public string VolumesDir { get; }

    public string EtcDir { get; }

    public string BinDir { get; }

    public string DataDir { get; }

    public string RunDir { get; }

    public string LogDir { get; }

    public string ComposeDir { get; }

    public static PathSpec Create(string? rootDir = null, string? volumesDir = null)
    {
        return new PathSpec(rootDir ?? Env.GetAppFolder(AppFolder.GlobalData, "plank"), volumesDir);
    }
}