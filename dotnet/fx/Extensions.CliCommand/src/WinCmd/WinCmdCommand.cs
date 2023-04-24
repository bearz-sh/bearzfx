using Bearz.Std;

namespace Bearz.Extensions.CliCommand.WinCmd;

public class WinCmdCommand : ShellCliCommand
{
    public WinCmdCommand(ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base("cmd", context, startInfo)
    {
        if (!Env.IsWindows())
            throw new PlatformNotSupportedException("WinCmd is only supported on Windows.");
    }

    public override string Extension => ".cmd";

    public static WinCmdCommand Create()
        => new();

    public static WinCmdCommand Create(CommandStartInfo startInfo)
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