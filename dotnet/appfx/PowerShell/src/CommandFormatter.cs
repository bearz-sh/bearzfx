using Bearz.Extra.Strings;
using Bearz.Secrets;
using Bearz.Std;

namespace Bearz.PowerShell;

public static class CommandFormatter
{
    public static Func<string, CommandArgs?, (string, bool)>? MessageFormatter { get; set; }

    public static (string, bool) FormatMessage(string command, CommandArgs? args = null)
    {
        if (MessageFormatter is not null)
        {
            return MessageFormatter(command, args);
        }

        args ??= new CommandArgs();
        var message = $"{command} {args}";
        message = SecretMasker.Default.Mask(message);

        if (Env.Get("TF_BUILD")?.EqualsIgnoreCase("true") == true)
        {
            return ($"##[command]{message}", false);
        }
        else if (Env.Get("GITHUB_ACTIONS")?.EqualsIgnoreCase("true") == true)
        {
            return ($"::notice::{message}", false);
        }
        else
        {
            return (message, true);
        }
    }
}