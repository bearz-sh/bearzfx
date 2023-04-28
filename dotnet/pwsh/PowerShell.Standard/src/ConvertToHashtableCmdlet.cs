using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Ze.PowerShell.Core;

[Alias("as-hashtable", "to_hashtable")]
[Cmdlet(VerbsData.ConvertTo, "ZeHashtable")]
[OutputType(typeof(void))]
public class ConvertToHashtableCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true)]
    public object? InputObject { get; set; }

    public static object? Convert(object? input)
    {
        if (input is null || input is string)
        {
            return input;
        }

        if (input is PSObject psObject)
        {
            var result = new Hashtable();
            foreach (var property in psObject.Properties)
            {
                result.Add(property.Name, Convert(property.Value));
            }

            return result;
        }

        if (input is IEnumerable enumerable)
        {
            var result = new List<object?>();
            foreach (var next in enumerable)
            {
                result.Add(Convert(next));
            }

            return result.ToArray();
        }

        return input;
    }

    protected override void ProcessRecord()
    {
        var result = Convert(this.InputObject);
        this.WriteObject(result);
    }
}