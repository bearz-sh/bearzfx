using Bearz.Secrets;
using Bearz.Virtual;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ze.Tasks.Internal;

public abstract class ExecutionContext : IExecutionContext, IDisposable
{
    private readonly IServiceScope? scope;

    protected ExecutionContext(IServiceProvider services)
    {
        this.scope = services.CreateScope();
        this.Services = this.scope.ServiceProvider;
        this.Env = this.Services.GetService<IEnvironment>() ?? new VirtualEnvironment();
        this.Process = this.Services.GetService<IProcess>() ?? new VirtualProcess(this.Env);
        this.Path = this.Services.GetService<IPath>() ?? new VirtualPath(this.Env);
        this.Fs = this.Services.GetService<IFileSystem>() ?? new VirtualFileSystem(this.Path);
        this.SecretMasker = this.Services.GetService<ISecretMasker>() ?? Bearz.Secrets.SecretMasker.Default;
        this.Log = NullLogger.Instance;
        this.Variables = new Variables();

        this.Bus = this.Services.GetRequiredService<IMessageBus>();

        var configuration = this.Services.GetService<IConfiguration>();
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
        this.SecretMasker = executionContext.SecretMasker;
        this.Variables = new Variables(executionContext.Variables.ToDictionary());
    }

    public IEnvironment Env { get; }

    public IProcess Process { get; }

    public IPath Path { get; }

    public IFileSystem Fs { get; }

    public IServiceProvider Services { get; }

    public ILogger Log { get; protected set; }

    public IMessageBus Bus { get; set; }

    public IConfiguration Config { get; }

    public IVariables Variables { get; set; }

    public ISecretMasker SecretMasker { get; set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || this.scope is null)
        {
            return;
        }

        this.scope.Dispose();

        // only dispose the bus if it was created by the execution context
        // thru the service provider
        this.Bus.Dispose();
    }
}