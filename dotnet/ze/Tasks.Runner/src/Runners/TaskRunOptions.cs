namespace Ze.Tasks.Runners;

public class TaskRunOptions : ITaskRunOptions
{
    public IReadOnlyList<string> Targets { get; set; } = new[] { "default" };

    public bool SkipDependencies { get; set; }
}