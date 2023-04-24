using Bearz.Extra.Strings;
using Bearz.Std;

using FluentBuilder;

namespace Bearz.Extensions.CliCommand.Docker;

[AutoGenerateBuilder(true)]
public class DockerComposeDownArgs : DockerComposeArgs
{
    public bool RemoveOrphans { get; set; }

    public DockerRmiOptions? Rmi { get; set; }

    public int? Timeout { get; set; }

    protected override string SubCommand => "down";

    protected override bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        if (name == nameof(this.Rmi))
        {
            var s = value?.ToString();
            if (!s.IsNullOrWhiteSpace())
                args.Add("--rmi", s.ToLowerInvariant());

            return true;
        }

        return base.Handle(name, value, type, args);
    }
}