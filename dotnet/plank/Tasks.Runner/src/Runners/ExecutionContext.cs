using Bearz.Virtual;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Plank.Tasks.Internal;

public abstract class ExecutionContext : IExecutionContext
{
    protected ExecutionContext(IServiceProvider services)
    {
        this.Services = services;
        this.Env = services.GetService<IEnvironment>() ?? new VirtualEnvironment();
        this.Process = services.GetService<IProcess>() ?? new VirtualProcess(this.Env);
        this.Path = services.GetService<IPath>() ?? new VirtualPath(this.Env);
        this.Fs = services.GetService<IFileSystem>() ?? new VirtualFileSystem(this.Path);
        this.Log = NullLogger.Instance;

        this.Bus = services.GetRequiredService<IMessageBus>();

        var configuration = services.GetService<IConfiguration>();
        if (configuration != null)
        {
            this.Config = configuration;
        }
        else
        {
            var builder = new ConfigurationBuilder();
            this.Config = builder.Build();
        }
    }

    protected ExecutionContext(IExecutionContext executionContext)
    {
        this.Services = executionContext.Services;
        this.Env = executionContext.Env;
        this.Process = executionContext.Process;
        this.Path = executionContext.Path;
        this.Fs = executionContext.Fs;
        this.Config = executionContext.Config;
        this.Log = NullLogger.Instance;
        this.Bus = executionContext.Bus;
    }

    public IEnvironment Env { get; }

    public IProcess Process { get; }

    public IPath Path { get; }

    public IFileSystem Fs { get; }

    public IServiceProvider Services { get; }

    public ILogger Log { get; protected set; }

    public IMessageBus Bus { get; set; }

    public IConfiguration Config { get; }
}