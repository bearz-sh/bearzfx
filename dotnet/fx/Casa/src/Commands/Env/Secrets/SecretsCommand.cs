using System.CommandLine;

using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Env.Secrets;

[CommandHandler(typeof(ListCommandHandler))]
public class SecretsCommand : Command
{
    public SecretsCommand()
        : base("secrets", "Manages environment secrets. The default command is list.")
    {
        this.AddCommand(new DeleteCommand());
        this.AddCommand(new GetCommand());
        this.AddCommand(new ListCommand());
        this.AddCommand(new SetCommand());
    }
}