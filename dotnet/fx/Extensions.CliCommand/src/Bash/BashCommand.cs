using System.Runtime.InteropServices;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Bash;

public class BashCommand : ShellCliCommand
{
    private bool? isWslBash;

    public BashCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("bash", context, startInfo)
    {
        this.WindowsPaths = new[]
        {
            "%ProgramFiles%\\Git\\bin\\bash.exe",
            "%ProgramFiles%\\Git\\usr\\bin\\bash.exe",
            "%WINDIR%\\System32\\bash.exe",
        };
    }

    public override string Extension => ".sh";

    protected bool IsWslBash
    {
        get
        {
            this.isWslBash ??= RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                               Env.Process
                                   .Which("bash")?
                                   .EqualsIgnoreCase("c:\\windows\\system32\\bash.exe") == true;

            return this.isWslBash.Value;
        }
    }

    public static BashCommand Create()
        => new BashCommand();

    public static BashCommand Create(CommandStartInfo startInfo)
        => new BashCommand(null, startInfo);

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs()
        {
            "--noprofile",
            "--norc",
            "-e",
            "-o",
            "pipefail",
            tempFile,
        };
    }

    protected override string GenerateScriptFile(string script)
    {
        var tempFile = base.GenerateScriptFile(script);

        if (!this.IsWslBash)
            return tempFile;

        var updated = tempFile.Substring(1).Replace(":", string.Empty).Replace("\\", "/");
        return $"/mnt/c/{updated}";
    }
}