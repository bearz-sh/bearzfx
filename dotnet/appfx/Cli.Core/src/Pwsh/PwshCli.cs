using Bearz.Std;

namespace Bearz.Cli.Pwsh;

public class PwshCli : ShellCliCommand
{
    public PwshCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
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

    public static PwshCli Create()
        => new PwshCli();

    public static PwshCli Create(CommandStartInfo startInfo)
        => new PwshCli(null, startInfo);

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        var args = new CommandArgs()
        {
            "-NoProfile",
            "-NoLogo",
            "-NonInteractive",
            "-ExecutionPolicy",
            "Bypass",
            "-Command",
            $". {tempFile}",
        };

        return args;
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