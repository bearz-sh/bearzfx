using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Object;
using Bearz.Std;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Plank.Package;
using Plank.Package.Actions;

using Command = System.CommandLine.Command;

namespace Plank.Commands.Compose;

[CommandHandler(typeof(InstallCommandHandler))]
public class InstallCommand : Command
{
    public InstallCommand()
        : base("install", "installs a plank compose app")
    {
        this.AddArgument(new Argument<string>("app", "the app to install"));
        this.AddOption(new Option<bool>(new[] { "--force", "-f" }, "force overwrite"));
        this.AddOption(new Option<string?[]>(new[] { "--var-file" }, "variable file to use"));
    }
}

public class InstallCommandHandler : AppCommandHandlerBase
{
    private readonly ILogger log;

    public InstallCommandHandler(IConfiguration config, ILogger<InstallCommandHandler> log)
        : base(config)
    {
        this.log = log;
    }

    public override int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
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

            if (!FsPath.Exists(composeFile) || this.Force)
            {
                var installTask = new InstallAction();
                installTask.Run(package, this.Force);
            }

            if (!Fs.FileExists(composeFile))
                throw new FileNotFoundException($"Unable to find compose file for app {this.App} at {composeFile}");

            DotEnvFile.LoadRelevantVariables(new[] { package.GlobalEnvFile, package.EnvFile }, package.Secrets);
            var args = new CommandArgs()
            {
                "--project-name",
                (package.Variables["name"]?.ToString() ?? package.Spec.Name),
                "--file",
                composeFile,
                "--ansi",
                "always",
                "up",
                "-d",
            };

            Env.Set("DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION", "true");
            Env.Set("DOTNET_CONSOLE_ANSI_COLOR", "true");
            Console.WriteLine(args.ToString());

            var result = Env.Process.CreateCommand("/usr/libexec/docker/cli-plugins/docker-compose")
                .WithArgs(args)
                .WithStdio(Stdio.Inherit)
                .RedirectTo((line, _) => Console.WriteLine(line))
                .RedirectErrorTo((line, _) => Console.WriteLine(line))
                .WithCwd(FsPath.GetDirectoryName(composeFile)!)
                .Output();

            return Task.FromResult(result.ExitCode);
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, ex.Message);
            return Task.FromResult(1);
        }
    }
}