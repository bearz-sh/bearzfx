using Bearz.Std;

namespace Bearz.Extensions.CliCommand.MkCert;

public class UninstallMkCertCaRootArgs : CommandArgsBuilder
{
    public override CommandArgs BuildArgs()
    {
        return new CommandArgs() { "-Uninstall" };
    }
}