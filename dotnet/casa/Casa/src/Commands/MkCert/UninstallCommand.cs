using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Cli;
using Bearz.Cli.MkCert;
using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.MkCert;

[CommandHandler(typeof(UninstallCommandHandler))]
public class UninstallCommand : Command
{
    public UninstallCommand()
        : base("install", "Install the mkcert certificate authority")
    {
    }
}

public class UninstallCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        var result = MkCertCli.Create()
            .WithArgs("-Uninstall")
            .WithStdio(Bearz.Std.Stdio.Inherit)
            .Output();

        return result.ExitCode;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var cts = context.GetCancellationToken();
        var cmd = MkCertCli.Create()
            .WithArgs("-Uninstall")
            .WithStdio(Bearz.Std.Stdio.Inherit);

        var result = await cmd
            .OutputAsync(cts)
            .ConfigureAwait(false);

        return result.ExitCode;
    }
}