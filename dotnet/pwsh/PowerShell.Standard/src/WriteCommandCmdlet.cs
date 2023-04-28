using System.Management.Automation;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsCommunications.Write, "Command")]
public class WriteCommandCmdlet : PSCmdlet
{
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string? Command { get; set; }

    [Parameter(Position = 1)]
    public CommandArgs? ArgumentList { get; set; }

    [Parameter]
    public ActionPreference? CommandAction { get; set; }

    protected override void ProcessRecord()
    {
        if (this.Command.IsNullOrWhiteSpace())
            return;

        this.ArgumentList ??= new CommandArgs();
        this.WriteCommand(this.Command, this.ArgumentList, this.CommandAction);
    }
}