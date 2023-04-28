using System.CommandLine;

using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Env;

[CommandHandler(typeof(ListCommandHandler))]
public class EnvCommand : Command
{
    public EnvCommand()
        : base("env", "Manage environments")
    {
        var arg = new Option<string>("store", "The name of the environment store.");
        this.AddGlobalOption(arg);
        this.AddCommand(new CreateCommand());
        this.AddCommand(new ListCommand());
        this.AddCommand(new Secrets.SecretsCommand());
    }
}