using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

using Bearz.Extensions.CliCommand;
using Bearz.Extensions.CliCommand.MkCert;
using Bearz.Extra.Strings;
using Bearz.Std;

using Spectre.Console;

using Command = System.CommandLine.Command;

namespace Casa.Commands.MkCert;

public class NewCommand : Command
{
    public NewCommand()
        : base("new", "create a new certificate")
    {
        this.AddArgument(new Argument<string[]>("hosts", "host names or ips addresses for the certificate"));
        this.AddOption(new Option<bool>(new[] { "--ecdsa", "-e" }, "generate an ECDSA key and certificate"));
        this.AddOption(new Option<bool>(new[] { "--pkcs12", "-p" }, "generate a PKCS#12 file (.p12/.pfx)"));
        this.AddOption(new Option<bool>(new[] { "--client" }, "generate a certificate for client authentication"));
        this.AddOption(new Option<string>(new[] { "--cert-file", "-c" }, "path to the certificate file"));
        this.AddOption(new Option<string>(new[] { "--key-file", "-k" }, "path to the key file"));
        this.AddOption(new Option<string>(new[] { "--csr" }, "path to the certificate signing request file"));
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class NewCommandHandler : ICommandHandler
{
    public bool Ecdsa { get; set; }

    public string[] Hosts { get; set; } = Array.Empty<string>();

    public string? CertFile { get; set; }

    public string? KeyFile { get; set; }

    public string? Csr { get; set; }

    public bool Pkcs12 { get; set; }

    public bool Client { get; set; }

    public int Invoke(InvocationContext context)
    {
        if (this.Hosts.Length == 0)
        {
            AnsiConsole.MarkupLine($"[red]hosts names or ips addresses for the certificate is required[/]");
            return 1;
        }

        var cmd = MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        if (this.Client)
            cmd.WithArgs("-client");

        if (this.Pkcs12)
            cmd.WithArgs("-pkcs12");

        if (this.Ecdsa)
            cmd.WithArgs("-ecdsa");

        if (!this.CertFile.IsNullOrWhiteSpace())
            cmd.WithArgs("-cert-file", this.CertFile);

        if (!this.KeyFile.IsNullOrWhiteSpace())
            cmd.WithArgs("-key-file", this.KeyFile);

        if (!this.Csr.IsNullOrWhiteSpace())
            cmd.WithArgs("-csr", this.Csr);

        cmd.WithArgs(this.Hosts)
            .WithStdio(Stdio.Inherit);

        return cmd.Output().ExitCode;
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        if (this.Hosts.Length == 0)
        {
            AnsiConsole.MarkupLine($"[red]hosts names or ips addresses for the certificate is required[/]");
            return 1;
        }

        var cmd = MkCertCli.Create();
        var path = cmd.Which();
        if (string.IsNullOrWhiteSpace(path))
        {
            AnsiConsole.MarkupLine($"[red]mkcert not found on environment path[/]");
            return 1;
        }

        if (this.Client)
            cmd.WithArgs("-client");

        if (this.Pkcs12)
            cmd.WithArgs("-pkcs12");

        if (this.Ecdsa)
            cmd.WithArgs("-ecdsa");

        if (!this.CertFile.IsNullOrWhiteSpace())
            cmd.WithArgs("-cert-file", this.CertFile);

        if (!this.KeyFile.IsNullOrWhiteSpace())
            cmd.WithArgs("-key-file", this.KeyFile);

        if (!this.Csr.IsNullOrWhiteSpace())
            cmd.WithArgs("-csr", this.Csr);

        cmd.WithArgs(this.Hosts)
            .WithStdio(Stdio.Inherit);

        var result = await cmd.OutputAsync(context.GetCancellationToken());
        return result.ExitCode;
    }
}