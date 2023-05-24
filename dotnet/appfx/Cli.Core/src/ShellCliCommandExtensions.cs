using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;

using Bearz.Diagnostics;
using Bearz.Std;

namespace Bearz.Cli;

public static class ShellCliCommandExtensions
{
    public static ShellCliCommand WithStdio(this ShellCliCommand command, Stdio stdio)
    {
        command.StartInfo.StdIn = stdio;
        command.StartInfo.StdOut = stdio;
        command.StartInfo.StdErr = stdio;
        return command;
    }

    public static ShellCliCommand WithStdIn(this ShellCliCommand command, Stdio stdio)
    {
        command.StartInfo.StdIn = stdio;
        return command;
    }

    public static ShellCliCommand WithStdOut(this ShellCliCommand command, Stdio stdio)
    {
        command.StartInfo.StdOut = stdio;
        return command;
    }

    public static ShellCliCommand WithStdErr(this ShellCliCommand command, Stdio stdio)
    {
        command.StartInfo.StdErr = stdio;
        return command;
    }

    public static ShellCliCommand WithArgs(this ShellCliCommand command, params string[] args)
    {
        command.StartInfo.Args = args;
        return command;
    }

    public static ShellCliCommand WithArgs(this ShellCliCommand command, IEnumerable<string> args)
    {
        command.StartInfo.Args = new CommandArgs(args);
        return command;
    }

    public static ShellCliCommand WithArgs(this ShellCliCommand command, CommandArgs args)
    {
        command.StartInfo.Args = args;
        return command;
    }

    public static ShellCliCommand AsUser(this ShellCliCommand command, string user)
    {
        command.StartInfo.User = user;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static ShellCliCommand AsUser(this ShellCliCommand command, string user, string password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.PasswordInClearText = password;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static ShellCliCommand AsUser(this ShellCliCommand command, string user, string password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.PasswordInClearText = password;
        command.StartInfo.Domain = domain;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static ShellCliCommand AsUser(this ShellCliCommand command, string user, SecureString password)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.Password = password;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static ShellCliCommand AsUser(this ShellCliCommand command, string user, SecureString password, string domain)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Calling AsUser() with Password is only supported on Windows");

        command.StartInfo.User = user;
        command.StartInfo.Password = password;
        command.StartInfo.Domain = domain;
        return command;
    }

    [SupportedOSPlatform("windows")]
    public static unsafe ShellCliCommand AsUser(this ShellCliCommand command, string user, ReadOnlySpan<char> password, string domain)
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

    public static ShellCliCommand AddArg(this ShellCliCommand command, string arg)
    {
        command.StartInfo.Args.Add(arg);
        return command;
    }

    public static ShellCliCommand AsSudo(this ShellCliCommand command)
    {
        command.StartInfo.Verb = "sudo";
        return command;
    }

    public static ShellCliCommand AsAdmin(this ShellCliCommand command)
    {
        command.StartInfo.Verb = "runas";
        return command;
    }

    public static ShellCliCommand AsOsAdmin(this ShellCliCommand command)
    {
        command.StartInfo.Verb = "admin";
        return command;
    }

    public static ShellCliCommand WithVerb(this ShellCliCommand command, string verb)
    {
        command.StartInfo.Verb = verb;
        return command;
    }

    public static ShellCliCommand AddEnv(this ShellCliCommand command, string key, string? value)
    {
        command.StartInfo.Env ??= new Dictionary<string, string?>();
        command.StartInfo.Env[key] = value;
        return command;
    }

    public static ShellCliCommand WithEnv(this ShellCliCommand command, IDictionary<string, string?> env)
    {
        command.StartInfo.Env ??= new Dictionary<string, string?>();
        foreach (var kvp in env)
        {
            command.StartInfo.Env[kvp.Key] = kvp.Value;
        }

        return command;
    }

    public static ShellCliCommand WithShellExecute(this ShellCliCommand command, bool shellExecute = true)
    {
        command.StartInfo.UseShellExecute = shellExecute;
        return command;
    }

    public static ShellCliCommand WithWindow(this ShellCliCommand command, bool createWindow = true)
    {
        command.StartInfo.CreateNoWindow = !createWindow;
        return command;
    }

    public static ShellCliCommand WithCwd(this ShellCliCommand command, string workingDirectory)
    {
        command.StartInfo.Cwd = workingDirectory;
        return command;
    }

    public static ShellCliCommand WithWorkingDirectory(this ShellCliCommand command, string workingDirectory)
        => WithCwd(command, workingDirectory);

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, IProcessCapture capture)
    {
        command.StartInfo.RedirectTo(capture);
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, TextWriter writer)
    {
        command.StartInfo.RedirectTo(new StreamCapture(writer));
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, FileInfo fileInfo, Encoding? encoding = null)
    {
        command.StartInfo.RedirectTo(new StreamCapture(fileInfo, encoding ?? Encoding.UTF8));
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, IObserver<string> observer)
    {
        command.StartInfo.RedirectTo(new ObserverCapture(observer));
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, Stream stream, Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false)
    {
        command.StartInfo.RedirectTo(new StreamCapture(stream, encoding, bufferSize, leaveOpen));
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, Action<string, System.Diagnostics.Process> action, Action<System.Diagnostics.Process>? onComplete = null)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        command.StartInfo.RedirectTo(new ActionCapture(action, onComplete));
        return command;
    }

    public static ShellCliCommand RedirectTo(this ShellCliCommand command, ICollection<string> collection)
    {
        command.StartInfo.RedirectTo(new CollectionCapture(collection));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, IProcessCapture capture)
    {
        command.StartInfo.RedirectErrorTo(capture);
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, TextWriter writer)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(writer));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, FileInfo fileInfo, Encoding? encoding = null)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(fileInfo, encoding ?? Encoding.UTF8));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, IObserver<string> observer)
    {
        command.StartInfo.RedirectErrorTo(new ObserverCapture(observer));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, Stream stream, Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false)
    {
        command.StartInfo.RedirectErrorTo(new StreamCapture(stream, encoding, bufferSize, leaveOpen));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, Action<string, System.Diagnostics.Process> action, Action<System.Diagnostics.Process>? onComplete = null)
    {
        command.StartInfo.RedirectErrorTo(new ActionCapture(action, onComplete));
        return command;
    }

    public static ShellCliCommand RedirectErrorTo(this ShellCliCommand command, ICollection<string> collection)
    {
        command.StartInfo.RedirectErrorTo(new CollectionCapture(collection));
        return command;
    }
}