using System.Management.Automation;

namespace Bearz.PowerShell.Standard;

[Alias("register_backwards_compatibility")]
[Cmdlet(VerbsLifecycle.Register, "BackwardsCompatibilityLayer")]
public class RegisterBackwardsCompatibilityLayerCmdlet : PSCmdlet
{
    protected override void ProcessRecord()
    {
        this.RegisterBackwardsCompatibility();
        base.ProcessRecord();
    }
}