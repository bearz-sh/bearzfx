using System;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Text.Json;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsCommon.Get, "ModuleConfig")]
[OutputType(typeof(Hashtable), typeof(PSObject), typeof(Array[]))]
public class GetModuleConfigCmdlet : PSCmdlet
{
    [Parameter(Position = 1)]
    public string? ModuleName { get; set; }

    [Parameter(Position = 0)]
    public string? Query { get; set; }

    protected override void ProcessRecord()
    {
        var moduleName = this.ModuleName ?? this.MyInvocation.MyCommand.ModuleName;
        var config = PsModuleConfig.ReadModuleConfig(moduleName, this.Query);
        this.WriteObject(config);
    }
}