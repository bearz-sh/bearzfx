using Bearz.Text;

namespace Plank.Tasks;

public class PlankJob : IJob
{
    public PlankJob(string name)
       : this(name, name)
    {
    }

    public PlankJob(string name, string id)
    {
        this.Name = name;
        this.Id = IdGenerator.Instance.FromName(id.AsSpan());
    }

    public string Id { get; }

    public string Name { get; }

    public int Timeout { get; set; }

    public IReadOnlyList<JobDependency> Dependencies { get; set; } = Array.Empty<JobDependency>();

    public List<ITask> Tasks { get; } = new();

    IReadOnlyList<ITask> IJob.Tasks => this.Tasks;

    public Task<JobResult> RunAsync(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new JobResult());
    }
}