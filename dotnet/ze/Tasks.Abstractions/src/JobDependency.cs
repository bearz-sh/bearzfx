namespace Ze.Tasks;

public struct JobDependency
{
    public JobDependency(string id, bool sequential)
    {
        this.Id = id;
        this.Sequential = sequential;
    }

    public string Id { get; }

    public bool Sequential { get; }
}