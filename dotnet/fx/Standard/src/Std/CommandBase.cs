using System.Diagnostics;

using Bearz.Diagnostics;
using Bearz.Extra.Collections;
using Bearz.Extra.Strings;

using ChildProcess = System.Diagnostics.Process;

namespace Bearz.Std;

public abstract class CommandBase
{
    public abstract string FileName { get; }

    public abstract CommandStartInfo StartInfo { get; }

    public virtual CommandOutput Output()
    {
        using var cmd = new System.Diagnostics.Process();
        var (stdOut, stdErr) = this.GetStandardOutput(cmd);
        this.MapStartInfo(cmd);

        cmd.Start();
        var started = DateTime.UtcNow;
        var pid = cmd.Id;
        try
        {
            // throws an exception on linux
            started = cmd.StartTime.ToUniversalTime();
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }

        cmd.EnableRaisingEvents = true;
        if (cmd.StartInfo.RedirectStandardOutput)
            cmd.BeginOutputReadLine();

        if (cmd.StartInfo.RedirectStandardError)
            cmd.BeginErrorReadLine();

        cmd.WaitForExit();
        var ended = cmd.ExitTime.ToUniversalTime();

        return new CommandOutput()
        {
            Id = pid,
            Executable = cmd.StartInfo.FileName,
            ExitCode = cmd.ExitCode,
            StartedAt = started,
            ExitedAt = ended,
            StdOut = stdOut,
            StdErr = stdErr,
        };
    }

    public virtual async Task<CommandOutput> OutputAsync(CancellationToken cancellationToken = default)
    {
        using var cmd = new System.Diagnostics.Process();
        var (stdOut, stdErr) = this.GetStandardOutput(cmd);
        this.MapStartInfo(cmd);

        cmd.Start();
        var started = DateTime.UtcNow;
        try
        {
            // throws an exception on linux
            started = cmd.StartTime.ToUniversalTime();
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }

        var pid = cmd.Id;

        cmd.EnableRaisingEvents = true;
        if (cmd.StartInfo.RedirectStandardOutput)
            cmd.BeginOutputReadLine();

        if (cmd.StartInfo.RedirectStandardError)
            cmd.BeginErrorReadLine();

        await cmd.WaitForExitAsync(cancellationToken);
        var ended = cmd.ExitTime.ToUniversalTime();

        return new CommandOutput()
        {
            Id = pid,
            Executable = cmd.StartInfo.FileName,
            ExitCode = cmd.ExitCode,
            StartedAt = started,
            ExitedAt = ended,
            StdOut = stdOut,
            StdErr = stdErr,
        };
    }

    public virtual ChildProcess CreateProcess()
    {
        var cmd = new ChildProcess();
        this.MapStartInfo(cmd);
        return cmd;
    }

    public virtual ChildProcess Spawn()
    {
        var cmd = this.CreateProcess();
        cmd.Start();
        cmd.EnableRaisingEvents = true;
        if (cmd.StartInfo.RedirectStandardOutput)
            cmd.BeginOutputReadLine();

        if (cmd.StartInfo.RedirectStandardError)
            cmd.BeginErrorReadLine();

        return cmd;
    }

    private (IReadOnlyList<string>, IReadOnlyList<string>) GetStandardOutput(ChildProcess cmd)
    {
        IReadOnlyList<string> standardOut = Array.Empty<string>();
        IReadOnlyList<string> standardError = Array.Empty<string>();

        if (this.StartInfo.StdOut != Stdio.Inherit)
        {
            cmd.StartInfo.RedirectStandardOutput = true;
            if (this.StartInfo.StdOut == Stdio.Piped)
            {
                var list = new List<string>();
                standardOut = list;
                this.StartInfo.RedirectTo(new CollectionCapture(list));
            }
            else if (this.StartInfo.StdOutRedirects.Count == 0)
            {
                // equivalent to /dev/null
                cmd.OutputDataReceived += (_, _) => { };
            }
        }

        if (this.StartInfo.StdErr != Stdio.Inherit && this.StartInfo.StdErrorRedirects.Count == 0)
        {
            cmd.StartInfo.RedirectStandardError = true;
            if (this.StartInfo.StdErr == Stdio.Piped)
            {
                var stdErr = new List<string>();
                this.StartInfo.RedirectErrorTo(new CollectionCapture(stdErr));
                standardError = stdErr;
            }
            else if (this.StartInfo.StdErrorRedirects.Count == 0)
            {
                // equivalent to /dev/null
                cmd.ErrorDataReceived += (_, _) => { };
            }
        }

        return (standardOut, standardError);
    }

    private void MapStartInfo(ChildProcess cmd)
    {
        var si = this.StartInfo;
        cmd.StartInfo.FileName = this.FileName;
        if (this.StartInfo.Verb?.EqualsIgnoreCase("runas") == true)
        {
            if (Std.Env.IsWindows)
            {
                cmd.StartInfo.Verb = "runas";
            }
            else
            {
                 throw new NotSupportedException("Verb 'runas' is only supported on Windows");
            }
        }

        if (this.StartInfo.Verb?.EqualsIgnoreCase("sudo") == true)
        {
            if (!Std.Env.IsWindows)
            {
                cmd.StartInfo.FileName = "sudo";
                si.Args.Prepend(this.FileName);
            }
            else
            {
                 throw new NotSupportedException("Verb 'sudo' is only supported on Unix");
            }
        }

        if (this.StartInfo.Verb?.EqualsIgnoreCase("admin") == true || this.StartInfo.Verb?.EqualsIgnoreCase("root") == true)
        {
            if (Std.Env.IsWindows)
            {
                cmd.StartInfo.Verb = "runas";
            }
            else
            {
                cmd.StartInfo.FileName = "sudo";
                si.Args.Prepend(this.FileName);
            }
        }

#if NET5_0_OR_GREATER
        foreach (var t in si.Args)
        {
            cmd.StartInfo.ArgumentList.Add(t);
        }
#else
        cmd.StartInfo.Arguments = si.Args.ToString();
#endif

        if (si.StdOutRedirects.Count > 0)
        {
            this.StartInfo.StdOut = Stdio.Piped;
            foreach (var capture in si.StdOutRedirects)
            {
                cmd.RedirectTo(capture);
            }
        }

        if (si.StdErrorRedirects.Count > 0)
        {
            this.StartInfo.StdErr = Stdio.Piped;
            foreach (var capture in si.StdErrorRedirects)
            {
                cmd.RedirectErrorTo(capture);
            }
        }

        if (!si.User.IsNullOrWhiteSpace())
        {
            cmd.StartInfo.UserName = si.User;

            if (Std.Env.IsWindows)
            {
                if (si.Password != null)
                {
                    cmd.StartInfo.Password = si.Password;
                }
                else if (!si.PasswordInClearText.IsNullOrWhiteSpace())
                {
                    cmd.StartInfo.PasswordInClearText = si.PasswordInClearText;
                }

                if (!si.Domain.IsNullOrWhiteSpace())
                {
                    cmd.StartInfo.Domain = si.Domain;
                }

                cmd.StartInfo.LoadUserProfile = si.LoadUserProfile;
            }
            else
            {
                if (si.Password != null)
                {
                    throw new PlatformNotSupportedException("Password is only supported on Windows");
                }

                if (!si.PasswordInClearText.IsNullOrWhiteSpace())
                {
                    throw new PlatformNotSupportedException("PasswordInClearText is only supported on Windows");
                }

                if (!si.Domain.IsNullOrWhiteSpace())
                {
                    cmd.StartInfo.Domain = si.Domain;
                }

                if (si.LoadUserProfile)
                    throw new PlatformNotSupportedException("LoadUserProfile is only supported on Windows");
            }
        }

        cmd.StartInfo.WorkingDirectory = this.StartInfo.Cwd;
        cmd.StartInfo.RedirectStandardOutput = this.StartInfo.StdOut != Stdio.Inherit;
        cmd.StartInfo.RedirectStandardError = this.StartInfo.StdErr != Stdio.Inherit;
        cmd.StartInfo.RedirectStandardInput = this.StartInfo.StdIn != Stdio.Inherit;
        cmd.EnableRaisingEvents = cmd.StartInfo.RedirectStandardOutput || cmd.StartInfo.RedirectStandardError
                                                                       || cmd.StartInfo.RedirectStandardInput;
        cmd.StartInfo.UseShellExecute = si.UseShellExecute;
        cmd.StartInfo.CreateNoWindow = si.CreateNoWindow;
        if (this.StartInfo.Env != null)
        {
            if (this.StartInfo.Env.Count == 0)
            {
                cmd.StartInfo.Environment.Clear();
            }
            else
            {
                foreach (var kvp in this.StartInfo.Env)
                {
                    if (kvp.Value == null)
                    {
                        cmd.StartInfo.Environment.Remove(kvp.Key);
                        continue;
                    }

                    cmd.StartInfo.Environment[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}