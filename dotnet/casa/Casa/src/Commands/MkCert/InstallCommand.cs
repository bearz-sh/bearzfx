using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.CliCommand;
using Bearz.Extensions.Hosting.CommandLine;

using Spectre.Console;

namespace Casa.Commands.MkCert;

[CommandHandler(typeof(InstallCommandHandler))]
public class InstallCommand : Command
{
    public InstallCommand()
        : base("install", "Install the mkcert certificate authority")
    {
    }
}

public class InstallCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        var cmd = Bearz.Extensions.CliCommand.MkCert.MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        var result = cmd
            .WithArgs("-Install")
            .WithStdio(Bearz.Std.Stdio.Inherit)
            .Output();

        return result.ExitCode;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var cmd = Bearz.Extensions.CliCommand.MkCert.MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        var cts = context.GetCancellationToken();
        cmd
            .WithArgs("-Install")
            .WithStdio(Bearz.Std.Stdio.Inherit);

        var result = await cmd
            .OutputAsync(cts)
            .ConfigureAwait(false);

        return result.ExitCode;
    }
}