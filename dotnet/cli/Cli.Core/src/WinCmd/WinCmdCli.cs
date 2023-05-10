using Bearz.Std;

namespace Bearz.Cli.WinCmd;

public class WinCmdCli : ShellCliCommand
{
    public WinCmdCli(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("cmd", context, startInfo)
    {
        if (!Env.IsWindows)
            throw new PlatformNotSupportedException("WinCmd is only supported on Windows.");
    }

    public override string Extension => ".cmd";

    public static WinCmdCli Create()
        => new();

    public static WinCmdCli Create(CommandStartInfo startInfo)
        => new(null, startInfo);

    protected override string GenerateScriptFile(string script)
    {
        script = $@"
@echo off
{script}
";
        return base.GenerateScriptFile(script);
    }

    protected override CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs()
        {
            "/D",
            "/E:ON",
            "/V:OFF",
            "/S",
            "/C",
            "CALL",
            tempFile,
        };
    }
}