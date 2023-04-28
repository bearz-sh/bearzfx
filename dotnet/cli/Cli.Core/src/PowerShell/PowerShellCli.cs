using Bearz.Extensions.CliCommand.Pwsh;
using Bearz.Std;

namespace Bearz.Extensions.CliCommand.PowerShell;

public class PowerShellCommand : ShellCliCommand
{
    public PowerShellCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("powershell", context, startInfo)
    {
        if (!Env.IsWindows())
            throw new PlatformNotSupportedException("PowerShell is only supported on Windows.");

        this.WindowsPaths = new[]
        {
            "%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
        };
    }

    public override string Extension => ".ps1";

    public static PwshCli Create()
        => new();

    public static PwshCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs()
        {
            "-NoProfile",
            "-NoLogo",
            "-NonInteractive",
            "-ExecutionPolicy",
            "Bypass",
            "-Command",
            $". '{tempFile}'",
        };
    }

    protected override string GenerateScriptFile(string script)
    {
        script = $@"
$ErrorActionPreference = 'Stop';
{script}
if((Test-Path -LiteralPath variable:LASTEXITCODE))
{{
    exit $LASTEXITCODE;
}}
";
        return base.GenerateScriptFile(script);
    }
}