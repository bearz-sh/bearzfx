namespace Bearz.Extensions.CliCommand;

public interface IPreCliCommandHook
{
    void Next(CliCommand command);
}