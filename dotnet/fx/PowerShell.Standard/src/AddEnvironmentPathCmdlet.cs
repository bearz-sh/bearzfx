using System;
using System.Management.Automation;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsCommon.Add, "EnvironmentPath")]
[OutputType(typeof(void))]
public class AddEnvironmentPathCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    public string[] Path { get; set; } = Array.Empty<string>();

    [Parameter]
    public EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.Process;

    [Parameter]
    public SwitchParameter Prepend { get; set; }

    protected override void ProcessRecord()
    {
        if (!Env.IsWindows() && this.Target != EnvironmentVariableTarget.Process)
        {
            this.WriteError(
                new PlatformNotSupportedException(
                    "Only EnvironmentVariableTarget.Process is supported on non-Windows platforms."));
            return;
        }

        foreach (var path in this.Path)
        {
            if (path.IsNullOrWhiteSpace())
                continue;

            EnvPath.Add(path, this.Prepend, this.Target);
        }
    }
}