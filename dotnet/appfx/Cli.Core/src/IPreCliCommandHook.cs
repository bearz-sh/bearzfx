namespace Bearz.Cli;

public interface IPreCliCommandHook
{
    void Next(CliCommand command);
}