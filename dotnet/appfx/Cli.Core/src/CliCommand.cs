using System.Runtime.InteropServices;

using Bearz.Diagnostics;
using Bearz.Extra.Strings;
using Bearz.Std;

namespace Bearz.Cli;

public abstract class CliCommand : CommandBase
{
    private static List<IPreCliCommandHook>? s_preCliCommandHooks = null;

    private static List<IPostCliCommandHook>? s_postCliCommandHooks = null;

    private List<IPreCliCommandHook>? preCliCommandHooks = null;

    private List<IPostCliCommandHook>? postCliCommandHooks = null;

    protected CliCommand(string name, ICliExecutionContext? context = null, CommandStartInfo? startInfo = null)
    {
        this.Name = name;
        this.Context = context;
        this.StartInfo = startInfo ?? new CommandStartInfo();
    }

    public override string FileName => this.Location ?? string.Empty;

    public override CommandStartInfo StartInfo { get; }

    public string Name { get; protected set; }

    protected string? Location { get; set; }

    protected ICliExecutionContext? Context { get; set; }

    protected string? ScriptFile { get; set; }

    protected IReadOnlyList<string> WindowsPaths { get; set; } = Array.Empty<string>();

    protected IReadOnlyList<string> LinuxPaths { get; set; } = Array.Empty<string>();

    protected IReadOnlyList<string> DarwinPaths { get; set; } = Array.Empty<string>();

    public static void AddGlobalPreCliCommandHook(IPreCliCommandHook hook)
    {
        s_preCliCommandHooks ??= new List<IPreCliCommandHook>();
        s_preCliCommandHooks.Add(hook);
    }

    public static void AddGlobalPostCliCommandHook(IPostCliCommandHook hook)
    {
        s_postCliCommandHooks ??= new List<IPostCliCommandHook>();
        s_postCliCommandHooks.Add(hook);
    }

    public CliCommand AddPreCliCommandHook(IPreCliCommandHook hook)
    {
        this.preCliCommandHooks ??= new List<IPreCliCommandHook>();
        this.preCliCommandHooks.Add(hook);
        return this;
    }

    public CliCommand AddPostCliCommandHook(IPostCliCommandHook hook)
    {
        this.postCliCommandHooks ??= new List<IPostCliCommandHook>();
        this.postCliCommandHooks.Add(hook);
        return this;
    }

    public override CommandOutput Output()
    {
        try
        {
            _ = this.WhichOrThrow();
            if (s_preCliCommandHooks is not null)
            {
                foreach (var hook in s_preCliCommandHooks)
                {
                    hook.Next(this);
                }
            }

            if (this.preCliCommandHooks is not null)
            {
                foreach (var hook in this.preCliCommandHooks)
                {
                    hook.Next(this);
                }
            }

            var preHookServices = this.Context?.Services.GetService(typeof(IEnumerable<IPreCliCommandHook>));
            if (preHookServices is IEnumerable<IPreCliCommandHook> preHooks)
            {
                foreach (var hook in preHooks)
                {
                    hook.Next(this);
                }
            }

            var output = base.Output();

            if (s_postCliCommandHooks is not null)
            {
                foreach (var hook in s_postCliCommandHooks)
                {
                    hook.Next(this.FileName, this, output);
                }
            }

            if (this.postCliCommandHooks is not null)
            {
                foreach (var hook in this.postCliCommandHooks)
                {
                    hook.Next(this.FileName, this, output);
                }
            }

            var hookServices = this.Context?.Services.GetService(typeof(IEnumerable<IPostCliCommandHook>));
            if (hookServices is IEnumerable<IPostCliCommandHook> hooks)
            {
                foreach (var hook in hooks)
                {
                    hook.Next(this.FileName, this, output);
                }
            }

            return output;
        }
        finally
        {
            if (this.ScriptFile is not null)
            {
                if (this.Context is not null)
                {
                    if (this.Context.Fs.FileExists(this.ScriptFile))
                        this.Context.Fs.RemoveFile(this.ScriptFile);
                }
                else
                {
                    if (Fs.FileExists(this.ScriptFile))
                        Fs.RemoveFile(this.ScriptFile);
                }
            }
        }
    }

    public override async Task<CommandOutput> OutputAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _ = this.WhichOrThrow();
            if (s_preCliCommandHooks is not null)
            {
                foreach (var hook in s_preCliCommandHooks)
                {
                    hook.Next(this);
                }
            }

            if (this.preCliCommandHooks is not null)
            {
                foreach (var hook in this.preCliCommandHooks)
                {
                    hook.Next(this);
                }
            }

            var output = await base.OutputAsync(cancellationToken);

            if (s_postCliCommandHooks is not null)
            {
                foreach (var hook in s_postCliCommandHooks)
                {
                    hook.Next(this.FileName, this, output);
                }
            }

            if (this.postCliCommandHooks is not null)
            {
                foreach (var hook in this.postCliCommandHooks)
                {
                    hook.Next(this.FileName, this, output);
                }
            }

            return output;
        }
        finally
        {
            if (this.ScriptFile is not null)
            {
                if (this.Context is not null)
                {
                    if (this.Context.Fs.FileExists(this.ScriptFile))
                        this.Context.Fs.RemoveFile(this.ScriptFile);
                }
                else
                {
                    if (Fs.FileExists(this.ScriptFile))
                        Fs.RemoveFile(this.ScriptFile);
                }
            }
        }
    }

    public string? Which()
    {
        string? exe = this.Location;
        if (!exe.IsNullOrWhiteSpace())
            return exe;

        var envName = $"{this.Name.Replace("-", "_").ToUpperInvariant()}_EXE";

        if (this.Context is not null)
        {
            if (this.Context.Env.TryGet(envName, out exe) && this.Context.Fs.FileExists(exe))
            {
                this.Location = exe;
                return this.Location;
            }

            exe = this.Context.Process.Which(this.Name, null, true);
            if (exe is not null)
            {
                this.Location = exe;
                return exe;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (var path in this.WindowsPaths)
                {
                    exe = this.Context.Env.Expand(path);
                    if (!this.Context.Fs.FileExists(exe))
                    {
                        continue;
                    }

                    this.Location = exe;
                    return exe;
                }

                return null;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                foreach (var path in this.DarwinPaths)
                {
                    exe = this.Context.Env.Expand(path);
                    if (!this.Context.Fs.FileExists(exe))
                    {
                        continue;
                    }

                    this.Location = exe;
                    return exe;
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                foreach (var path in this.LinuxPaths)
                {
                    exe = this.Context.Env.Expand(path);
                    if (!this.Context.Fs.FileExists(exe))
                    {
                        continue;
                    }

                    this.Location = exe;
                    return exe;
                }
            }

            return null;
        }

        if (Env.TryGet(envName, out exe) && Fs.FileExists(exe))
        {
            this.Location = exe;
            return this.Location;
        }

        exe = Env.Process.Which(this.Name, null, true);
        if (exe is not null)
        {
            this.Location = exe;
            return exe;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            foreach (var path in this.WindowsPaths)
            {
                exe = Env.Expand(path);
                if (!File.Exists(exe))
                {
                    continue;
                }

                this.Location = exe;
                return exe;
            }

            return null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            foreach (var path in this.DarwinPaths)
            {
                exe = Env.Expand(path);
                if (!File.Exists(exe))
                {
                    continue;
                }

                this.Location = exe;
                return exe;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            foreach (var path in this.LinuxPaths)
            {
                exe = Env.Expand(path);
                if (!File.Exists(exe))
                {
                    continue;
                }

                this.Location = exe;
                return exe;
            }
        }

        return null;
    }

    public virtual string WhichOrThrow()
    {
        var exe = this.Which();
        if (exe is null)
        {
            throw new NotFoundOnPathException(this.Name);
        }

        return exe;
    }
}