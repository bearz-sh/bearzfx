using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;

using Bearz.Extra.Strings;
using Bearz.Secrets;

namespace Bearz.PowerShell.Standard;

[Cmdlet(VerbsCommon.New, "Secret")]
[OutputType(typeof(string), typeof(char[]), typeof(SecureString), typeof(byte[]))]
public class NewSecretCmdlet : PSCmdlet
{
    [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Character")]
    [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "CharacterList")]
    public int Length { get; set; } = 16;

    [Parameter(Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName = "Characters")]
    public string? Character { get; set; }

    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "CharacterList")]
    public IEnumerable<char>? CharacterList { get; set; }

    [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
    public ScriptBlock? Validator { get; set; } = null!;

    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "Character")]
    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "CharacterList")]
    public SwitchParameter AsString { get; set; }

    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "Character")]
    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "CharacterList")]
    public SwitchParameter AsBytes { get; set; }

    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "Character")]
    [Parameter(ValueFromPipelineByPropertyName = true, ParameterSetName = "CharacterList")]
    public SwitchParameter AsSecureString { get; set; }

    protected override void ProcessRecord()
    {
        var pg = new SecretGenerator();

        if (this.CharacterList is not null)
        {
            var list = this.CharacterList.ToList();
            if (list.Count == 0)
            {
                pg.AddDefaults();
            }
            else
            {
                pg.Add(list);
            }
        }
        else if (!this.Character.IsNullOrWhiteSpace())
        {
            pg.Add(this.Character);
        }
        else
        {
            pg.AddDefaults();
        }

        if (this.Validator is not null)
        {
            pg.SetValidator((chars) =>
            {
                var r = this.Validator.Invoke(chars);
                if (r.Count == 1)
                {
                    return r[0].BaseObject is bool and true;
                }

                return false;
            });
        }

        if (this.AsString.ToBool())
        {
            this.WriteObject(pg.GenerateAsString(this.Length));
            return;
        }

        if (this.AsBytes.ToBool())
        {
            var bytes = pg.GenerateAsBytes(this.Length);
            this.WriteObject(bytes, false);
            return;
        }

        if (this.AsSecureString.ToBool())
        {
            this.WriteObject(pg.GenerateAsSecureString(this.Length));
            return;
        }

        var secret = pg.Generate(this.Length);
        this.WriteObject(secret, false);
    }
}