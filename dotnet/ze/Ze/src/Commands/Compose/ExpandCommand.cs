using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Ze.Package;
using Ze.Package.Actions;

using Command = System.CommandLine.Command;

namespace Ze.Commands.Compose;

[CommandHandler(typeof(ExpandCommandHandler))]
public class ExpandCommand : Command
{
    public ExpandCommand()
        : base("expand", "expands a plank compose app")
    {
        this.AddArgument(new Argument<string>("app"));
        this.AddOption(new Option<bool>(new[] { "--force", "-f" }, "force overwrite"));
        this.AddOption(new Option<string?[]>(new[] { "--var-file" }, "variable file to use"));
    }
}

public class ExpandCommandHandler : AppCommandHandlerBase
{
    private readonly ILogger log;

    public ExpandCommandHandler(IConfiguration config, ILogger<ExpandCommandHandler> logger)
        : base(config)
    {
        this.log = logger;
    }

    public override int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<int> InvokeAsync(InvocationContext context)
    {
        try
        {
            var package = new ZeExtractedPackage(
                this.GetPackageDirectory(),
                this.PathSpec,
                this.Target,
                this.VarFile);
            var installTask = new InstallAction();
            installTask.Run(package, this.Force);
        }
        catch (Exception ex)
        {
            this.log.LogError(ex, ex.Message);
            return Task.FromResult(1);
        }

        return Task.FromResult(0);
    }
}