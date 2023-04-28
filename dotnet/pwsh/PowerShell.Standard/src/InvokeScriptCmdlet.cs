using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

using Bearz.Diagnostics;
using Bearz.Extra.Collections;
using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Std.Unix;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsLifecycle.Invoke, "Script")]
[OutputType(typeof(CommandOutput))]
public class InvokeScriptCmdlet : PSCmdlet
{
    static InvokeScriptCmdlet()
    {
        Shells = new(StringComparer.OrdinalIgnoreCase);
        Shells.Add(
            "bash",
            new ScriptResolver()
            {
                Shell = "bash",
                Extension = ".sh",
                ArgumentList = new CommandArgs()
                {
                    "--noprofile",
                    "--norc",
                    "-e",
                    "-o",
                    "pipefail",
                },
            });

        Shells.Add(
            "pwsh",
            new ScriptResolver()
            {
                Shell = "pwsh",
                Extension = ".ps1",
                ArgumentList = new CommandArgs()
                {
                    "-NoLogo",
                    "-ExecutionPolicy",
                    "ByPass",
                    "-NonInteractive",
                    "-NoProfile",
                    "-Command",
                },
                ScriptFormat = """
    $ErrorActionPreference = 'Stop';
    {0}
    if((Test-Path -LiteralPath variable:LASTEXITCODE))
    {
        exit $LASTEXITCODE;
    }
    """,
            });

        Shells.Add("powershell", new ScriptResolver()
        {
            Shell = "powershell",
            Extension = ".ps1",
            ArgumentList = new CommandArgs()
            {
                "-NoLogo",
                "-ExecutionPolicy",
                "ByPass",
                "-NonInteractive",
                "-NoProfile",
                "-Command",
            },
            ScriptFormat = """
$ErrorActionPreference = 'Stop';
{0}
if((Test-Path -LiteralPath variable:LASTEXITCODE))
{
    exit $LASTEXITCODE;
}
""",
        });

        Shells.Add("sh", new ScriptResolver()
        {
            Shell = "sh",
            Extension = ".sh",
            ArgumentList = new CommandArgs() { "-e" },
        });
    }

    public static Dictionary<string, ScriptResolver> Shells { get; set; }

    [Parameter(Position = 0, Mandatory = true)]
    public string Script { get; set; } = string.Empty;

    [ArgumentCompleter(typeof(Completer))]
    [Parameter(Position = 1)]
    public string Shell { get; set; } = string.Empty;

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
    public Hashtable? Environment { get; set; }

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
        if (this.Shell.IsNullOrWhiteSpace())
        {
            if (Env.IsWindows())
            {
                this.Shell = "powershell";
            }
            else
            {
                this.Shell = "bash";
            }
        }

        if (!Shells.TryGetValue(this.Shell, out var resolver) || resolver == null)
        {
            this.ThrowTerminatingError(new InvalidOperationException($"Unknown shell {this.Shell}"));
            return;
        }

        var script = this.Script;

        if (!resolver.ScriptFormat.IsNullOrWhiteSpace())
        {
            script = string.Format(resolver.ScriptFormat, script);
        }

        var tmpDir = System.IO.Path.GetTempPath();
        var tmpFile = FsPath.Combine(tmpDir, $"{System.IO.Path.GetRandomFileName()}.{resolver.Extension}");
        if (!System.IO.File.Exists(tmpFile))
        {
            System.IO.File.WriteAllText(tmpFile, script);
        }

        try
        {
            var exe = Env.Process.Which(this.Shell);
            if (exe.IsNullOrWhiteSpace())
            {
                this.ThrowTerminatingError(new NotFoundOnPathException(this.Shell));
                return;
            }

            Dictionary<string, string?>? env = null;
            var args = new CommandArgs();
            if (resolver.ArgumentList != null)
                args.AddRange(resolver.ArgumentList);

            args.Add(tmpFile);

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
                        }
                        else if (value is string vstr)
                        {
                            env[name] = vstr;
                        }
                    }
                }
            }

            if (this.AsSudo.ToBool() && !Env.IsWindows() && !UnixUser.IsRoot)
            {
                args!.Prepend(exe);
                exe = "sudo";
            }

            this.WriteCommand(resolver.Shell, args);

            var ci = new CommandStartInfo()
            {
                Args = args,
                StdOut = this.StdOut,
                StdErr = this.StdError,
                Cwd = this.WorkingDirectory,
                Env = env,
            };

            foreach (var capture in this.StdOutCapture)
            {
                ci.StdOut = Stdio.Piped;
                ci.RedirectTo(capture);
            }

            foreach (var capture in this.StdErrorCapture)
            {
                ci.StdErr = Stdio.Piped;
                ci.RedirectErrorTo(capture);
            }

            var cmd = Env.Process.CreateCommand(exe, ci);

            var r = cmd.Output();
            if (r.ExitCode != 0)
            {
                this.ThrowTerminatingError(new ProcessException(r.ExitCode, exe));
                return;
            }

            if (this.PassThru.ToBool() || this.StdOut == Stdio.Piped || this.StdError == Stdio.Piped)
            {
                this.WriteObject(r);
            }
        }
        finally
        {
            if (System.IO.File.Exists(tmpFile))
                System.IO.File.Delete(tmpFile);
        }
    }

    public class Completer : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            if (wordToComplete.IsNullOrWhiteSpace())
                return Shells.Keys.Select(x => new CompletionResult(x));

            return Shells.Keys.Where(x => x.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase)).Select(x => new CompletionResult(x));
        }
    }
}