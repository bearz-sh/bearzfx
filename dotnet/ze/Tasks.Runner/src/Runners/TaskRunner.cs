using Ze.Tasks.Internal;
using Ze.Tasks.Messages;
using Ze.Tasks.Runner.Runners;

namespace Ze.Tasks.Runners;

public class TaskRunner : ITaskRunner
{
    private readonly IServiceProvider services;

    public TaskRunner(IServiceProvider services)
    {
        this.services = services;
    }

    public async Task<TaskRunnerResult> RunAsync(
        IDependencyCollection<ITask> tasks,
        ITaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new TaskRunOptions();
        context ??= new RootExecutionContext(this.services);
        IMessageBus bus = context.Bus;
        var set = new List<ITask>();

        if (options.Targets.Count == 0)
        {
            bus.Publish(new ErrorMessage("Missing task name."));
            return TaskRunnerResult.Failed();
        }

        try
        {
            if (options.Targets.Count == 1)
            {
                var taskName = options.Targets[0];
                var rootTask = tasks[taskName];
                if (rootTask is null)
                {
                    bus.Publish(new ErrorMessage($"Unable to find a task with the name '{taskName}'."));
                    return TaskRunnerResult.Failed();
                }

                if (rootTask.Dependencies.Count == 0 || options.SkipDependencies)
                {
                    var ctx = new TaskExecutionContext(rootTask, context);

                    var ct = cancellationToken;
                    if (rootTask.Timeout > 0)
                    {
                        var cts = new CancellationTokenSource(rootTask.Timeout);
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                        ct = cts.Token;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        bus.Publish(new TaskStartedMessage(rootTask));
                        bus.Publish(new TaskFinishedMessage(rootTask, TaskStatus.Cancelled));
                        return TaskRunnerResult.Cancelled();
                    }

                    try
                    {
                        bus.Publish(new TaskStartedMessage(rootTask));

                        if (ctx.Variables is Variables v)
                        {
                            var taskInfo = new Dictionary<string, object?>() { ["name"] = rootTask.Name, };
                            v["task"] = taskInfo;
                        }

                        await rootTask.RunAsync(ctx, ct)
                            .ConfigureAwait(false);

                        bus.Publish(new TaskFinishedMessage(rootTask, TaskStatus.Completed));
                    }
                    catch (TaskCanceledException)
                    {
                        bus.Publish(new TaskFinishedMessage(rootTask, TaskStatus.Cancelled));
                        return TaskRunnerResult.Cancelled();
                    }
                    catch (Exception ex)
                    {
                        ctx.Error(ex);
                        bus.Publish(new TaskFinishedMessage(rootTask, TaskStatus.Failed));
                        return TaskRunnerResult.Failed();
                    }
                }

                Collect(rootTask, tasks, set);
            }
            else
            {
                foreach (var target in options.Targets)
                {
                    var task = tasks[target];
                    if (task is null)
                    {
                        bus.Publish(new ErrorMessage($"No tasks were found for {target}"));
                        return TaskRunnerResult.Failed();
                    }

                    if (options.SkipDependencies)
                    {
                        set.Add(task);
                        continue;
                    }

                    Collect(task, tasks, set);
                }
            }

            IExecutionContext parentContext = context;
            bool failed = false;

            ITaskExecutionContext? previousTaskContext = null;
            foreach (var task in tasks)
            {
                if (failed && !task.ContinueOnError)
                {
                    bus.Publish(new TaskStartedMessage(task));
                    bus.Publish(new TaskFinishedMessage(task, TaskStatus.Skipped));
                    continue;
                }

                if (previousTaskContext is not null && previousTaskContext.Variables is IMutableVariables parentVariables)
                {
                    var outputData = previousTaskContext.Outputs.ToDictionary();
                    if (!parentVariables.TryGetValue("outputs", out var outputObject))
                    {
                        outputObject = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                    }

                    var outputs = (IDictionary<string, object?>)outputObject!;
                    outputs[previousTaskContext.Task.Id] = outputData;
                }

                var ctx = new TaskExecutionContext(task, parentContext);

                previousTaskContext = ctx;

                var ct = cancellationToken;
                if (task.Timeout > 0)
                {
                    var cts = new CancellationTokenSource(task.Timeout);
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
                    ct = cts.Token;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    bus.Publish(new TaskStartedMessage(task));
                    bus.Publish(new TaskFinishedMessage(task, TaskStatus.Cancelled));
                    return TaskRunnerResult.Cancelled();
                }

                try
                {
                    bus.Publish(new TaskStartedMessage(task));

                    if (ctx.Variables is IMutableVariables mut)
                    {
                        var taskInfo = new Dictionary<string, object?>() { ["name"] = task.Name, };
                        mut["task"] = taskInfo;
                    }

                    await task.RunAsync(ctx, ct)
                        .ConfigureAwait(false);

                    var status = ctx.Status;
                    switch (status)
                    {
                        case TaskStatus.Failed:
                            failed = true;
                            break;
                        case TaskStatus.Cancelled:
                            bus.Publish(new TaskFinishedMessage(task, TaskStatus.Cancelled));
                            return TaskRunnerResult.Cancelled();
                    }

                    bus.Publish(new TaskFinishedMessage(task, status));
                }
                catch (TaskCanceledException)
                {
                    bus.Publish(new TaskFinishedMessage(task, TaskStatus.Cancelled));
                }
                catch (Exception ex)
                {
                    failed = true;
                    ctx.Error(ex);
                    bus.Publish(new TaskFinishedMessage(task, TaskStatus.Failed));
                    continue;
                }

                parentContext = ctx;
            }

            if (failed)
            {
                return TaskRunnerResult.Failed();
            }

            return TaskRunnerResult.Success();
        }
        catch (Exception ex)
        {
            bus.Publish(new ErrorMessage(ex));
            return TaskRunnerResult.Failed();
        }
        finally
        {
            if (context is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private static void Collect(ITask task, IDependencyCollection<ITask> source, List<ITask> destination)
    {
        foreach (var dep in task.Dependencies)
        {
            var childTask = source[dep];
            if (childTask is null)
                throw new InvalidOperationException($"Task dependency {dep} was not found for task {task.Name}");

            Collect(childTask, source, destination);
        }

        if (!destination.Contains(task))
            destination.Add(task);
    }
}