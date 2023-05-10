using Microsoft.Extensions.DependencyInjection;

using Plank.Tasks.Internal;
using Plank.Tasks.Messages;
using Plank.Tasks.Runners;

namespace Plank.Tasks.Runner.Yaml;

public class YamlTaskRunner : ITaskRunner
{
    public YamlTaskRunner(IServiceProvider services)
    {
        this.Services = services;
    }

    private IServiceProvider Services { get; }

    public async Task<TaskRunnerResult> RunAsync(
        IDependencyCollection<ITask> tasks,
        ITaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new TaskRunOptions();
        IMessageBus bus = context?.Bus ?? this.Services.GetRequiredService<IMessageBus>();
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
                    var ctx = context != null
                        ? new TaskExecutionContext(rootTask, context)
                        : new TaskExecutionContext(rootTask, this.Services);

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

            IExecutionContext? parentContext = context;
            bool failed = false;

            foreach (var task in tasks)
            {
                if (failed && !task.ContinueOnError)
                {
                    bus.Publish(new TaskStartedMessage(task));
                    bus.Publish(new TaskFinishedMessage(task, TaskStatus.Skipped));
                    continue;
                }

                var ctx = parentContext != null
                    ? new TaskExecutionContext(task, parentContext)
                    : new TaskExecutionContext(task, this.Services);

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