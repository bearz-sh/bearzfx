using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz;
using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text.DotEnv;
using Bearz.Text.DotEnv.Document;

using Microsoft.Extensions.Configuration;

using Command = System.CommandLine.Command;

namespace Casa.Commands.Compose;

[CommandHandler(typeof(ComposeCommandHandler))]
public class ComposeCommand : Command
{
    public ComposeCommand()
        : base("compose", "Compose the environment")
    {
        this.AddCommand(new EvaluateCommand());
        this.AddGlobalOption(new Option<string>(
            new[] { "--ansi" },
            "Control when to print ANSI control characters (\"never\"|\"always\"|\"auto\") (default \"auto\")"));

        this.AddGlobalOption(new Option<bool>(
            new[] { "--compatibility" },
            "Run compose in backward compatibility mode"));

        this.AddGlobalOption(new Option<string[]>(
            new[] { "--env-file" },
            "Specify an alternate environment file."));

        this.AddGlobalOption(new Option<string[]>(
            new[] { "--file", "-f" },
            "Compose configuration files"));

        this.AddGlobalOption(new Option<int>(
            new[] { "--parallel" },
            "Control max parallelism, -1 for unlimited (default -1)"));

        this.AddGlobalOption(new Option<string[]>(
            new[] { "--profile" },
            "Specify a profile to enable"));

        this.AddGlobalOption(new Option<string>(
            new[] { "--project-directory" },
            "Specify an alternate working directory (default: the path of the, first specified, Compose file)"));

        this.AddGlobalOption(new Option<string>(
            new[] { "--project-name", "-p" },
            "Project name"));

        this.AddOption(new Option<string?>(new[] { "--env" }, "The environment to evaluate"));
    }
}

public abstract class ComposeCommandBaseHandler : ICommandHandler
{
    protected ComposeCommandBaseHandler(IConfiguration configuration)
    {
        this.Config = configuration;
        this.Command = Bearz.Std.Env.Process.CreateCommand("docker");
    }

    public string? Ansi { get; set; }

    public bool Compatibility { get; set; }

    public string[]? EnvFile { get; set; }

    public string[]? File { get; set; }

    public string? Env { get; set; }

    public int Parallel { get; set; }

    public string[]? Profile { get; set; }

    public string? ProjectDirectory { get; set; }

    public string? ProjectName { get; set; }

    public bool NoOverride { get; set; }

    protected IConfiguration Config { get; set; }

    protected Bearz.Std.Command Command { get; set; }

    public abstract int Invoke(InvocationContext context);

    public virtual Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));

    protected void BuildComposeArgs()
    {
        var args = this.Command.StartInfo.Args;
        args.Add("compose");
        if (!Bearz.Std.Env.IsWindows() && !Bearz.Std.Env.IsUserElevated)
        {
            this.Command.WithVerb("sudo");
        }

        var cmd = this.Command;
        var envName = this.Env ?? this.Config.GetValue<string?>("env:default") ?? "default";
        var dir = this.ProjectDirectory.IsNullOrWhiteSpace() ? Bearz.Std.Env.Cwd : this.ProjectDirectory;

        if (!this.ProjectName.IsNullOrWhiteSpace())
            args.Add("--project-name", this.ProjectName);

        if (!this.ProjectDirectory.IsNullOrWhiteSpace())
            args.Add("--project-directory", this.ProjectDirectory);

        var envValues = new Dictionary<string, string?>();
        if (this.EnvFile?.Length > 0)
        {
            foreach (var file in this.EnvFile)
            {
                var envText = Fs.ReadTextFile(file);
                var env = DotEnvSerializer.Deserialize<EnvDocument>(envText);
                if (env is not null)
                {
                    foreach (var (key, value) in env.ToDictionary())
                        envValues[key] = value;
                }
            }

            cmd.WithEnv(envValues);
        }

        if (!this.Ansi.IsNullOrWhiteSpace())
            args.Add("--ansi", this.Ansi);

        if (this.Compatibility)
            args.Add("--compatibility");

        if (this.Profile?.Length > 0)
        {
            foreach (var profile in this.Profile)
                args.Add("--profile", profile);
        }

        if (this.Parallel > 0)
            args.Add("--parallel", this.Parallel.ToString());

        if (this.File?.Length > 0)
        {
            foreach (var file in this.File)
            {
                args.Add("--file", file);
            }
        }
        else
        {
            var defaultComposeFiles = new[]
            {
                Path.Join(dir, "compose.yaml"),
                Path.Join(dir, "compose.yml"),
                Path.Join(dir, "docker-compose.yaml"),
                Path.Join(dir, "docker-compose.yml"),
            };

            var composeFiles = new List<string>()
            {
                Path.Join(dir, $"{envName}.compose.yaml"),
                Path.Join(dir, $"{envName}.compose.yml"),
                Path.Join(dir, $"{envName}.docker-compose.yaml"),
                Path.Join(dir, $"{envName}.docker-compose.yml"),
            };

            var defaultComposeFile = defaultComposeFiles.FirstOrDefault(Fs.FileExists);
            var composeFile = composeFiles.FirstOrDefault(Fs.FileExists);

            if (!this.NoOverride)
            {
                if (!composeFile.IsNullOrWhiteSpace())
                    args.Add("--file", composeFile);
                else if (!defaultComposeFile.IsNullOrWhiteSpace())
                    args.Add("--file", defaultComposeFile);
            }
            else
            {
                if (!composeFile.IsNullOrWhiteSpace())
                    args.Add("--file", composeFile);
                if (!defaultComposeFile.IsNullOrWhiteSpace())
                    args.Add("--file", defaultComposeFile);
            }
        }

        if (this.EnvFile?.Length == 0)
        {
            var defaultEnvFile = Path.Combine(dir, ".env");
            var envFile = Path.Join(dir, $"{envName}.env");
            if (!this.NoOverride)
            {
                if (!envFile.IsNullOrWhiteSpace())
                {
                    var envText = Fs.ReadTextFile(envFile);
                    var env = DotEnvSerializer.Deserialize<EnvDocument>(envText);

                    if (env is not null)
                    {
                        foreach (var (key, value) in env.ToDictionary())
                        {
                            envValues[key] = value;
                        }
                    }
                }
                else if (!defaultEnvFile.IsNullOrWhiteSpace())
                {
                    var envText = Fs.ReadTextFile(defaultEnvFile);
                    var env = DotEnvSerializer.Deserialize<EnvDocument>(envText);

                    if (env is not null)
                    {
                        foreach (var (key, value) in env.ToDictionary())
                        {
                            envValues[key] = value;
                        }
                    }
                }
            }
            else
            {
                if (!defaultEnvFile.IsNullOrWhiteSpace())
                {
                    var envText = Fs.ReadTextFile(defaultEnvFile);
                    var env = DotEnvSerializer.Deserialize<EnvDocument>(envText);

                    if (env is not null)
                    {
                        foreach (var (key, value) in env.ToDictionary())
                        {
                            envValues[key] = value;
                        }
                    }
                }

                if (!envFile.IsNullOrWhiteSpace())
                {
                    var envText = Fs.ReadTextFile(envFile);
                    var env = DotEnvSerializer.Deserialize<EnvDocument>(envText);

                    if (env is not null)
                    {
                        foreach (var (key, value) in env.ToDictionary())
                        {
                            envValues[key] = value;
                        }
                    }
                }
            }

            cmd.WithEnv(envValues);
        }
    }
}

public class ComposeCommandHandler : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}