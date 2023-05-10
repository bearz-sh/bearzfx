namespace Bearz.Std;

#if STD
public
#else
internal
#endif
static partial class Env
{
    private static readonly string Key = Env.IsWindows ? "Path" : "PATH";

    public static void AddPath(string path, bool prepend = false, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var paths = SplitPath(target);
        if (InternalHasPath(path, paths))
            return;

        var current = GetPath(target);
        if (string.IsNullOrWhiteSpace(current))
        {
            SetPath(path, target);
            return;
        }

        if (prepend)
        {
            var newPath = $"{path}{FsPath.PathSeparator}{GetPath(target)}";
            SetPath(newPath, target);
        }
        else
        {
            var newPath = $"{GetPath(target)}{FsPath.PathSeparator}{path}";
            SetPath(newPath, target);
        }
    }

    public static string? GetPath(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        => Environment.GetEnvironmentVariable(Key, target);

    public static string[] SplitPath(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
            => (GetPath(target) ?? string.Empty).Split(new[] { FsPath.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

    public static void SetPath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
#if NETLEGACY
        Environment.SetEnvironmentVariable(Key, path, target);
#else
        Environment.SetEnvironmentVariable(Key, path, IsWindows ? target : EnvironmentVariableTarget.Process);
#endif
    }

    public static bool HasPath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return InternalHasPath(path, SplitPath(target));
    }

    public static void RemovePath(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var paths = SplitPath(target);
        if (!InternalHasPath(path, paths))
            return;

        var newPath = string.Join(FsPath.PathSeparator.ToString(), paths.Where(p => !p.Equals(path, StringComparison.OrdinalIgnoreCase)));
        SetPath(newPath, target);
    }

    private static bool InternalHasPath(string path, string[] paths)
    {
        if (Env.IsWindows)
        {
            foreach (var p in paths)
            {
                if (p.Equals(path, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        else
        {
            foreach (var p in paths)
            {
                if (p.Equals(path, StringComparison.Ordinal))
                    return true;
            }
        }

        return false;
    }
}