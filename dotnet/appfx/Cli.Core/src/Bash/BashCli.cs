using System.Runtime.InteropServices;

using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.Cli.Bash;

public class BashCli : ShellCliCommand
{
    private bool? isWslBash;

    public BashCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
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

    public static BashCli Create()
        => new BashCli();

    public static BashCli Create(CommandStartInfo startInfo)
        => new BashCli(null, startInfo);

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