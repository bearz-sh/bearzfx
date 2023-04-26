using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

using Bearz.Extensions.CliCommand;
using Bearz.Extra.Strings;
using Bearz.Std;

using Spectre.Console;

using AgeCmd = Bearz.Extensions.CliCommand.Age.AgeCommand;

namespace Casa.Commands.Age;

#pragma warning disable SA1200 // needed to override the Casa.Commands.Env namespace,
using Env = Bearz.Std.Env;
#pragma warning restore SA1200

public class AgeEncryptCommand : AgeBaseCommand
{
    public AgeEncryptCommand()
        : base("encrypt", "Encrypt the input file")
    {
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class AgeEncryptCommandHandler : ICommandHandler
{
    public string Input { get; set; } = string.Empty;

    public string? Output { get; set; }

    public bool Armor { get; set; }

    public string[]? Recipient { get; set; }

    public string[]? RecipientFile { get; set; }

    public bool Passphrase { get; set; }

    public string[]? Identity { get; set; }

    public int Invoke(InvocationContext context)
    {
        var cmd = new AgeCmd();
        var path = cmd.Which();
        if (path.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine($"[red]age not found on environment path[/]");
            return 1;
        }

        var args = new CommandArgs()
        {
            "--encrypt",
        };

        if (this.Armor)
            args.Add("--armor");

        if (this.Passphrase)
            args.Add("--passphrase");

        if (this.Recipient is not null)
        {
            foreach (var r in this.Recipient)
            {
                args.Add("--recipient", r);
            }
        }
        else if (Env.Get("AGE_RECIPIENT_KEY") is not null)
        {
            args.Add("--recipient", Env.Get("AGE_RECIPIENT_KEY")!);
        }

        if (this.RecipientFile is not null)
        {
            foreach (var r in this.RecipientFile)
                args.Add("--recipient-file", r);
        }
        else if (Env.Get("AGE_RECIPIENT_FILE") is not null)
        {
            args.Add("--recipient-file", Env.Get("AGE_RECIPIENT_FILE")!);
        }

        if (this.Identity is not null)
        {
            foreach (var r in this.Identity)
                args.Add("--identity", r);
        }

        if (!string.IsNullOrWhiteSpace(this.Output))
            args.Add("--output", this.Output);

        args.Add(this.Input);

        var result = cmd.WithArgs(args)
            .WithStdio(Stdio.Inherit)
            .Output();

        return result.ExitCode;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}