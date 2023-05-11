using System.CommandLine.Invocation;

using Bearz.Extensions.Hosting.CommandLine;
using Bearz.Std;
using Bearz.Std.Unix;

using Microsoft.Extensions.Configuration;

using Ze.Package.Actions;

using Command = System.CommandLine.Command;

namespace Ze.Commands;

[CommandHandler(typeof(InitCommandHandler))]
public class InitCommand : Command
{
    public InitCommand()
        : base("init", "initializes plank")
    {
    }
}

public class InitCommandHandler : ICommandHandler
{
    public InitCommandHandler(IConfiguration config)
    {
        this.Config = config;
        this.AppDirectory = config.GetValue<string?>("appDirectory") ?? Paths.AppDirectory;
        this.PackagesDirectory =
            config.GetValue<string?>("packagesDirectory") ?? Path.Join(Paths.AppDirectory, "packages");

        this.PathSpec = new PathSpec(this.AppDirectory);
    }

    protected IConfiguration Config { get; }

    protected string AppDirectory { get; }

    protected string PackagesDirectory { get; }

    protected PathSpec PathSpec { get; }

    public int Invoke(InvocationContext context)
    {
        throw new NotImplementedException();
    }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        if (!Directory.Exists(this.AppDirectory))
        {
            Fs.MakeDirectory(this.AppDirectory);
        }

        Console.WriteLine("windows " + Env.IsWindows);

        if (!Env.IsWindows)
        {
            Env.TryGet("SUDO_UID", out var uid);
            if (!int.TryParse(uid, out var id))
                id = UnixUser.UserId ?? 0;

            var (dockerId, dockerGroupId) = UnixUser.GetUserAndGroupIds("docker");
            if (!dockerGroupId.HasValue)
            {
                var lines = Fs.ReadAllLines("/etc/group");
                foreach (var line in lines)
                {
                    if (line.StartsWith("docker"))
                    {
                        var parts = line.Split(":");
                        if (parts.Length > 2 && int.TryParse(parts[2], out var d2))
                        {
                            dockerGroupId = (uint)d2;
                        }
                    }
                }
            }

            Console.WriteLine($"{id},{dockerId}, {dockerGroupId}");
            if (dockerGroupId.HasValue)
                Fs.Chown(this.AppDirectory, id, (int)(dockerGroupId!));

            var mod = UnixFileMode.GroupExecute | UnixFileMode.GroupRead | UnixFileMode.GroupWrite |
                       UnixFileMode.OtherRead |
                      UnixFileMode.UserExecute | UnixFileMode.UserRead | UnixFileMode.UserWrite;
            Fs.Chmod(this.AppDirectory, (int)mod);
        }

        return Task.FromResult(0);
    }
}