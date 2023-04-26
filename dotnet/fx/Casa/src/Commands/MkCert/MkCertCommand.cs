using System.CommandLine;

using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.MkCert;

[CommandHandler(typeof(NewCommandHandler))]
public class MkCertCommand : Command
{
    public MkCertCommand()
        : base("mkcert", "casa commands to run the mkcert command line tool in a more intuitive way")
    {
        this.AddArgument(new Argument<string[]>("hosts", "host names or ips addresses for the certificate"));
        this.AddOption(new Option<bool>(new[] { "--ecdsa", "-e" }, "generate an ECDSA key and certificate"));
        this.AddOption(new Option<bool>(new[] { "--pkcs12", "-p" }, "generate a PKCS#12 file (.p12/.pfx)"));
        this.AddOption(new Option<bool>(new[] { "--client" }, "generate a certificate for client authentication"));
        this.AddOption(new Option<string>(new[] { "--cert-file", "-c" }, "path to the certificate file"));
        this.AddOption(new Option<string>(new[] { "--key-file", "-k" }, "path to the key file"));
        this.AddOption(new Option<string>(new[] { "--csr" }, "path to the certificate signing request file"));
        this.AddCommand(new NewCommand());
        this.AddCommand(new GetCaRootCommand());
        this.AddCommand(new InstallCommand());
        this.AddCommand(new UninstallCommand());
    }
}