using Bearz.Std;

namespace Casa.Commands.Sops;

public class SopsCommand : SopsCommandBase
{
    public SopsCommand()
        : base("sops", "casa commands to run the sops command line tool in a more intuitive way. The default action is to encrypt.")
    {
    }
}