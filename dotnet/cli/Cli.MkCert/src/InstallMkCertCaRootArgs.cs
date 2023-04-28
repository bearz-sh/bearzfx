using Bearz.Std;

namespace Bearz.Extensions.CliCommand.MkCert;

public class InstallMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "-Install" };
    }
}