using Bearz.Std;

namespace Bearz.Cli.PowerShell;

public class PowerShellCli : ShellCliCommand
{
    public PowerShellCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("powershell", context, startInfo)
    {
        if (!Env.IsWindows)
            throw new PlatformNotSupportedException("PowerShell is only supported on Windows.");

        this.WindowsPaths = new[]
        {
            "%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe",
        };
    }

    public override string Extension => ".ps1";

    public static PowerShellCli Create()
        => new();

    public static PowerShellCli Create(CommandStartInfo startInfo)
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