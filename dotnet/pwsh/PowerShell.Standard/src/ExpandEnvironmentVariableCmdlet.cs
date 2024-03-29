using System;
using System.Management.Automation;

using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Alias("env_expand")]
[Cmdlet(VerbsData.Expand, "EnvironmentVariable")]
public class ExpandEnvironmentVariableCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    public string[] Template { get; set; } = Array.Empty<string>();

    protected override void ProcessRecord()
    {
        if (this.Template.Length == 0)
        {
            this.WriteObject(null);
            return;
        }

        foreach (var template in this.Template)
        {
            this.WriteObject(Env.Expand(template));
        }
    }
}