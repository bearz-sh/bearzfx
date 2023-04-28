using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

namespace Casa.Commands.Sops;

public class SopsDecryptCommand : SopsCommandBase
{
    public SopsDecryptCommand()
        : base("decrypt", "Decrypt a file")
    {
        this.AddOption(
            new Option<string?>("--extract", "Extract the encrypted values to a file instead of decrypting the entire file"));
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class SopsDecryptCommandHandler : SopsCommandBaseHandler
{
    public SopsDecryptCommandHandler()
        : base("--decrypt")
    {
    }
}