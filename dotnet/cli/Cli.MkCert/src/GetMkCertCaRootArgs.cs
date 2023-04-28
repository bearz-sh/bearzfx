using Bearz.Std;

namespace Bearz.Cli.MkCert;

public class GetMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "--CAROOT", };
    }
}