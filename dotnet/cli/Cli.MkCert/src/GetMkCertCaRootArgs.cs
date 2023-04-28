using Bearz.Std;

namespace Bearz.Extensions.CliCommand.MkCert;

public class GetMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "--CAROOT", };
    }
}