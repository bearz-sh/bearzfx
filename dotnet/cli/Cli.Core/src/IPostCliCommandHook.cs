using Bearz.Std;

namespace Bearz.Extensions.CliCommand;

public interface IPostCliCommandHook
{
    void Next(string executable, CliCommand command, CommandOutput output);
}