using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extra.Strings;

using Microsoft.Extensions.Configuration;

namespace Casa.Commands.Compose;

public class DownCommand : Command
{
    public DownCommand()
        : base("down", "Stop and remove containers, networks, images, and volumes")
    {
        this.AddOption(
            new Option<bool>(
                new[] { "--remove-orphans" },
                "Remove containers for services not defined in the Compose file"));

        this.AddOption(new Option<string?>(
            new[] { "--rmi" },
            "Remove images used by services. \"local\" remove only images that don't have a custom tag (\"local\"|\"all\")"));

        this.AddOption(new Option<int>(
            new[] { "--timeout", "-t" },
            "Specify a shutdown timeout in seconds (default 10)"));

        this.AddOption(new Option<bool>(
            new[] { "--volumes" },
            "Remove named volumes declared in the volumes section of the Compose file and anonymous volumes attached to containers"));
    }
}

public class DownCommandHandler : ComposeCommandBaseHandler
{
    public DownCommandHandler(IConfiguration config)
        : base(config)
    {
    }

    public bool RemoveOrphans { get; set; }

    public string? Rmi { get; set; }

    public int Timeout { get; set; }

    public bool Volumes { get; set; }

    public override int Invoke(InvocationContext context)
    {
        this.BuildComposeArgs();
        var args = this.Command.StartInfo.Args;

        if (this.RemoveOrphans)
            args.Add("--remove-orphans");

        if (!this.Rmi.IsNullOrWhiteSpace())
            args.Add("--rmi", this.Rmi);

        if (this.Timeout > 0)
            args.Add("--timeout", this.Timeout.ToString());

        if (this.Volumes)
            args.Add("--volumes");

        return this.Command.Output().ExitCode;
    }
}