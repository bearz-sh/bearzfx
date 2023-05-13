using System.Management.Automation;

using Bearz.Text.Yaml;

namespace Bearz.PowerShell.Yaml;

[Alias("write_yaml_file")]
[Cmdlet(VerbsCommunications.Write, "YamlFile")]
[OutputType(typeof(void))]
public class WriteYamlFileCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
    public object? InputObject { get; set; }

    [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
    public string? File { get; set; }

    public SerializationOptions Option { get; set; } = SerializationOptions.Roundtrip;

    protected override void ProcessRecord()
    {
        if (this.File.IsNullOrWhiteSpace())
            throw new PSArgumentNullException(nameof(this.File));

        if (this.InputObject is null)
        {
            System.IO.File.WriteAllText(this.File, string.Empty);
            return;
        }

        using var sw = System.IO.File.CreateText(this.File);
        PsYamlWriter.WriteYaml(this.InputObject, this.Option, sw);
        sw.Flush();
    }
}