using System;
using System.Management.Automation;

using Bearz.Secrets;

namespace Bearz.PowerShell.Standard;

public class AddMaskedSecretCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true, Mandatory = true)]
    public string[] InputObject { get; set; } = Array.Empty<string>();

    protected override void ProcessRecord()
    {
        foreach (var secret in this.InputObject)
        {
            SecretMasker.Default.Add(secret);
        }
    }
}