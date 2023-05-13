using System;
using System.Collections;
using System.Collections.Specialized;
using System.Management.Automation;

namespace Bearz.PowerShell.Yaml;
[Alias("read_yaml_file")]
[Cmdlet(VerbsCommunications.Read, "YamlFile")]
[OutputType(typeof(PSObject), typeof(Array), typeof(Hashtable), typeof(OrderedDictionary))]
public class ReadYamlFileCmdlet : PSCmdlet
{
    [Parameter]
    public string? File { get; set; }

    [Parameter]
    public SwitchParameter AsHashtable { get; set; }

    [Parameter]
    public SwitchParameter Merge { get; set; }

    [Parameter]
    public SwitchParameter All { get; set; }

    protected override void ProcessRecord()
    {
        if (this.File.IsNullOrWhiteSpace())
            throw new PSArgumentNullException(nameof(this.File));

        var content = System.IO.File.ReadAllText(this.File);

        var result = PsYamlReader.ReadYaml(
            content,
            this.Merge.ToBool(),
            this.All.ToBool(),
            this.AsHashtable.ToBool());

        this.WriteObject(result);
    }
}