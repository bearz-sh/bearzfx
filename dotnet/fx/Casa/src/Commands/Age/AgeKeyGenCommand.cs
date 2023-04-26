using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Casa.App;
using Bearz.Extensions.CliCommand;
using Bearz.Extra.Strings;
using Bearz.Std;

using Spectre.Console;

using AgeKeyGenCmd = Bearz.Extensions.CliCommand.Age.AgeKeyGenCommand;
using Command = System.CommandLine.Command;

namespace Casa.Commands.Age;

public class AgeKeygenCommand : Command
{
    public AgeKeygenCommand()
        : base("keygen", "Generate a new age keypair")
    {
        this.AddArgument(new Argument<string>("Input", "The key file input. This is only valid with the --show or -y flag"));
        this.AddOption(new Option<bool>(new[] { "--show", "-y" }, "Reads an age key and returns the public key"));
        this.AddOption(new Option<string?>(new[] { "--output", "-o" }, "The file to write the keypair to"));
    }
}

public class AgeKeygenCommandHandler : ICommandHandler
{
    public string? Output { get; set; }

    public string? Input { get; set; }

    public bool Show { get; set; }

    public int Invoke(InvocationContext context)
    {
        var cmd = new AgeKeyGenCmd();
        var path = cmd.Which();
        if (path.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine($"[red]age-keygen not found on environment path[/]");
            return 1;
        }

        // write new key to std out
        if (!this.Show && this.Output.IsNullOrWhiteSpace())
        {
            var result = cmd
                .Output()
                .ThrowOnInvalidExitCode();

            return result.ExitCode;
        }

        var args = new CommandArgs();
        if (!string.IsNullOrWhiteSpace(this.Output))
        {
            args.Add("--output");
            args.Add(this.Output);
        }
        else if (this.Show)
        {
            if (this.Input.IsNullOrWhiteSpace())
            {
                AnsiConsole.MarkupLine("[red]Input file is required when using the --show or -y flag[/]");
                return 1;
            }

            args.Add("-y", this.Input);
        }

        var r = cmd
            .WithStdio(Stdio.Inherit)
            .Output();

        return r.ExitCode;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}