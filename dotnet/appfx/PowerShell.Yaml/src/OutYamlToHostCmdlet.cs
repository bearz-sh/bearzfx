using System;
using System.IO;
using System.Management.Automation;

using Bearz.Text.Yaml;

namespace Bearz.PowerShell.Yaml;

[Alias("pipe_yaml_to_host")]
[Cmdlet(VerbsData.Out, "YamlToHost")]
[OutputType(typeof(void))]
public class OutYamlToHostCmdlet : PSCmdlet
{
    [Parameter(ValueFromPipeline = true)]
    public object? InputObject { get; set; }

    [Parameter]
    public SerializationOptions Option { get; set; } = SerializationOptions.Roundtrip;

    [Parameter]
    public SwitchParameter AsConsole { get; set; }

    protected override void ProcessRecord()
    {
        using var sw = new StringWriter();
        PsYamlWriter.WriteYaml(this.InputObject, this.Option, sw);
        if (this.AsConsole)
        {
            Console.WriteLine(sw.ToString());
            return;
        }

        if (this.IsDebug(false))
        {
            if (this.InputObject is null)
            {
                this.WriteDebug(null);
                return;
            }

            this.WriteDebug(sw.ToString());
            return;
        }

        if (this.IsVerbose(false))
        {
            if (this.InputObject is null)
            {
                this.WriteVerbose(null);
                return;
            }

            this.WriteVerbose(sw.ToString());
            return;
        }

        using var ps = Pwsh.Create(RunspaceMode.CurrentRunspace);
        var commandInfo = this.GetCmdlet("Get-Command");
        ps.AddCommand(commandInfo);
        ps.AddParameter("Object", sw.ToString());
        ps.Invoke();
    }
}