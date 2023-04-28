using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

using Bearz.Diagnostics;
using Bearz.Extra.Collections;
using Bearz.Extra.Strings;
using Bearz.PowerShell;
using Bearz.Std;
using Bearz.Std.Unix;

namespace Ze.PowerShell.Core;

[Alias("invoke_process")]
[Cmdlet(VerbsLifecycle.Invoke, "Process")]
[OutputType(typeof(CommandOutput))]
public class InvokeProcessCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0)]
    public string? Executable { get; set; }

    [Parameter]
    public CommandArgs? Arguments { get; set; }

    [Parameter]
    public Stdio StdOut { get; set; } = Stdio.Inherit;

    [Parameter]
    public Stdio StdError { get; set; } = Stdio.Inherit;

    [Parameter]
    public SwitchParameter AsSudo { get; set; }

    [Parameter]
    public ActionPreference? CommandAction { get; set; }

    [Alias("Env", "e")]
    [Parameter]
    public IDictionary? Environment { get; set; }

    [Alias("Cwd", "Wd")]
    [Parameter]
    public string? WorkingDirectory { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    [Parameter]
    public IEnumerable<IProcessCapture> StdOutCapture { get; set; } = Array.Empty<IProcessCapture>();

    [Parameter]
    public IEnumerable<IProcessCapture> StdErrorCapture { get; set; } = Array.Empty<IProcessCapture>();

    protected override void ProcessRecord()
    {
        Dictionary<string, string?>? env = null;

        if (this.Executable.IsNullOrWhiteSpace())
            throw new PSArgumentNullException(nameof(this.Executable));

        var args = this.Arguments;
        args ??= new CommandArgs();

        var exe = Env.Process.Which(this.Executable);
        if (exe is null)
            throw new NotFoundOnPathException(this.Executable);

        if (this.AsSudo.ToBool() && !Env.IsWindows() && !UnixUser.IsRoot)
        {
            args.Prepend(exe);
            exe = "sudo";
        }

        if (this.Environment != null)
        {
            env = new Dictionary<string, string?>();
            foreach (var key in this.Environment.Keys)
            {
                if (key is string name)
                {
                    var value = this.Environment[name];
                    if (value is null)
                    {
                        env[name] = null;
                        continue;
                    }

                    if (value is string str)
                    {
                        env[name] = str;
                    }
                }
            }
        }

        var ci = new CommandStartInfo()
        {
            Args = args,
            Env = env,
            Cwd = this.WorkingDirectory,
            StdOut = this.StdOut,
            StdErr = this.StdError,
        };

        foreach (var capture in this.StdOutCapture)
        {
            ci.RedirectTo(capture);
        }

        foreach (var capture in this.StdErrorCapture)
        {
            ci.RedirectErrorTo(capture);
        }

        var cmd = Env.Process.CreateCommand(exe, ci);

        this.WriteCommand(this.Executable, this.Arguments, this.CommandAction);

        var result = cmd.Output();
        this.SessionState.PSVariable.Set("LASTEXITCODE", result.ExitCode);

        if (this.PassThru.ToBool() || this.StdOut == Stdio.Piped || this.StdError == Stdio.Piped)
        {
            this.WriteObject(result);
        }
    }
}