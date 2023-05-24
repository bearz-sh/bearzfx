using Bearz.Std;

namespace Bearz.Cli;

public abstract class ShellCliCommand : CliCommand
{
    private static Dictionary<string, Type> s_shellCommands = new(StringComparer.OrdinalIgnoreCase)
    {
        ["deno"] = typeof(Deno.DenoCli),
        ["node"] = typeof(NodeJs.NodeJsCli),
        ["python"] = typeof(Python.PythonCli),
        ["ruby"] = typeof(Ruby.RubyCli),
        ["bash"] = typeof(Bash.BashCli),
        ["sh"] = typeof(Sh.ShCli),
        ["pwsh"] = typeof(Pwsh.PwshCli),
        ["powershell"] = typeof(PowerShell.PowerShellCli),
        ["cmd"] = typeof(WinCmd.WinCmdCli),
    };

    protected ShellCliCommand(string name, ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
        : base(name, context, startInfo)
    {
    }

    public abstract string Extension { get; }

    public static void RegisterShell(string shell, Type type)
    {
        s_shellCommands[shell] = type;
    }

    public static void RegisterShell<T>(string shell)
        where T : ShellCliCommand
    {
        s_shellCommands[shell] = typeof(T);
    }

    public static CommandOutput RunScript(
        string shell,
        string script,
        Action<CommandStartInfo>? configure)
    {
        if (!s_shellCommands.TryGetValue(shell, out var type))
            throw new InvalidOperationException($"Invalid shell type {shell}");

        var command = (ShellCliCommand)Activator.CreateInstance(type)!;
        configure?.Invoke(command.StartInfo);
        command.WithScript(script);
        return command.Output();
    }

    public static async Task<CommandOutput> RunScriptAsync(
        string shell,
        string script,
        Action<CommandStartInfo>? configure,
        ICliExecutionContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var si = new CommandStartInfo();
        configure?.Invoke(si);
        if (!s_shellCommands.TryGetValue(shell, out var type))
            throw new InvalidOperationException($"Invalid shell type {shell}");

        var command = (ShellCliCommand)Activator.CreateInstance(type, new object?[] { context, si })!;
        configure?.Invoke(command.StartInfo);
        command.WithScript(script);
        var result = await command.OutputAsync(cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    public ShellCliCommand WithScript(string script)
    {
        var tempFile = this.GenerateScriptFile(script);
        var args = this.GenerateScriptArgs(tempFile);
        this.StartInfo.Args = args;
        this.ScriptFile = tempFile;

        return this;
    }

    protected virtual string GenerateScriptFile(string script)
    {
        var fileName = FsPath.GetRandomFileName();
        if (this.Context is not null)
        {
            var dir = this.Context.Path.TempDir();
            var file = Path.Combine(dir, $"{fileName}.{this.Extension}");
            this.Context.Fs.WriteFileText(file, script);
            return file;
        }
        else
        {
            var dir = FsPath.GetTempDir();
            var file = Path.Combine(dir, $"{fileName}.{this.Extension}");
            Fs.WriteTextFile(file, script);
            return file;
        }
    }

    protected virtual CommandArgs GenerateScriptArgs(string tempFile)
    {
        return new CommandArgs() { tempFile };
    }
}