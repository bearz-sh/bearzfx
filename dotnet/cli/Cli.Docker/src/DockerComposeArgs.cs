using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.Cli.Docker;

public abstract class DockerComposeArgs : ReflectionCommandArgsBuilder
{
    private readonly string[] skipped = new[] { "File", "EnvFile", "Parallel", "Profile", "ProjectDirectory", "Compatibility", "Ansi" };

    protected DockerComposeArgs()
        : base(null)
    {
    }

    public string? ProjectName { get; set; }

    public List<string> File { get; set; } = new();

    public string? EnvFile { get; set; }

    public int? Parallel { get; set; }

    public bool Compatibility { get; set; }

    public List<string> Profile { get; set; } = new();

    public string? ProjectDirectory { get; set; }

    public DockerAnsiOptions Ansi { get; set; } = DockerAnsiOptions.Auto;

    protected abstract string SubCommand { get; }

    protected override CommandArgs BuildArgs(CommandArgs args)
    {
        args.Add("compose");
        this.ApplyBaseArgs(args);
        args.Add(this.SubCommand);
        return base.BuildArgs(args);
    }

    protected void ApplyBaseArgs(CommandArgs args)
    {
        if (!this.ProjectName.IsNullOrWhiteSpace())
            args.Add("--project-name", this.ProjectName);

        if (!this.ProjectDirectory.IsNullOrWhiteSpace())
            args.Add("--project-directory", this.ProjectDirectory);

        if (this.Ansi != DockerAnsiOptions.Auto)
            args.Add("--ansi", this.Ansi.ToString().ToLowerInvariant());

        if (this.Compatibility)
            args.Add("--compatibility");

        if (this.Parallel.HasValue)
            args.Add("--parallel", this.Parallel.Value.ToString());

        if (!this.EnvFile.IsNullOrWhiteSpace())
            args.Add("--env-file", this.EnvFile);

        if (this.Profile.Count > 0)
        {
            foreach (var item in this.Profile)
                args.Add("--profile", item);
        }

        if (this.File.Count > 0)
        {
            foreach (var item in this.File)
                args.Add("--file", item);
        }
    }

    protected override bool Handle(string name, object? value, Type type, CommandArgs args)
    {
        if (this.skipped.Any(o => o == name))
            return true;

        return false;
    }
}