using System.IO;
using System.Management.Automation;

using Bearz.Extra.YamlDotNet;
using Bearz.Text.Yaml;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeResolvers;

namespace Bearz.PowerShell.Yaml;

[Alias("to_yaml")]
[Cmdlet(VerbsData.ConvertTo, "Yaml")]
[OutputType(typeof(string))]
public class ConvertToYamlCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true)]
    public object? InputObject { get; set; }

    [Parameter]
    public SerializationOptions Option { get; set; } = SerializationOptions.Roundtrip;

    protected override void ProcessRecord()
    {
        using var writer = new StringWriter();

        PsYamlWriter.WriteYaml(this.InputObject, this.Option, writer);

        this.WriteObject(writer.ToString());
    }
}