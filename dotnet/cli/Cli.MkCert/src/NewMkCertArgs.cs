using Bearz.Std;

using FluentBuilder;

namespace Bearz.Extensions.CliCommand.MkCert;

[AutoGenerateBuilder]
public class NewMkCertArgs : ReflectionCommandArgsBuilder
{
    public List<string> Hosts { get; set; } = new List<string>();

    public string? CertFile { get; set; }

    public string? KeyFile { get; set; }

    public string? P12File { get; set; }

    public bool Client { get; set; }

    public bool Ecdsa { get; set; }

    public string? Csr { get; set; }

    protected override string OptionPrefix => "-";

    public override CommandArgs BuildArgs()
    {
        if (this.Hosts.Count > 0)
            this.TrailingArguments.AddRange(this.Hosts);

        return base.BuildArgs();
    }
}