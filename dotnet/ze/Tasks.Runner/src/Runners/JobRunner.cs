using Bearz.Extra.Strings;
using Bearz.Text;

namespace Ze.Tasks.Runners;

public class JobRunner
{
    /*
    public async Task<JobStatus> RunAsync(
        string target,
        JobCollection jobs,
        IExecutionContext context,
        CancellationToken cancellationToken)
    {
        var rootJob = jobs.FirstOrDefault(o => o.Name.EqualsIgnoreCase(target));
        if (rootJob is null)
        {
            context.Log.LogError("Job '{0}' not found", target);
            return JobStatus.Failed;
        }

        if (rootJob.Dependencies.Count == 0)
        {
            var ct = cancellationToken;
            var ctx = new JobExecutionContext(rootJob.Id, context);
            if (rootJob.Timeout > 0)
            {
                var cts = new CancellationTokenSource(rootJob.Timeout);
                ct = CancellationTokenSource.CreateLinkedTokenSource(ct, cts.Token).Token;
            }

            return await rootJob.RunAsync(ctx, ct);
        }
        else
        {
            IExecutionContext ctx = context;
            jobs.CheckForCircularDependencies();
            jobs.CheckForMissingDependencies();

            bool failed = false;

            var list = new List<Tuple<IJob, bool>>();
            Collect(list, rootJob, true, jobs);

            var set = new List<IJob>();
            IDictionary<string, object?> outputs = new Dictionary<string, object?>();

            foreach (var next in list)
            {
                var job = next.Item1;
                var sequental = next.Item2;

                if (cancellationToken.IsCancellationRequested)
                {
                    return JobStatus.Cancelled;
                }

                if (sequental)
                {
                    if (set.Count > 0)
                    {
                        var tasks = new List<Task<JobResult>>();

                        foreach (var nextJob in set)
                        {
                            IJobExecutionContext nextCtx;
                            if (ctx is IActionExecutionContext actx)
                            {
                                nextCtx = new JobExecutionContext(nextJob.Id, ctx, actx.Outputs.ToDictionary());
                            }
                            else
                            {
                                nextCtx = new JobExecutionContext(nextJob.Id, ctx, outputs);
                            }

                            var t = this.RunAsync(nextJob, nextCtx, cancellationToken);
                            if (t.Status == System.Threading.Tasks.TaskStatus.Created)
                                t.Start();
                            tasks.Add(t);
                        }

                        try
                        {
                            var results = await Task.WhenAll(tasks);
                            outputs = new Dictionary<string, object?>();

                            foreach (var result in results)
                            {
                                var data = result.Context.Outputs.ToDictionary();

                                foreach (var kvp in data)
                                {
                                    if (kvp.Key.StartsWith("jobs."))
                                        outputs[kvp.Key] = kvp.Value;

                                    var sb = StringBuilderCache.Acquire();
                                    sb.Append("jobs.").Append(result.Id).Append(".")
                                        .Append(kvp.Key);

                                    var newKey = StringBuilderCache.GetStringAndRelease(sb);
                                    outputs[newKey] = kvp.Value;
                                }

                                if (result.Status == JobStatus.Failed)
                                {
                                    failed = true;
                                    break;
                                }

                                if (result.Status == JobStatus.Cancelled)
                                    return JobStatus.Cancelled;
                            }

                            set.Clear();
                        }
                        catch (Exception ex)
                        {
                            context.Log.LogError("Unexpected error running jobs", ex);
                            return JobStatus.Failed;
                        }
                    }

                    if (failed)
                        break;

                    var jctx = new JobExecutionContext(job.Id, ctx, outputs);
                    ctx = jctx;
                    var lastResult = await this.RunAsync(job, jctx, cancellationToken);
                    if (lastResult.Status == JobStatus.Cancelled)
                    {
                        return JobStatus.Cancelled;
                    }

                    if (lastResult.Status == JobStatus.Failed)
                    {
                        return JobStatus.Failed;
                    }

                    outputs = new Dictionary<string, object?>();
                    foreach (var kvp in lastResult.Context.Outputs.ToDictionary())
                    {
                        if (kvp.Key.StartsWith("jobs."))
                            outputs[kvp.Key] = kvp.Value;

                        var sb = StringBuilderCache.Acquire();
                        sb.Append("jobs.").Append(job.Id).Append(".")
                            .Append(kvp.Key);

                        var newKey = StringBuilderCache.GetStringAndRelease(sb);
                        outputs[newKey] = kvp.Value;
                    }
                }
                else
                {
                    set.Add(job);
                }
            }

            if (failed)
                return JobStatus.Failed;
        }

        return JobStatus.Completed;
    }

    private static void Collect(List<Tuple<IJob, bool>> list, IJob job, bool seq, JobCollection jobs)
    {
        foreach (var dependency in job.Dependencies)
        {
            var depJob = jobs.FirstOrDefault(o => o.Id.EqualsIgnoreCase(dependency.Id));
            if (depJob is null)
            {
                throw new InvalidOperationException($"Job '{job.Name}' depends on '{dependency}' which is not defined");
            }

            Collect(list, depJob, dependency.Sequential, jobs);
        }

        if (list.All(o => o.Item1.Id != job.Id))
        {
            list.Add(Tuple.Create(job, seq));
        }
    }

    private async Task<JobResult> RunAsync(
        IJob job,
        IJobExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var ct = cancellationToken;
        if (job.Timeout > 0)
        {
            var cts = new CancellationTokenSource(job.Timeout);
            ct = CancellationTokenSource.CreateLinkedTokenSource(ct, cts.Token).Token;
        }

        var status = await job.RunAsync(context, ct);
        return new JobResult(job.Id, context, status);
    }

    private sealed class JobResult
    {
        public JobResult(string id, IJobExecutionContext context, JobStatus status)
        {
            this.Id = id;
            this.Context = context;
            this.Status = status;
        }

        public string Id { get;  }

        public IJobExecutionContext Context { get;  }

        public JobStatus Status { get;  }
    }
    */
}