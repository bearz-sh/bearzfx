using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

using Spectre.Console;

namespace Casa.Commands.Sops;

public class SopsEncryptCommand : SopsCommandBase
{
    public SopsEncryptCommand()
        : base("encrypt", "Encrypts a file. By default the result will be sent to standard out.")
    {
    }
}

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class SopsEncryptCommandHandler : SopsCommandBaseHandler
{
    public SopsEncryptCommandHandler()
        : base("--encrypt")
    {
    }
}