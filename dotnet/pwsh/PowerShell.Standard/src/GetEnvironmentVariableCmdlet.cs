using System;
using System.Collections.Generic;
using System.Management.Automation;

using Bearz.Std;

namespace Bearz.PowerShell.Standard;

[Alias("env_get")]
[Cmdlet(VerbsCommon.Get, "EnvironmentVariable")]
[OutputType(typeof(string))]
public class GetEnvironmentVariableCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true)]
    public string[]? Name { get; set; }

    [Parameter(Position = 1)]
    public string? Default { get; set; } = null;

    [Parameter]
    public EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.Process;

    protected override void ProcessRecord()
    {
        if (this.Name is null)
        {
            if (Env.IsWindows())
            {
                var variables = Environment.GetEnvironmentVariables(this.Target);
                foreach (var key in variables.Keys)
                {
                    if (key is string name)
                    {
                        this.WriteObject(new KeyValuePair<string, object?>(name, variables[name]));
                    }
                }

                return;
            }

            if (this.Target == EnvironmentVariableTarget.Process)
            {
                var variables = Environment.GetEnvironmentVariables(this.Target);
                foreach (var key in variables.Keys)
                {
                    if (key is string name)
                    {
                        this.WriteObject(new KeyValuePair<string, object?>(name, variables[name]));
                    }
                }

                return;
            }

            var msg = "Non-Windows platforms only support setting process environment variables.";
            this.WriteError(new PlatformNotSupportedException(msg), this.Name);
        }
        else
        {
            if (Env.IsWindows())
            {
                foreach (var name in this.Name)
                {
                    var value = Env.Get(name, this.Target) ?? this.Default;
                    this.WriteObject(value);
                }

                return;
            }

            if (this.Target == EnvironmentVariableTarget.Process)
            {
                foreach (var name in this.Name)
                {
                    var value = Env.Get(name, this.Target) ?? this.Default;
                    this.WriteObject(value);
                }
            }

            var msg = "Non-Windows platforms only support setting process environment variables.";
            this.WriteError(new PlatformNotSupportedException(msg), this.Name);
        }
    }
}