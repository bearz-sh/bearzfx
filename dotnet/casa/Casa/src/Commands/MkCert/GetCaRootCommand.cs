using System.CommandLine.Invocation;

using Bearz.Extensions.CliCommand;
using Bearz.Extensions.CliCommand.MkCert;
using Bearz.Std;

using Spectre.Console;

using Command = System.CommandLine.Command;

namespace Casa.Commands.MkCert;

public class GetCaRootCommand : Command
{
    public GetCaRootCommand()
        : base("caroot", "Get the file path of the root MkCert certificate authority certificate")
    {
    }
}

public class GetCaRootCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        var cmd = MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        var result = cmd.WithArgs("-CAROOT")
            .WithStdio(Stdio.Piped)
            .Output();

        if (result.ExitCode != 0)
        {
            foreach (var line in result.StdOut)
            {
                AnsiConsole.MarkupLine($"[red]{line}[/]");
            }

            foreach (var line in result.StdErr)
            {
                AnsiConsole.MarkupLine($"[red]{line}[/]");
            }

            return result.ExitCode;
        }

        Console.WriteLine(result.ReadFirstLine());
        return 0;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var cmd = MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        var cts = context.GetCancellationToken();
        var result = await cmd.WithArgs("-CAROOT")
            .WithStdio(Stdio.Piped)
            .OutputAsync(cts);

        if (result.ExitCode != 0)
        {
            foreach (var line in result.StdOut)
            {
                AnsiConsole.MarkupLine($"[red]{line}[/]");
            }

            foreach (var line in result.StdErr)
            {
                AnsiConsole.MarkupLine($"[red]{line}[/]");
            }

            return result.ExitCode;
        }

        Console.WriteLine(result.ReadFirstLine());
        return 0;
    }
}