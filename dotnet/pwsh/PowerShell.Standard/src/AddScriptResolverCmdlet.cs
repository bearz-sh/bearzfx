using System.Management.Automation;

using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsCommon.Add, "ScriptResolver")]
[OutputType(typeof(void))]
public class AddScriptResolverCmdlet : PSCmdlet
{
    [Parameter(Position = 0)]
    public string Shell { get; set; } = string.Empty;

    [Parameter(Position = 1)]
    public string Extension { get; set; } = string.Empty;

    [Parameter(Position = 2)]
    public string? ScriptFormat { get; set; }

    public CommandArgs? CommandArgs { get; set; }

    protected override void ProcessRecord()
    {
        InvokeScriptCmdlet.Shells[this.Shell] = new ScriptResolver()
        {
            Shell = this.Shell,
            Extension = this.Extension,
            ScriptFormat = this.ScriptFormat,
            ArgumentList = this.CommandArgs,
        };
    }
}