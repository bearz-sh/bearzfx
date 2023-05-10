using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Plank.Package;
using Plank.Package.Actions;

using Command = System.CommandLine.Command;

namespace Plank.Commands.Compose;

[CommandHandler(typeof(UninstallCommandHandler))]
public class UninstallCommand : Command
{
    public UninstallCommand()
        : base("uninstall", "uninstalls a plank compose app")
    {
        this.AddArgument(new Argument<string>("app", "the app to uninstall"));
        this.AddOption(new Option<bool>(new[] { "--force", "-f" }, "force overwrite"));
        this.AddOption(new Option<string?[]>(new[] { "--var-file" }, "variable file to use"));
    }
}

public class UninstallCommandHandler : AppCommandHandlerBase
{
    private readonly ILogger log;

    public UninstallCommandHandler(IConfiguration config, ILogger<UninstallCommandHandler> logger)
        : base(config)
    {
        this.log = logger;
    }

    public override Task<int> InvokeAsync(InvocationContext context)
    {
        try
        {
            var package = new PlankExtractedPackage(
                this.GetPackageDirectory(),
                this.PathSpec,
                this.Target,
                this.VarFile);

            var composeFile = FsPath.Combine(this.PathSpec.ComposeDir, package.Spec.Name, "compose.yml");
            var serviceName = package.Variables["name"]?.ToString() ?? package.Spec.Name;
            if (!FsPath.Exists(composeFile))
            {
                this.log.LogWarning("Unable to find compose file for app {App} at {ComposeFile}", serviceName, composeFile);
                return Task.FromResult(0);
            }

            if (!Fs.FileExists(composeFile))
                throw new FileNotFoundException($"Unable to find compose file for app {this.App} at {composeFile}");

            var result1 = Env.Process.CreateCommand("docker")
                .WithArgs("compose", "ls", "--quiet", "--filter", $"name={serviceName}")
                .WithStdio(Stdio.Piped)
                .Output();

            result1.ThrowOnInvalidExitCode();

            if (result1.StdOut.Count == 0)
                return Task.FromResult(0);

            if (result1.ReadFirstLine().Trim().EqualsIgnoreCase(serviceName))
            {
                DotEnvFile.LoadRelevantVariables(new[] { package.GlobalEnvFile, package.EnvFile }, package.Secrets);
                var args = new CommandArgs()
                {
                    "compose",
                    "--project-name",
                    (package.Variables["name"]?.ToString() ?? package.Spec.Name),
                    "--file",
                    composeFile,
                    "down",
                };

                var result = Env.Process.CreateCommand("docker")
                    .WithArgs(args)
                    .WithStdio(Stdio.Inherit)
                    .WithCwd(FsPath.GetDirectoryName(composeFile)!)
                    .Output();

                return Task.FromResult(result.ExitCode);
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, ex.Message);
            return Task.FromResult(1);
        }
    }

    public override int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }
}