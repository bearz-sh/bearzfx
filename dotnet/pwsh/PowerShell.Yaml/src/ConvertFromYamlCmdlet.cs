using System;
using System.Management.Automation;

namespace Bearz.PowerShell.Yaml;

[Alias("from_yaml")]
[Cmdlet(VerbsData.ConvertFrom, "Yaml")]
[OutputType(typeof(PSObject), typeof(Array))]
public class ConvertFromYamlCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true)]
    public string? InputObject { get; set; }

    [Parameter]
    public SwitchParameter AsHashtable { get; set; }

    [Parameter]
    public SwitchParameter Merge { get; set; }

    [Parameter]
    public SwitchParameter All { get; set; }

    protected override void ProcessRecord()
    {
        if (string.IsNullOrWhiteSpace(this.InputObject))
        {
            this.WriteObject(null);
            return;
        }

        var result = PsYamlReader.ReadYaml(
            this.InputObject,
            this.Merge.ToBool(),
            this.All.ToBool(),
            this.AsHashtable.ToBool());
        this.WriteObject(result);
    }
}