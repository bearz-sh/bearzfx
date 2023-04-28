using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;

using Bearz.Diagnostics;

namespace Bearz.Std;

public static class CommandStartInfoExtensions
{
    public static CommandStartInfo WithStdio(this CommandStartInfo startInfo, Stdio stdio)
    {
        startInfo.StdIn = stdio;
        startInfo.StdOut = stdio;
        startInfo.StdErr = stdio;
        return startInfo;
    }

    public static CommandStartInfo WithStdIn(this CommandStartInfo startInfo, Stdio stdio)
    {
        startInfo.StdIn = stdio;
        return startInfo;
    }

    public static CommandStartInfo WithStdOut(this CommandStartInfo startInfo, Stdio stdio)
    {
        startInfo.StdOut = stdio;
        return startInfo;
    }

    public static CommandStartInfo WithStdErr(this CommandStartInfo startInfo, Stdio stdio)
    {
        startInfo.StdErr = stdio;
        return startInfo;
    }

    public static CommandStartInfo WithArgs(this CommandStartInfo startInfo, params string[] args)
    {
        startInfo.Args = args;
        return startInfo;
    }

    public static CommandStartInfo WithArgs(this CommandStartInfo startInfo, IEnumerable<string> args)
    {
        startInfo.Args = new CommandArgs(args);
        return startInfo;
    }

    public static CommandStartInfo WithArgs(this CommandStartInfo startInfo, CommandArgs args)
    {
        startInfo.Args = args;
        return startInfo;
    }

    public static CommandStartInfo AsUser(this CommandStartInfo startInfo, string user)
    {
        startInfo.User = user;
        return startInfo;
    }

    [SupportedOSPlatform("windows")]
    public static CommandStartInfo AsUser(this CommandStartInfo startInfo, string user, string password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        startInfo.User = user;
        startInfo.PasswordInClearText = password;
        return startInfo;
    }

    [SupportedOSPlatform("windows")]
    public static CommandStartInfo AsUser(this CommandStartInfo startInfo, string user, string password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        startInfo.User = user;
        startInfo.PasswordInClearText = password;
        startInfo.Domain = domain;
        return startInfo;
    }

    [SupportedOSPlatform("windows")]
    public static CommandStartInfo AsUser(this CommandStartInfo startInfo, string user, SecureString password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        startInfo.User = user;
        startInfo.Password = password;
        return startInfo;
    }

    [SupportedOSPlatform("windows")]
    public static CommandStartInfo AsUser(this CommandStartInfo startInfo, string user, SecureString password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        startInfo.User = user;
        startInfo.Password = password;
        startInfo.Domain = domain;
        return startInfo;
    }

    [SupportedOSPlatform("windows")]
    public static unsafe CommandStartInfo AsUser(this CommandStartInfo startInfo, string user, ReadOnlySpan<char> password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        startInfo.User = user;
        fixed (char* pChars = &password.GetPinnableReference())
        {
            var ss = new SecureString(pChars, password.Length);
            startInfo.Password = ss;
        }

        startInfo.Domain = domain;
        return startInfo;
    }

    public static CommandStartInfo AddArg(this CommandStartInfo startInfo, string arg)
    {
        startInfo.Args.Add(arg);
        return startInfo;
    }

    public static CommandStartInfo AsSudo(this CommandStartInfo startInfo)
    {
        startInfo.Verb = "sudo";
        return startInfo;
    }

    public static CommandStartInfo AsAdmin(this CommandStartInfo startInfo)
    {
        startInfo.Verb = "runas";
        return startInfo;
    }

    public static CommandStartInfo AsOsAdmin(this CommandStartInfo startInfo)
    {
        startInfo.Verb = "admin";
        return startInfo;
    }

    public static CommandStartInfo WithVerb(this CommandStartInfo startInfo, string verb)
    {
        startInfo.Verb = verb;
        return startInfo;
    }

    public static CommandStartInfo AddEnv(this CommandStartInfo startInfo, string key, string? value)
    {
        startInfo.Env ??= new Dictionary<string, string?>();
        startInfo.Env[key] = value;
        return startInfo;
    }

    public static CommandStartInfo WithEnv(this CommandStartInfo startInfo, IDictionary<string, string?> env)
    {
        startInfo.Env ??= new Dictionary<string, string?>();
        foreach (var kvp in env)
        {
            startInfo.Env[kvp.Key] = kvp.Value;
        }

        return startInfo;
    }

    public static CommandStartInfo WithShellExecute(this CommandStartInfo startInfo, bool shellExecute = true)
    {
        startInfo.UseShellExecute = shellExecute;
        return startInfo;
    }

    public static CommandStartInfo WithWindow(this CommandStartInfo startInfo, bool createWindow = true)
    {
        startInfo.CreateNoWindow = !createWindow;
        return startInfo;
    }

    public static CommandStartInfo WithCwd(this CommandStartInfo startInfo, string workingDirectory)
    {
        startInfo.Cwd = workingDirectory;
        return startInfo;
    }

    public static CommandStartInfo WithWorkingDirectory(this CommandStartInfo startInfo, string workingDirectory)
        => startInfo.WithCwd(workingDirectory);
}