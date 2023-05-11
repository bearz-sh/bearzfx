namespace Plank.Tasks.Runner.Yaml;

public class YamlTaskWorkflowDefinition
{
    public YamlTaskWorkflowDefinition(TaskCollection collection)
    {
        this.Tasks = collection;
    }

    public TaskCollection Tasks { get; }
}