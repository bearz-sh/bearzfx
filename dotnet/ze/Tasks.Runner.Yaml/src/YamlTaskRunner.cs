using Bearz.Extra.Strings;
using Bearz.Handlebars.Helpers;
using Bearz.Std;

using HandlebarsDotNet;

using Ze.Tasks.Internal;
using Ze.Tasks.Messages;
using Ze.Tasks.Runner.Runners;
using Ze.Tasks.Runners;

namespace Ze.Tasks.Runner.Yaml;

public class YamlTaskRunner : IYamlTaskRunner
{
    public YamlTaskRunner(IServiceProvider services)
    {
        this.Services = services;
    }

    private IServiceProvider Services { get; }

    public async Task<TaskRunnerResult> RunAsync(
        IYamlTaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var yamlFile = options?.TaskFile ?? FsPath.Combine(Env.Cwd, "planktasks.yaml");

        var file = Fs.GetExistingFile(yamlFile, new[] { ".yaml", ".yml" });
        if (file is null)
            throw new FileNotFoundException($"Unable to find a planktasks.yaml file from {file}.");

        var workflow = TasksYamlFileParser.ParseFile(file);

        var result = await this.RunAsync(workflow.Tasks, options, context, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<TaskRunnerResult> RunAsync(
        IDependencyCollection<ITask> tasks,
        ITaskRunOptions? options = null,
        IExecutionContext? context = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new TaskRunOptions();
        bool createdRootContext = false;
        if (context is null)
        {
            createdRootContext = true;
            context = new RootExecutionContext(this.Services);
        }

        var bus = context.Bus;

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

                        var hb2 = Handlebars.Create();
                        hb2.RegisterJsonHelpers();
                        hb2.RegisterStringHelpers();
                        hb2.RegisterDateTimeHelpers();
                        hb2.RegisterRegexHelpers();

                        if (rootTask is YamlShellTask shellTask && ctx.Variables is IMutableVariables mut)
                        {
                            foreach (var kvp2 in shellTask.Inputs)
                            {
                                var inputBock = kvp2.Value;
                                if (inputBock.Expression.IsNullOrWhiteSpace())
                                {
                                    mut[kvp2.Key] = null;
                                    continue;
                                }

                                var template = hb2.Compile(inputBock.Expression);
                                var value = template(mut.ToDictionary());
                                mut[kvp2.Key] = value;
                                shellTask.Env[kvp2.Key] = value;
                            }

                            Console.WriteLine("compile template");
                            var run = shellTask.Run;
                            var runTemplate = hb2.Compile(run);
                            shellTask.Run = runTemplate(mut.ToDictionary());
                        }

                        await rootTask.RunAsync(ctx, ct)
                            .ConfigureAwait(false);

                        bus.Publish(new TaskFinishedMessage(rootTask, ctx.Status));

                        return ctx.Status switch
                        {
                            TaskStatus.Completed => TaskRunnerResult.Success(),
                            TaskStatus.Failed => TaskRunnerResult.Failed(),
                            TaskStatus.Cancelled => TaskRunnerResult.Cancelled(),
                            _ => TaskRunnerResult.Failed(),
                        };
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

            var hb = Handlebars.Create();
            hb.RegisterJsonHelpers();
            hb.RegisterStringHelpers();
            hb.RegisterDateTimeHelpers();
            hb.RegisterRegexHelpers();

            foreach (var task in set)
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
                if (task is not YamlShellTask)
                    Console.WriteLine("not yaml shell task");

                if (ctx.Variables is not IMutableVariables)
                    Console.WriteLine("not mutable variables");

                if (task is YamlShellTask shellTask && ctx.Variables is IMutableVariables mut)
                {
                    foreach (var kvp2 in shellTask.Inputs)
                    {
                        var inputBock = kvp2.Value;
                        if (inputBock.Expression.IsNullOrWhiteSpace())
                        {
                            mut[kvp2.Key] = null;
                            continue;
                        }

                        var template = hb.Compile(inputBock.Expression);
                        var value = template(mut.ToDictionary());
                        mut[kvp2.Key] = value;
                        shellTask.Env[kvp2.Key] = value;
                    }

                    var run = shellTask.Run;
                    var runTemplate = hb.Compile(run);
                    shellTask.Run = runTemplate(mut.ToDictionary());
                }

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
            // ensure the scoped context is disposed
            if (createdRootContext && context is IDisposable disposable)
            {
                disposable.Dispose();
            }
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