using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#if NETLEGACY
using Bearz.Extra.Strings;
#endif

// ReSharper disable InconsistentNaming
namespace Bearz.Std;

public static partial class Env
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

    public static string Expand(string template, bool useWindows = true, Func<string, string?>? getVariable = null)
    {
        getVariable ??= (name) => Get(name);
#if NET7_0
        // TODO: replace regular expressions with a proper parser
        if (useWindows)
        {
            template = WindowsVariables().Replace(
                template,
                m =>
                {
                    var key = m.Groups[1].Value;
                    var value = getVariable(key);
                    return value ?? string.Empty;
                });
        }

        template = LinuxEscapedVariables().Replace(
            template,
            m =>
            {
                var key = m.Groups[1].Value;
                var name = key;
                if (key.Contains(":-"))
                {
                    var parts = name.Split(":-");
                    name = parts[0];
                    var defaultValue = parts[1];
                    var value = getVariable(name);

                    if (value == null)
                        return defaultValue;

                    return value;
                }

                if (key.Contains("-"))
                {
                    var parts = name.Split("-");
                    name = parts[0];
                    var defaultValue = parts[1];
                    var value = getVariable(name);

                    if (value == null)
                        return defaultValue;

                    return value;
                }

                if (key.Contains("?"))
                {
                    var parts = name.Split("?");
                    name = parts[0];
                    var message = parts[1];
                    var value = getVariable(name);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        if (string.IsNullOrWhiteSpace(message))
                            message = $"Environment variable '{name}' is not set.";

                        throw new InvalidOperationException(message);
                    }

                    return value;
                }

                return getVariable(name) ?? string.Empty;
            });

        template = LinuxVariables().Replace(
            template,
            m =>
            {
                var key = m.Groups[1].Value;
                var value = getVariable(key);
                return value ?? string.Empty;
            });
#else
        if (useWindows)
        {
            template = new Regex("%([^%]+)%", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(
                template,
                m =>
                {
                    var key = m.Groups[1].Value;
                    var value = getVariable(key);
                    return value ?? string.Empty;
                });
        }

        template = new Regex(@"\$\{([^\}]+)\}", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(
            template,
            m =>
            {
                var key = m.Groups[1].Value;
                var name = key;
                if (key.Contains(":-"))
                {
                    var parts = name.Split(":-");
                    name = parts[0];
                    var defaultValue = parts[1];
                    var value = getVariable(name);

                    if (value == null)
                        return defaultValue;

                    return value;
                }

                if (key.Contains("-"))
                {
                    var parts = name.Split(":-");
                    name = parts[0];
                    var defaultValue = parts[1];
                    var value = getVariable(name);

                    if (value == null)
                        return defaultValue;

                    return value;
                }

                if (key.Contains("?"))
                {
                    var parts = name.Split(":-");
                    name = parts[0];
                    var message = parts[1];
                    var value = getVariable(name);
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        if (string.IsNullOrWhiteSpace(message))
                            message = $"Environment variable '{name}' is not set.";

                        throw new InvalidOperationException(message);
                    }

                    return value;
                }

                return getVariable(name) ?? string.Empty;
            });

        template = new Regex(@"\$([A-Za-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(
            template,
            m =>
            {
                var key = m.Groups[1].Value;
                var value = getVariable(key);
                return value ?? string.Empty;
            });
#endif

        return template;
    }

    public static string? Get(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return Environment.GetEnvironmentVariable(name, target);
    }

    public static IDictionary<string, string> GetAll(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        var variables = Environment.GetEnvironmentVariables(target);
        var result = new Dictionary<string, string>();
        foreach (var key in variables.Keys)
        {
            var name = key as string;
            if (name is null)
                continue;

            var value = variables[key] as string;
            if (value is null)
                continue;

            result.Add(name, value);
        }

        return result;
    }

    public static bool TryGet(string name, out string value)
    {
        value = string.Empty;
        var v = Environment.GetEnvironmentVariable(name);
        if (v == null)
        {
            return false;
        }

        value = v;
        return true;
    }

    public static bool TryGet(string name, EnvironmentVariableTarget target, out string value)
    {
        value = string.Empty;
        var v = Environment.GetEnvironmentVariable(name, target);
        if (v == null)
        {
            return false;
        }

        value = v;
        return true;
    }

    public static void Set(string name, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        Environment.SetEnvironmentVariable(name, value, target);
    }

    public static void Unset(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        Environment.SetEnvironmentVariable(name, null, target);
    }

    public static bool Has(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
    {
        return Environment.GetEnvironmentVariable(name, target) != null;
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
        return Path.TempDir();
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

#if NET7_0
    [GeneratedRegex("%([^%]+)%", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    internal static partial Regex WindowsVariables();

    [GeneratedRegex(@"\$\{([^\}]+)\}", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    internal static partial Regex LinuxEscapedVariables();

    [GeneratedRegex(@"\$([A-Za-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    internal static partial Regex LinuxVariables();
#endif

    private static string? SpecialFolderExtended(SpecialDirectory folder)
    {
        switch (folder)
        {
            case SpecialDirectory.Downloads:
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            case SpecialDirectory.HomeCache:
                if (IsWindows())
                {
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                }

                var cache = Env.Get("XDG_CACHE_HOME") ??
                            Path.Combine(Env.Directory(SpecialDirectory.Home), ".cache");
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