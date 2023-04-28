using System.CommandLine;

namespace Casa.Commands.Age;

public abstract class AgeBaseCommand : Command
{
    protected AgeBaseCommand(string commandName, string description)
        : base(commandName, description)
    {
        this.AddArgument(new Argument<string>("input", "the input file to encrypt or decrypt"));
        this.AddOption(new Option<string?>(new[] { "output", "o" }, "the output file"));
        this.AddOption(new Option<bool>(new[] { "armor", "a" }, "Encrypt to a PEM encoded format."));
        this.AddOption(new Option<string[]?>(
            new[] { "recipient", "r" },
            "Encrypt to the specified recipient. Can be specified multiple times."));
        this.AddOption(new Option<string[]?>(
            new[] { "recipient-file", "R" },
            "Encrypt to the specified recipient. Can be specified multiple times."));
        this.AddOption(new Option<bool>(new[] { "passphrase", "p" }, "Encrypt with the specified passphrase."));
        this.AddOption(new Option<string[]?>(new[] { "identity", "i" }, "Use the given identity file."));
    }
}