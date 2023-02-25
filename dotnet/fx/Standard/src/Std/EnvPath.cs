namespace Bearz.Std;

public static class EnvPath
{
    private static readonly string Key = Env.IsWindows() ? "Path" : "PATH";

    public static void Add(string path, bool prepend = false, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var paths = Split(target);
        if (InternalHas(path, paths))
            return;

        var current = Get(target);
        if (string.IsNullOrWhiteSpace(current))
        {
            Set(path, target);
            return;
        }

        if (prepend)
        {
            var newPath = $"{path}{Path.PathSeparator}{Get(target)}";
            Set(newPath, target);
        }
        else
        {
            var newPath = $"{Get(target)}{Path.PathSeparator}{path}";
            Set(newPath, target);
        }
    }

    public static string? Get(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        => Environment.GetEnvironmentVariable(Key, target);

    public static string[] Split(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
            => (Get(target) ?? string.Empty).Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

    public static void Set(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
#if NETLEGACY
        Environment.SetEnvironmentVariable(Key, path, target);
#else
        if (OperatingSystem.IsWindows())
        {
            Environment.SetEnvironmentVariable(Key, path, target);
        }
        else
        {
            Environment.SetEnvironmentVariable(Key, path, EnvironmentVariableTarget.Process);
        }
#endif
    }

    public static bool Has(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return InternalHas(path, Split(target));
    }

    public static void Delete(string path, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var paths = Split(target);
        if (!InternalHas(path, paths))
            return;

        var newPath = string.Join(Path.PathSeparator.ToString(), paths.Where(p => !p.Equals(path, StringComparison.OrdinalIgnoreCase)));
        Set(newPath, target);
    }

    private static bool InternalHas(string path, string[] paths)
    {
        if (Env.IsWindows())
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