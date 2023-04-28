using System.Management.Automation;

using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Alias("is_admin", "Test-UserIsAdministrator")]
[Cmdlet(VerbsDiagnostic.Test, "UserIsAdmin")]
[OutputType(typeof(bool))]
public class TestUserIsAdminCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        this.WriteObject(Env.IsUserElevated);
    }
}