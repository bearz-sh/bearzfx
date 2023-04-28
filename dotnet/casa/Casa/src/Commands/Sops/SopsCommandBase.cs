using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

using Bearz.Casa.App;
using Bearz.Extensions.CliCommand;
using Bearz.Extensions.CliCommand.Sops;
using Bearz.Extra.Strings;
using Bearz.Std;

using Spectre.Console;

namespace Casa.Commands.Sops;

public abstract class SopsCommandBase : System.CommandLine.Command
{
    protected SopsCommandBase(string command, string? description)
        : base(command, description)
    {
        this.AddArgument(new Argument<string>(
            "input",
            "The input file to encrypt or decrypt"));

        this.AddOption(new Option<bool>(
            new[] { "--in-place", "-i" },
            "Write output back to the same file instead of stdout."));

        this.AddOption(new Option<bool>(
            new[] { "--azure-kv" },
            "Comma separated list of Azure Key Vault URLs [$SOPS_AZURE_KEYVAULT_URLS]"));

        this.AddOption(new Option<string?>(
            new[] { "--age", "-a" },
            "comma separated list of age recipients [$SOPS_AGE_RECIPIENTS]"));

        this.AddOption(new Option<string>(
            new[] { "--output", "-o" },
            "Save the output after encryption or decryption to the file specified"));

        this.AddOption(new Option<string?>(
            new[] { "--input-type" },
            "currently json, yaml, dotenv and binary are supported. If not set, sops will use the file's extension to determine the type"));

        this.AddOption(new Option<string?>(
            new[] { "--output-type" },
            "currently json, yaml, dotenv and binary are supported. If not set, sops will use the file's extension to determine the type"));

        this.AddOption(new Option<bool>(
            "--ignore-mac",
            "ignore Message Authentication Code during decryption"));

        this.AddOption(new Option<string?>(
            "--config",
            "path to sops' config file. If set, sops will not search for the config file recursively."));
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public abstract class SopsCommandBaseHandler : ICommandHandler
{
    private readonly string command;

    protected SopsCommandBaseHandler(string command)
    {
        this.command = command;
    }

    public string Input { get; set; } = string.Empty;

    public string? InputType { get; set; }

    public string? Output { get; set; }

    public string? OutputType { get; set; }

    public bool IgnoreMac { get; set; }

    public bool InPlace { get; set; }

    public string? Config { get; set; }

    public string? AzureKv { get; set; }

    public string? Age { get; set; }

    public string? Extract { get; set; }

    public int Invoke(InvocationContext context)
    {
        if (this.Input.IsNullOrWhiteSpace())
        {
            AnsiConsole.MarkupLine($"[red]hosts names or ips addresses for the certificate is required[/]");
            return 1;
        }

        var input = FsPath.Resolve(this.Input);

        if (!FsPath.Exists(input))
        {
            AnsiConsole.MarkupLine($"[red]Unable to find the file to encrypt: {input}[/]");
            return 1;
        }

        var cmd = SopsCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]sops not found on environment path[/]");
            return 1;
        }

        var ageKey = Path.Join(Paths.UserDataDirectory, "casa.age.key");
        if (Fs.FileExists(ageKey))
        {
            Bearz.Std.Env.Set("SOPS_AGE_KEY_FILE", ageKey);
        }

        var args = new CommandArgs()
        {
            this.command,
        };

        if (!this.InputType.IsNullOrWhiteSpace())
            args.Add("--input-type", this.InputType);

        var piped = false;
        if (!this.InPlace)
        {
            if (!this.Output.IsNullOrWhiteSpace())
            {
                args.Add("--output", this.Output);

                cmd.WithStdio(Stdio.Inherit);

                if (!this.OutputType.IsNullOrWhiteSpace())
                    args.Add("--output-type", this.OutputType);
            }
            else
            {
                cmd.WithStdio(Stdio.Piped);
                piped = true;
            }
        }
        else
        {
            args.Add("--in-place");
            piped = true;
        }

        if (this.IgnoreMac)
            args.Add("--ignore-mac");

        if (!this.Config.IsNullOrWhiteSpace())
            args.Add("--config", this.Config);

        if (!this.AzureKv.IsNullOrWhiteSpace())
            args.Add("--azure-kv", this.AzureKv);

        if (!this.Age.IsNullOrWhiteSpace())
            args.Add("--age", this.Age);

        if (!this.Extract.IsNullOrWhiteSpace())
            args.Add("--extract", this.Extract);

        if (piped)
        {
            cmd.RedirectTo(Console.Out);
            cmd.RedirectErrorTo(Console.Error);
        }

        return cmd.Output().ExitCode;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}