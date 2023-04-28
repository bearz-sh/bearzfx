using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text.DotEnv;
using Bearz.Text.DotEnv.Document;

using Microsoft.Extensions.Configuration;

using Command = System.CommandLine.Command;

namespace Casa.Commands.Compose;

/*
      --abort-on-container-exit   Stops all containers if any container was stopped. Incompatible with -d
      --always-recreate-deps      Recreate dependent containers. Incompatible with --no-recreate.
      --attach stringArray        Attach to service output.
      --attach-dependencies       Attach to dependent containers.
      --build                     Build images before starting containers.
  -d, --detach                    Detached mode: Run containers in the background
      --exit-code-from string     Return the exit code of the selected service container. Implies --abort-on-container-exit
      --force-recreate            Recreate containers even if their configuration and image haven't changed.
      --no-attach stringArray     Don't attach to specified service.
      --no-build                  Don't build an image, even if it's missing.
      --no-color                  Produce monochrome output.
      --no-deps                   Don't start linked services.
      --no-log-prefix             Don't print prefix in logs.
      --no-recreate               If containers already exist, don't recreate them. Incompatible with --force-recreate.
      --no-start                  Don't start the services after creating them.
      --pull string               Pull image before running ("always"|"missing"|"never") (default "missing")
      --quiet-pull                Pull without printing progress information.
      --remove-orphans            Remove containers for services not defined in the Compose file.
  -V, --renew-anon-volumes        Recreate anonymous volumes instead of retrieving data from the previous containers.
      --scale scale               Scale SERVICE to NUM instances. Overrides the scale setting in the Compose file if present.
      --timestamps                Show timestamps.
      --wait                      Wait for services to be running|healthy. Implies detached mode.
      --wait-timeout int          timeout waiting for application to be running|healthy.
  -t, --waitTimeout int           Use this waitTimeout in seconds for container shutdown when attached or when containers are already running.
                                  (default 10)

 */

[CommandHandler(typeof(UpCommandHandler))]
public class UpCommand : Command
{
    public UpCommand()
        : base("up", "Bring the environment up")
    {
        this.AddOption(new Option<bool>(
            new[] { "--abort-on-container-exit" },
            "Stops all containers if any container was stopped. Incompatible with -d"));

        this.AddOption(new Option<bool>(
            new[] { "--always-recreate-deps" },
            "Recreate dependent containers. Incompatible with --no-recreate."));

        this.AddOption(new Option<string[]>(
            new[] { "--attach" },
            "Attach to service output."));

        this.AddOption(new Option<bool>(
            new[] { "--attach-dependencies" },
            "Attach to dependent containers."));

        this.AddOption(new Option<bool>(
            new[] { "--build" },
            "Build images before starting containers."));

        this.AddOption(new Option<bool>(
            new[] { "--detach", "-d" },
            "Detached mode: Run containers in the background"));

        this.AddOption(new Option<string>(
            new[] { "--exit-code-from" },
            "Return the exit code of the selected service container. Implies --abort-on-container-exit"));

        this.AddOption(new Option<bool>(
            new[] { "--force-recreate" },
            "Recreate containers even if their configuration and image haven't changed."));

        this.AddOption(new Option<string[]>(
            new[] { "--no-attach" },
            "Don't attach to specified service."));

        this.AddOption(new Option<bool>(
            new[] { "--no-build" },
            "Don't build an image, even if it's missing."));

        this.AddOption(new Option<bool>(
            new[] { "--no-color" },
            "Produce monochrome output."));

        this.AddOption(new Option<bool>(
            new[] { "--no-deps" },
            "Don't start linked services."));

        this.AddOption(new Option<bool>(
            new[] { "--no-log-prefix" },
            "Don't print prefix in logs."));

        this.AddOption(new Option<bool>(
            new[] { "--no-recreate" },
            "If containers already exist, don't recreate them. Incompatible with --force-recreate."));

        this.AddOption(new Option<bool>(
            new[] { "--no-start" },
            "Don't start the services after creating them."));

        this.AddOption(new Option<string>(
            new[] { "--pull" },
            "Pull image before running (\"always\"|\"missing\"|\"never\") (default \"missing\")"));

        this.AddOption(new Option<bool>(
            new[] { "--quiet-pull" },
            "Pull without printing progress information."));

        this.AddOption(new Option<bool>(
            new[] { "--remove-orphans" },
            "Remove containers for services not defined in the Compose file."));

        this.AddOption(new Option<bool>(
            new[] { "--renew-anon-volumes", "-V" },
            "Recreate anonymous volumes instead of retrieving data from the previous containers."));

        this.AddOption(new Option<string>(
            new[] { "--scale" },
            "Scale SERVICE to NUM instances. Overrides the scale setting in the Compose file if present."));

        this.AddOption(new Option<bool>(
            new[] { "--timestamps" },
            "Show timestamps."));

        this.AddOption(new Option<bool>(
            new[] { "--wait" },
            "Wait for services to be running|healthy. Implies detached mode."));

        this.AddOption(new Option<int>(
            new[] { "--wait-timeout" },
            "timeout waiting for application to be running|healthy."));
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UpCommandHandler : ComposeCommandBaseHandler
{
    public UpCommandHandler(IConfiguration config)
        : base(config)
    {
    }

    public bool AbortOnContainerExit { get; set; }

    public bool AlwaysRecreateDeps { get; set; }

    public string[]? Attach { get; set; }

    public bool AttachDependencies { get; set; }

    public bool Build { get; set; }

    public bool Detach { get; set; }

    public string? ExitCodeFrom { get; set; }

    public bool ForceRecreate { get; set; }

    public string[]? NoAttach { get; set; }

    public bool NoBuild { get; set; }

    public bool NoColor { get; set; }

    public bool NoDeps { get; set; }

    public bool NoLogPrefix { get; set; }

    public bool NoRecreate { get; set; }

    public bool NoStart { get; set; }

    public string? Pull { get; set; }

    public bool QuietPull { get; set; }

    public bool RemoveOrphans { get; set; }

    public bool RenewAnonVolumes { get; set; }

    public string? Scale { get; set; }

    public bool TimeStamps { get; set; }

    public bool Wait { get; set; }

    public int WaitTimeout { get; set; }

    public int WaitTimeoutSeconds { get; set; }

    public override int Invoke(InvocationContext context)
    {
        this.BuildComposeArgs();
        var args = this.Command.StartInfo.Args;
        args.Add("up");

        if (this.AttachDependencies)
            args.Add("--attach-dependencies");

        if (this.Attach?.Length > 0)
        {
            foreach (var service in this.Attach)
                args.Add("--attach", service);
        }

        if (this.AlwaysRecreateDeps)
            args.Add("--always-recreate-deps");

        if (this.AbortOnContainerExit)
            args.Add("--abort-on-container-exit");

        if (this.Build)
            args.Add("--build");

        if (this.Detach)
            args.Add("--detach");

        if (!this.ExitCodeFrom.IsNullOrWhiteSpace())
            args.Add("--exit-code-from", this.ExitCodeFrom);

        if (this.ForceRecreate)
            args.Add("--force-recreate");

        if (this.NoRecreate)
            args.Add("--no-recreate");

        if (this.NoBuild)
            args.Add("--no-build");

        if (this.NoDeps)
            args.Add("--no-deps");

        if (this.NoLogPrefix)
            args.Add("--no-log-prefix");

        if (this.NoColor)
            args.Add("--no-color");

        if (this.NoStart)
            args.Add("--no-start");

        if (this.NoAttach?.Length > 0)
        {
            foreach (var service in this.NoAttach)
                args.Add("--no-attach", service);
        }

        if (this.QuietPull)
            args.Add("--quiet-pull");

        if (this.RemoveOrphans)
            args.Add("--remove-orphans");

        if (this.RenewAnonVolumes)
            args.Add("--renew-anon-volumes");

        if (this.TimeStamps)
            args.Add("--timestamps");

        if (this.Wait)
            args.Add("--wait");

        if (this.WaitTimeout > 0)
            args.Add("--wait-timeout", this.WaitTimeout.ToString());

        if (this.WaitTimeoutSeconds > 0)
            args.Add("--waitTimeout", this.WaitTimeoutSeconds.ToString());

        this.Command.WithStdio(Stdio.Inherit);

        var output = this.Command.Output();
        return output.ExitCode;
    }
}