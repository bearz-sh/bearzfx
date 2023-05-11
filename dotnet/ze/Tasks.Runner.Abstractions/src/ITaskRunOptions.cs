namespace Ze.Tasks.Runners;

public interface ITaskRunOptions
{
    IReadOnlyList<string> Targets { get; set; }

    bool SkipDependencies { get; set; }
}