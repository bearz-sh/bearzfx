using Bearz.Std;

namespace Bearz.Cli.MkCert;

public class MkCertCli : CliCommand
{
    public MkCertCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("mkcert", context, startInfo)
    {
    }

    public static MkCertCli Create()
        => new();

    public static MkCertCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    public static void CreateNewCert(NewMkCertArgs args)
    {
        _ = new MkCertCli()
            .WithArgs(args)
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static void InstallCaRoot()
    {
        _ = new MkCertCli()
            .WithArgs("-Install")
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static void UninstallCaRoot()
    {
        _ = new MkCertCli()
            .WithArgs("-Uninstall")
            .WithStdio(Stdio.Inherit)
            .Output()
            .ThrowOnInvalidExitCode();
    }

    public static string GetCaRoot(GetMkCertCaRootArgs args)
    {
        var installLocation = new MkCertCli()
            .WithArgs("-CAROOT")
            .WithStdio(Stdio.Piped)
            .Output()
            .ThrowOnInvalidExitCode()
            .ReadFirstLine();

        return installLocation;
    }
}