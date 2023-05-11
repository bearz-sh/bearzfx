namespace Ze.Tasks;

public enum JobStatus
{
    None,
    Queued,
    Running,
    Completed,
    Failed,
    Cancelled,
    Skipped,
}