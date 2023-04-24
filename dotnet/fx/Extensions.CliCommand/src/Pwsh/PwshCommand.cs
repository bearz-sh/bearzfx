using Bearz.Std;

namespace Bearz.Extensions.CliCommand.Pwsh;

public class PwshCommand : ShellCliCommand
{
    public PwshCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("pwsh", context, startInfo)
    {
        this.WindowsPaths = new[]
        {
            "%ProgramFiles%\\PowerShell\\7\\pwsh.exe",
            "%ProgramFiles(x86)%\\PowerShell\\7\\pwsh.exe",
            "%ProgramFiles%\\PowerShell\\6\\pwsh.exe",
            "%ProgramFiles(x86)%\\PowerShell\\6\\pwsh.exe",
        };

        this.LinuxPaths = new[]
        {
            "/opt/microsoft/powershell/7/pwsh",
            "/opt/microsoft/powershell/6/pwsh",
        };
    }

    public override string Extension => ".ps1";

    public static PwshCommand Create()
        => new PwshCommand();

    public static PwshCommand Create(CommandStartInfo startInfo)
        => new PwshCommand(null, startInfo);

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