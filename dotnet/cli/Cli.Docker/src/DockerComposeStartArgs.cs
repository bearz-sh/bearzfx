using Bearz.Std;

using FluentBuilder;

namespace Bearz.Extensions.CliCommand.Docker;

[AutoGenerateBuilder(true)]
public class DockerComposeStartArgs : DockerComposeArgs
{
    public List<string> Services { get; set; } = new List<string>();

    protected override string SubCommand => "start";

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