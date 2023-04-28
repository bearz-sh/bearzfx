namespace Bearz.Extensions.CliCommand.MkCert;

public class MkCertArgs : ReflectionCommandArgsBuilder
{
    public bool Install { get; set; }

    public bool Uninstall { get; set; }

    public List<string> Hosts { get; set; } = new List<string>();

    public string? CertFile { get; set; }

    public string? KeyFile { get; set; }

    public string? P12File { get; set; }

    public string? Client { get; set; }

    public bool Ecdsa { get; set; }

    public string? Csr { get; set; }

    protected override string OptionPrefix => "-";
}