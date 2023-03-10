using System;
using System.IO;
using System.Management.Automation;
using System.Security;

using Bearz.Std;
using Bearz.Text;

namespace Bearz.PowerShell.Standard;

[Alias("env_remove")]
[Cmdlet(VerbsCommon.Remove, "EnvironmentVariable")]
public class RemoveEnvironmentVariableCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    public string Name { get; set; } = string.Empty;

    [Parameter]
    public EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.Process;

    protected override void ProcessRecord()
    {
        if (Env.IsWindows())
        {
            Env.Unset(this.Name, this.Target);
            return;
        }

        if (!Env.IsWindows() && this.Target != EnvironmentVariableTarget.Process)
        {
            this.WriteError(
                new PlatformNotSupportedException(
                    "Only EnvironmentVariableTarget.Process is supported on non-Windows platforms."));
            return;
        }

        if (this.Target == EnvironmentVariableTarget.Machine && !Env.IsUserElevated)
        {
            this.WriteError(
                new SecurityException("You must be an administrator to modify the machine environment path."));
            return;
        }

        Env.Unset(this.Name);
    }
}