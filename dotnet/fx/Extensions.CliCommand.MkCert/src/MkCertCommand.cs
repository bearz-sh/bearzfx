using Bearz.Std;

namespace Bearz.Extensions.CliCommand.MkCert;

public class MkCertCommand : CliCommand
{
    public MkCertCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("mkcert", context, startInfo)
    {
    }

    public static MkCertCommand Create()
        => new();

    public static MkCertCommand Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    public static void CreateNewCert(NewMkCertArgs args)
    {
        _ = new MkCertCommand()
            .WithArgs(args)
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static void InstallCaRoot()
    {
        _ = new MkCertCommand()
            .WithArgs("-Install")
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static void UninstallCaRoot()
    {
        _ = new MkCertCommand()
            .WithArgs("-Uninstall")
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static string GetCaRoot(GetMkCertCaRootArgs args)
    {
        var installLocation = new MkCertCommand()
            .WithArgs("-CAROOT")
            .WithStdio(Stdio.Piped)
            .Output()
            .ThrowOnInvalidExitCode()
            .ReadFirstLine();

        return installLocation;
    }
}