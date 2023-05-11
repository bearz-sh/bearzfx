using System.CommandLine.Invocation;

using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;

using Ze.Package.Actions;

namespace Ze.Commands.Compose;

public abstract class AppCommandHandlerBase : ICommandHandler
{
    protected AppCommandHandlerBase(IConfiguration config)
    {
        this.Config = config;
        this.AppDirectory = config.GetValue<string?>("appDirectory") ?? Paths.AppDirectory;
        this.PackagesDirectory =
            config.GetValue<string?>("packagesDirectory") ?? Path.Join(Paths.AppDirectory, "packages");

        this.PathSpec = new PathSpec(this.AppDirectory);
    }

    public string? App { get; set; }

    public string? Target { get; set; }

    public string[]? VarFile { get; set; }

    public bool Force { get; set; }

    protected IConfiguration Config { get; }

    protected string AppDirectory { get; }

    protected string PackagesDirectory { get; }

    protected PathSpec PathSpec { get; }

    public abstract int Invoke(InvocationContext context);

    public abstract Task<int> InvokeAsync(InvocationContext context);

    protected string GetPackageDirectory()
    {
        var app = this.App;
        var packageDir = this.PackagesDirectory;
        if (app.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException("Missing app name");
        }

        if (app.Contains('/') || app.Contains('\\'))
        {
            packageDir = app;
        }
        else
        {
            packageDir = FsPath.Join(packageDir, app);
        }

        return packageDir;
    }
}