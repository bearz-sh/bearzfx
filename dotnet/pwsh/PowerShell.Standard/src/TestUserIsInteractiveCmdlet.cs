using System;
using System.Linq;
using System.Management.Automation;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsDiagnostic.Test, "UserIsInteractive")]
[OutputType(typeof(bool))]
public class TestUserIsInteractiveCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        if (ModuleState.ShellInteractive.HasValue)
        {
            this.WriteObject(ModuleState.ShellInteractive.Value);
            return;
        }

        ModuleState.ShellInteractive = true;

        var pwshInteractive = Env.Get("POWERSHELL_INTERACTIVE");
        if (pwshInteractive is not null && !pwshInteractive.EqualsIgnoreCase("true") && pwshInteractive != "1")
        {
            ModuleState.ShellInteractive = false;
            this.WriteObject(ModuleState.ShellInteractive);
            return;
        }

        if (!Env.IsUserInteractive)
        {
            ModuleState.ShellInteractive = false;
        }
        else if (!Environment.UserInteractive)
        {
            ModuleState.ShellInteractive = false;
        }
        else if (Env.Process.Argv.Any(o => o.EqualsIgnoreCase("-NonInteractive")))
        {
            ModuleState.ShellInteractive = false;
        }
        else if (Env.Get("DEBIAN_FRONTEND")?.EqualsIgnoreCase("noninteractive") == true)
        {
            ModuleState.ShellInteractive = false;
        }
        else if (Env.Get("CI")?.EqualsIgnoreCase("true") == true)
        {
            ModuleState.ShellInteractive = false;
        }
        else if (Env.Get("TF_BUILD")?.EqualsIgnoreCase("true") == true)
        {
            ModuleState.ShellInteractive = false;
        }

        Env.Set("POWERSHELL_INTERACTIVE", ModuleState.ShellInteractive.Value.ToString());

        this.WriteObject(ModuleState.ShellInteractive);
    }
}