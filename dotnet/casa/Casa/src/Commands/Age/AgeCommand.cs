using Bearz.Extensions.Hosting.CommandLine;

namespace Casa.Commands.Age;

[CommandHandler(typeof(AgeEncryptCommandHandler))]
public class AgeCommand : AgeBaseCommand
{
    public AgeCommand()
        : base(
            "age",
            "casa commands to run the age command line tool in a more intuitive way. The default action is to encrypt.")
    {
        this.AddCommand(new AgeKeygenCommand());
        this.AddCommand(new AgeEncryptCommand());
        this.AddCommand(new AgeDecryptCommand());
    }
}