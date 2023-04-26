using System.CommandLine;
using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;

using Microsoft.Extensions.Configuration;

namespace Casa.Commands.Env;

[CommandHandler(typeof(GetCommandHandler))]
public class GetCommand : Command
{
    public GetCommand()
        : base("get", "Get the default environment")
    {
    }
}

public class GetCommandHandler : ICommandHandler
{
    private readonly IConfiguration config;

    public GetCommandHandler(IConfiguration config)
    {
        this.config = config;
    }

    public int Invoke(InvocationContext context)
    {
        Console.WriteLine(this.config.GetValue<string>("env:name"));
        return 0;
    }

    public Task<int> InvokeAsync(InvocationContext context)
        => Task.FromResult(this.Invoke(context));
}