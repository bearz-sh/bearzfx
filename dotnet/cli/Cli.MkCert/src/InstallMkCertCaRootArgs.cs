using Bearz.Std;

namespace Bearz.Cli.MkCert;

public class InstallMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "-Install" };
    }
}