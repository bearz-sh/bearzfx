using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.Json;

using Bearz.Extra.Object;
using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;

namespace Plank.Package.Actions;

public class PlankVariables
{
    private readonly Dictionary<string, object?> variables;

    public PlankVariables()
    {
        this.variables = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["tag"] = "latest",
            ["restart"] = "unless-stopped",
            ["tz"] = "Universal",
            ["puid"] = "0",
            ["guid"] = "0",
            ["host"] = new Dictionary<string, object?>()
            {
                ["ip"] = "127.0.0.1",
                ["domain"] = "plank.home",
                ["name"] = null,
                ["ports"] = Array.Empty<string>(),
            },
            ["network"] = new Dictionary<string, object?>()
            {
                ["name"] = "plank",
                ["ipv4"] = null,
                ["enabled"] = false,
            },
            ["watchtower"] = false,
            ["os"] = new Dictionary<string, object?>()
                {
                    ["windows"] = Env.IsWindows,
                    ["linux"] = Env.IsLinux,
                    ["darwin"] = Env.IsMacOS,
                    ["macos"] = Env.IsMacOS,
                    ["freebsd"] = RuntimeInformation.IsOSPlatform(OSPlatform.Create("FreeBSD")),
                },
            ["localIp"] = GetLocalIp(),
        };
    }

    public object? this[string key]
    {
        get
        {
            if (this.variables.TryGetValue(key, out var value))
                return value;

            return default;
        }
        set => this.variables[key] = value;
    }

    public PlankVariables Add(IConfiguration config)
    {
        ProcessSection(config.GetChildren(), this.variables);
        return this;
    }

    public PlankVariables Add(IPathSpec paths)
    {
        this.variables["paths"] = new Dictionary<string, object?>()
        {
            ["plank"] = paths.PlankRootDir,
            ["etc"] = paths.EtcDir,
            ["data"] = paths.DataDir,
            ["log"] = paths.LogDir,
            ["run"] = paths.RunDir,
            ["bin"] = paths.BinDir,
        };

        return this;
    }

    public PlankVariables Add(PlankSpec spec)
    {
        this.variables["name"] = spec.Name;
        var host = (IDictionary<string, object?>)this.variables["host"]!;
        host["cname"] = spec.Name;

        this.variables["spec"] = new Dictionary<string, object>()
        {
            ["name"] = spec.Name,
            ["version"] = spec.Version,
            ["labels"] = spec.Labels,
            ["deps"] = spec.Deps,
        };

        return this;
    }

    public PlankVariables AddVarFiles(IReadOnlyCollection<string> varFiles)
    {
        if (varFiles.Count == 0)
        {
            return this;
        }

        var builder = new ConfigurationBuilder();
        foreach (var file in varFiles)
        {
            if (!Fs.FileExists(file))
            {
                if (!file.EndsWith(".yml"))
                    continue;

                var secondary = FsPath.ChangeExtension(file, ".yaml");
                if (!Fs.FileExists(secondary))
                    continue;

                builder.AddYamlFile(secondary, true, false);
                continue;
            }

            if (file.EndsWith(".yaml") || file.EndsWith("yml"))
            {
                builder.AddYamlFile(file, true, false);
                continue;
            }

            if (file.EndsWith(".json") || file.EndsWith(".jsonc"))
            {
                builder.AddJsonFile(file, true, false);
            }
        }

        var config = builder.Build();
        return this.Add(config);
    }

    public PlankVariables Dump()
    {
        var json = JsonSerializer.Serialize(this.variables, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        Console.WriteLine(json);
        return this;
    }

    public PlankVariables LoadAsEnvironmentVariables()
    {
        ProcessVariables(string.Empty, this.variables);
        return this;
    }

    public IDictionary<string, object?> ToDictionary()
    {
        return this.variables;
    }

    private static void ProcessVariables(string baseKey, IDictionary<string, object?> map)
    {
        foreach (var kvp in map)
        {
            if (kvp.Value is null)
            {
                continue;
            }

            var key = kvp.Key;
            if (!baseKey.IsNullOrWhiteSpace())
                key = $"{baseKey.ToUpperInvariant()}_{key.ToUpperInvariant()}";

            if (kvp.Value is IDictionary<string, object?> childMap)
            {
                ProcessVariables(key, childMap);
                continue;
            }

            if (kvp.Value is IList<object?> list)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var value = list[i];
                    Env.Set($"{key}_{i}", value.ToSafeString());
                }

                continue;
            }

            if (kvp.Value is Array array)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    var value = array.GetValue(i);
                    Env.Set($"{key}_{i}", value.ToSafeString());
                }

                continue;
            }

            Env.Set(key, kvp.Value.ToSafeString());
        }
    }

    private static void ProcessSection(IEnumerable<IConfigurationSection> children, Dictionary<string, object?> context)
    {
        foreach (var child in children)
        {
            if (child.Value is not null)
            {
                var value = Env.Expand(child.Value);
                context[child.Key] = value;
            }
            else
            {
                var childContext = new Dictionary<string, object?>();
                context[child.Key] = childContext;
                ProcessSection(child.GetChildren(), childContext);
            }
        }
    }

    private static string? GetLocalIp()
    {
        string? localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("1.1.1.1", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint?.Address.ToString();
        }

        return localIP;
    }
}