using FluentBuilder;

namespace Bearz.Extensions.CliCommand.Docker;

[AutoGenerateBuilder(true)]
public class DockerComposeUpArgs : DockerComposeArgs
{
    public bool AbortOnContainerExit { get; set; }

    public bool AlwaysRecreateDeps { get; set; }

    public List<string> Attach { get; set; } = new();

    public bool AttachDependencies { get; set; }

    public bool Build { get; set; }

    public bool Detach { get; set; }

    public string? ExitCodeFromString { get; set; }

    public bool ForceRecreate { get; set; }

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

    public int? Scale { get; set; }

    public int? Timeout { get; set; }

    public bool Timestamps { get; set; }

    public bool Wait { get; set; }

    protected override string SubCommand => "Up";
}