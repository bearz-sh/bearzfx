using System.Diagnostics;

using Bearz.Diagnostics;

using ChildProcess = System.Diagnostics.Process;

namespace Bearz.Std;

public class Command
{
    public Command(string fileName, CommandStartInfo? startInfo)
    {
        this.FileName = fileName;
        this.StartInfo = startInfo ?? new CommandStartInfo();
    }

    public string FileName { get; }

    public CommandStartInfo StartInfo { get; }

    public Command Cwd(string cwd)
    {
        this.StartInfo.Cwd = cwd;
        return this;
    }

    public Command Arg(string value)
    {
        this.StartInfo.Args.Add(value);
        return this;
    }

    public Command StdOut(Stdio stdout)
    {
        this.StartInfo.StdOut = stdout;
        return this;
    }

    public Command StdErr(Stdio stderr)
    {
        this.StartInfo.StdErr = stderr;
        return this;
    }

    public Command StdIn(Stdio stdin)
    {
        this.StartInfo.StdIn = stdin;
        return this;
    }

    public Command ClearEnv()
    {
        this.StartInfo.Env ??= new Dictionary<string, string>();
        return this;
    }

    public Command Env(string name, string value)
    {
        this.StartInfo.Env ??= new Dictionary<string, string>();
        this.StartInfo.Env[name] = value;
        return this;
    }

    public CommandOutput Output()
    {
        using var cmd = new System.Diagnostics.Process();
        this.MapStartInfo(cmd);
        var (stdOut, stdErr) = this.GetStandardOutput(cmd);

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

    public async Task<CommandOutput> OutputAsync(CancellationToken cancellationToken = default)
    {
        using var cmd = new System.Diagnostics.Process();
        this.MapStartInfo(cmd);
        var (stdOut, stdErr) = this.GetStandardOutput(cmd);

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

    public ChildProcess CreateProcess()
    {
        var cmd = new ChildProcess();
        this.MapStartInfo(cmd);
        return cmd;
    }

    public ChildProcess Spawn()
    {
        var cmd = this.CreateProcess();
        cmd.Start();
        return cmd;
    }

    private (IReadOnlyList<string>, IReadOnlyList<string>) GetStandardOutput(ChildProcess cmd)
    {
        IReadOnlyList<string> standardOut = Array.Empty<string>();
        IReadOnlyList<string> standardError = Array.Empty<string>();

        if (this.StartInfo.StdOut != Stdio.Inherit && this.StartInfo.StdOutRedirects.Count == 0)
        {
            cmd.StartInfo.RedirectStandardOutput = true;
            if (this.StartInfo.StdOut == Stdio.Piped)
            {
                var stdOut = new List<string>();
                cmd.OutputDataReceived += (_, e) =>
                {
                    if (e.Data == null)
                        return;
                    stdOut.Add(e.Data);
                };

                standardOut = stdOut;
            }
            else
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
                cmd.ErrorDataReceived += (_, e) =>
                {
                    if (e.Data == null)
                        return;

                    stdErr.Add(e.Data);
                };

                standardError = stdErr;
            }
            else
            {
                // equivalent to /dev/null
                cmd.ErrorDataReceived += (_, _) => { };
            }
        }

        return (standardOut, standardError);
    }

    private void MapStartInfo(ChildProcess cmd)
    {
        cmd.StartInfo.FileName = this.FileName;
#if NET5_0_OR_GREATER
        foreach (var t in this.StartInfo.Args)
        {
            cmd.StartInfo.ArgumentList.Add(t);
        }
#else
        cmd.StartInfo.Arguments = this.StartInfo.Args.ToString();
#endif
        var si = this.StartInfo;
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

        cmd.StartInfo.WorkingDirectory = this.StartInfo.Cwd;
        cmd.StartInfo.RedirectStandardOutput = this.StartInfo.StdOut != Stdio.Inherit;
        cmd.StartInfo.RedirectStandardError = this.StartInfo.StdErr != Stdio.Inherit;
        cmd.StartInfo.RedirectStandardInput = this.StartInfo.StdIn != Stdio.Inherit;
        cmd.EnableRaisingEvents = cmd.StartInfo.RedirectStandardOutput || cmd.StartInfo.RedirectStandardError
                                                                       || cmd.StartInfo.RedirectStandardInput;
        cmd.StartInfo.UseShellExecute = false;
        cmd.StartInfo.CreateNoWindow = true;
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
                    cmd.StartInfo.Environment[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}