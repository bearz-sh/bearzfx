using Bearz.Std;

using FluentBuilder;

namespace Bearz.Cli.Docker;

[AutoGenerateBuilder(true)]
public class DockerComposeStopArgs : DockerComposeArgs
{
    public List<string> Services { get; set; } = new List<string>();

    public int? Timeout { get; set; }

    protected override string SubCommand => "stop";

    public override CommandArgs BuildArgs()
    {
        this.TrailingArguments.AddRange(this.Services);

        return base.BuildArgs();
    }

    protected override bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        if (name == nameof(this.Services))
            return true;

        return base.Handle(name, value, type, args);
    }
}