using Bearz.Std;

namespace Bearz.Cli.MkCert;

public class UninstallMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "-Uninstall" };
    }
}