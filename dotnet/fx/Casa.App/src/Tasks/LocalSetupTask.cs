using System.Diagnostics;
using System.Security;
using System.Text.Json;

using Bearz.Extensions.CliCommand;
using Bearz.Extensions.CliCommand.Age;
using Bearz.Extra.Strings;
using Bearz.Secrets;
using Bearz.Std;
using Bearz.Std.Unix;

using Process = System.Diagnostics.Process;

namespace Bearz.Casa.App.Tasks;

public class LocalSetupTask
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        var globalDirs = new List<string>()
        {
            Paths.AppDirectory,
            Paths.ConfigDirectory,
            Paths.DataDirectory,
            Paths.LogsDirectory,
            Paths.TemplatesDirectory,
            Paths.ComposeDirectory,
            Paths.SharedComposeDataDirectory,
        };

        if (!Env.IsUserElevated && globalDirs.Any(o => !Fs.DirectoryExists(o)))
        {
            throw new SecurityException(
                "Unable to create global directories. Please run casa as root");
        }

        int userId = 0;
        if (Env.IsLinux() && UnixUser.EffectiveUserId.HasValue)
            userId = UnixUser.EffectiveUserId.Value;

        bool mustChown = Env.TryGet("SUDO_UID", out var uid) && int.TryParse(uid, out userId);

        int casaGroupdId = 0;
        Dictionary<string, int> groups = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (!Env.IsWindows())
        {
            foreach (var line in Fs.ReadAllLines("/etc/group"))
            {
                var parts = line.Split(':');
                if (parts.Length != 4)
                    continue;

                if (int.TryParse(parts[1], out var gid))
                {
                    groups.Add(parts[0], gid);
                }
            }

            if (!groups.TryGetValue("casa", out casaGroupdId))
            {
                new Command("groupadd")
                    .WithArgs("casa")
                    .Output()
                    .ThrowOnInvalidExitCode();

                var user = Env.GetRequired("SUDO_USER");
                new Command("usermod")
                    .WithArgs("-a", "-G", "casa", user)
                    .Output()
                    .ThrowOnInvalidExitCode();

                foreach (var line in Fs.ReadAllLines("/etc/group"))
                {
                    var parts = line.Split(':');

                    if (parts.Length != 4)
                        continue;

                    if (int.TryParse(parts[1], out var gid))
                    {
                        groups[parts[0]] = gid;
                        if (parts[0] == "casa")
                            casaGroupdId = gid;
                    }
                }
            }
        }

        foreach (var dir in globalDirs)
        {
            if (!Fs.DirectoryExists(dir))
            {
                Fs.MakeDirectory(dir);

                // ensure that anyone can read/write logs.
                // no need for execute permissions.
                if (!dir.Equals(Paths.ConfigDirectory))
                {
                    if (!Env.IsWindows())
                    {
                        Fs.Chown(dir, userId, casaGroupdId);
                        Fs.Chmod(dir, 770);
                    }
                    else
                    {
                        Debug.WriteLine("No windows");
                    }
                }
            }
        }

        var userDirectories = new List<string>()
        {
            Paths.UserConfigDirectory,
            Paths.UserDataDirectory,
        };

        foreach (var dir in userDirectories)
        {
            if (!Fs.DirectoryExists(dir))
            {
                Fs.MakeDirectory(dir);
                if (mustChown)
                {
                    Fs.Chown(dir, userId, userId);
                }
            }
        }

        if (Env.IsUserElevated)
        {
            foreach (var dir in globalDirs)
            {
                if (!Fs.DirectoryExists(dir))
                {
                    Fs.MakeDirectory(dir);
                }
            }
        }

        var globalConfigFile = Path.Join(Paths.ConfigDirectory, "casa.config.json");

        if (!Fs.FileExists(globalConfigFile))
        {
            if (Env.IsLinux() && !Env.IsUserElevated)
            {
                throw new SecurityException(
                    "Unable to create global config file. Please run casa as root");
            }

            var cfg = new Dictionary<string, object?>()
            {
                ["compose"] = new Dictionary<string, object?>()
                {
                    ["variables"] = new Dictionary<string, object?>()
                    {
                        ["dirs"] = new Dictionary<string, object>()
                        {
                            ["etc"] = "./etc",
                            ["certs"] = "./certs",
                            ["log"] = "./log",
                            ["data"] = "./data",
                            ["run"] = "./run",
                            ["cache"] = "./cache",
                            ["shared"] = Paths.SharedComposeDataDirectory,
                        },
                        ["tz"] = "America/Chicago",
                        ["guid"] = "0",
                        ["puid"] = "0",
                        ["vnetName"] = "docker-vnet",
                    },
                },
            };

            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions()
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            Fs.WriteTextFile(globalConfigFile, json);
        }

        var encryptKeyFile = Path.Join(Paths.UserDataDirectory, "casa.key");
        if (!Fs.FileExists(encryptKeyFile))
        {
            var key = new SecretGenerator()
                .AddDefaults()
                .GenerateAsBytes(40);

            Fs.WriteFile(encryptKeyFile, key);
            if (!Env.IsWindows())
            {
                if (mustChown)
                {
                    Fs.Chown(encryptKeyFile, userId, userId);
                }

                Fs.Chmod(encryptKeyFile, 600);
            }
        }

        var ageKeyFile = Path.Join(Paths.UserDataDirectory, "casa.age.key");
        if (!Fs.FileExists(ageKeyFile))
        {
            var cmd = new AgeCommand();
            var age = cmd.Which();
            if (!age.IsNullOrWhiteSpace())
            {
                AgeCommand.Create()
                    .WithArgs("-o", ageKeyFile)
                    .WithStdio(Stdio.Inherit)
                    .Output()
                    .ThrowOnInvalidExitCode();

                if (!Env.IsWindows())
                {
                    if (mustChown)
                    {
                        Fs.Chown(ageKeyFile, userId, userId);
                    }

                    Fs.Chmod(encryptKeyFile, 600);
                }
            }
        }

        return Task.CompletedTask;
    }
}