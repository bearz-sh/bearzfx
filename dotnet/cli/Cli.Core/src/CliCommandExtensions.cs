using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;

using Bearz.Diagnostics;
using Bearz.Std;

namespace Bearz.Cli;

public static class CliCommandExtensions
{
    public static CliCommand WithStdio(this CliCommand command, Stdio stdio)
    {
        command.StartInfo.StdIn = stdio;
        command.StartInfo.StdOut = stdio;
        command.StartInfo.StdErr = stdio;
        return command;
    }

    public static CliCommand WithStdIn(this CliCommand command, Stdio stdio)
    {
        command.StartInfo.StdIn = stdio;
        return command;
    }

    public static CliCommand WithStdOut(this CliCommand command, Stdio stdio)
    {
        command.StartInfo.StdOut = stdio;
        return command;
    }

    public static CliCommand WithStdErr(this CliCommand command, Stdio stdio)
    {
        command.StartInfo.StdErr = stdio;
        return command;
    }

    public static CliCommand WithArgs(this CliCommand command, params string[] args)
    {
        command.StartInfo.Args = args;
        return command;
    }

    public static CliCommand WithArgs(this CliCommand command, IEnumerable<string> args)
    {
        command.StartInfo.Args = new CommandArgs(args);
        return command;
    }

    public static CliCommand WithArgs(this CliCommand command, CommandArgs args)
    {
        command.StartInfo.Args = args;
        return command;
    }

    public static CliCommand AsUser(this CliCommand command, string user)
    {
        command.StartInfo.User = user;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static CliCommand AsUser(this CliCommand command, string user, string password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.PasswordInClearText = password;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static CliCommand AsUser(this CliCommand command, string user, string password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.PasswordInClearText = password;
        command.StartInfo.Domain = domain;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static CliCommand AsUser(this CliCommand command, string user, SecureString password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.Password = password;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static CliCommand AsUser(this CliCommand command, string user, SecureString password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.Password = password;
        command.StartInfo.Domain = domain;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static unsafe CliCommand AsUser(this CliCommand command, string user, ReadOnlySpan<char> password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        fixed (char* pChars = &password.GetPinnableReference())
        {
            var ss = new SecureString(pChars, password.Length);
            command.StartInfo.Password = ss;
        }

        command.StartInfo.Domain = domain;
        return command;
    }

    public static CliCommand AddArg(this CliCommand command, string arg)
    {
        command.StartInfo.Args.Add(arg);
        return command;
    }

    public static CliCommand AsSudo(this CliCommand command)
    {
        command.StartInfo.Verb = "sudo";
        return command;
    }

    public static CliCommand AsAdmin(this CliCommand command)
    {
        command.StartInfo.Verb = "runas";
        return command;
    }

    public static CliCommand AsOsAdmin(this CliCommand command)
    {
        command.StartInfo.Verb = "admin";
        return command;
    }

    public static CliCommand WithVerb(this CliCommand command, string verb)
    {
        command.StartInfo.Verb = verb;
        return command;
    }

    public static CliCommand AddEnv(this CliCommand command, string key, string? value)
    {
        command.StartInfo.Env ??= new Dictionary<string, string?>();
        command.StartInfo.Env[key] = value;
        return command;
    }

    public static CliCommand WithEnv(this CliCommand command, IDictionary<string, string?> env)
    {
        command.StartInfo.Env ??= new Dictionary<string, string?>();
        foreach (var kvp in env)
        {
            command.StartInfo.Env[kvp.Key] = kvp.Value;
        }

        return command;
    }

    public static CliCommand WithShellExecute(this CliCommand command, bool shellExecute = true)
    {
        command.StartInfo.UseShellExecute = shellExecute;
        return command;
    }

    public static CliCommand WithWindow(this CliCommand command, bool createWindow = true)
    {
        command.StartInfo.CreateNoWindow = !createWindow;
        return command;
    }

    public static CliCommand WithCwd(this CliCommand command, string workingDirectory)
    {
        command.StartInfo.Cwd = workingDirectory;
        return command;
    }

    public static CliCommand WithWorkingDirectory(this CliCommand command, string workingDirectory)
        => WithCwd(command, workingDirectory);

    public static CliCommand RedirectTo(this CliCommand command, IProcessCapture capture)
    {
        command.StartInfo.RedirectTo(capture);
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, TextWriter writer)
    {
        command.StartInfo.RedirectTo(new StreamCapture(writer));
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, FileInfo fileInfo, Encoding? encoding = null)
    {
        command.StartInfo.RedirectTo(new StreamCapture(fileInfo, encoding ?? Encoding.UTF8));
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, IObserver<string> observer)
    {
        command.StartInfo.RedirectTo(new ObserverCapture(observer));
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, Stream stream, Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false)
    {
        command.StartInfo.RedirectTo(new StreamCapture(stream, encoding, bufferSize, leaveOpen));
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, Action<string, System.Diagnostics.Process> action, Action<System.Diagnostics.Process>? onComplete = null)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        command.StartInfo.RedirectTo(new ActionCapture(action, onComplete));
        return command;
    }

    public static CliCommand RedirectTo(this CliCommand command, ICollection<string> collection)
    {
        command.StartInfo.RedirectTo(new CollectionCapture(collection));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, IProcessCapture capture)
    {
        command.StartInfo.RedirectErrorTo(capture);
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, TextWriter writer)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(writer));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, FileInfo fileInfo, Encoding? encoding = null)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(fileInfo, encoding ?? Encoding.UTF8));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, IObserver<string> observer)
    {
        command.StartInfo.RedirectErrorTo(new ObserverCapture(observer));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, Stream stream, Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(stream, encoding, bufferSize, leaveOpen));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, Action<string, System.Diagnostics.Process> action, Action<System.Diagnostics.Process>? onComplete = null)
    {
        command.StartInfo.RedirectErrorTo(new ActionCapture(action, onComplete));
        return command;
    }

    public static CliCommand RedirectErrorTo(this CliCommand command, ICollection<string> collection)
    {
        command.StartInfo.RedirectErrorTo(new CollectionCapture(collection));
        return command;
    }
}