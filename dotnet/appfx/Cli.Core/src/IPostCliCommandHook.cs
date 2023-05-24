using Bearz.Std;

namespace Bearz.Cli;

public interface IPostCliCommandHook
{
    void Next(string executable, CliCommand command, CommandOutput output);
}