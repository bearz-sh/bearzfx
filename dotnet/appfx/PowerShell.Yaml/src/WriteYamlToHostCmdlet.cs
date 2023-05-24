using System;
using System.IO;
using System.Management.Automation;

using Bearz.Text.Yaml;

namespace Bearz.PowerShell.Yaml;

[Alias("write_yaml_to_host")]
[Cmdlet(VerbsCommunications.Write, "YamlToHost")]
public class WriteYamlToHostCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public object? InputObject { get; set; }

    [Parameter]
    public SerializationOptions Option { get; set; } = SerializationOptions.Roundtrip;

    [Parameter]
    public SwitchParameter AsConsole { get; set; }

    protected override void ProcessRecord()
    {
        using var writer = new StringWriter();
        PsYamlWriter.WriteYaml(this.InputObject, this.Option, writer);

        if (this.AsConsole)
        {
            Console.WriteLine(writer.ToString());
            return;
        }

        if (this.IsDebug(false))
        {
            if (this.InputObject is null)
            {
                this.WriteDebug(null);
                return;
            }

            this.WriteDebug(writer.ToString());
            return;
        }

        if (this.IsVerbose(false))
        {
            if (this.InputObject is null)
            {
                this.WriteVerbose(null);
                return;
            }

            this.WriteVerbose(writer.ToString());
            return;
        }

        using var ps = Pwsh.Create(RunspaceMode.CurrentRunspace);
        var commandInfo = this.GetCmdlet("Get-Command");
        ps.AddCommand(commandInfo);
        ps.AddParameter("Object", writer.ToString());
        ps.Invoke();
    }
}