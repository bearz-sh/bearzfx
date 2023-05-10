using System.Net;
using System.Net.Sockets;
using System.Text.Json;

using Bearz.Extra.Object;
using Bearz.Extra.Strings;
using Bearz.Std;

using Microsoft.Extensions.Configuration;

namespace Plank.Tasks;

public class Variables
{
    private readonly Dictionary<string, object?> variables;

    public Variables()
    {
        this.variables = new Dictionary<string, object?>()
        {
            ["os"] = new Dictionary<string, object?>()
            {
                ["windows"] = Env.IsWindows,
                ["linux"] = Env.IsLinux,
                ["mac"] = Env.IsMacOS,
                ["darwin"] = Env.IsMacOS,
            },
            ["process"] = new Dictionary<string, object?>()
            {
                ["pid"] = Env.Process.Id,
                ["elevated"] = Env.IsUserElevated,
                ["is64bit"] = Env.IsProcess64Bit,
            },
            ["env"] = new Dictionary<string, object?>()
            {
                ["cwd"] = Env.Cwd,
                ["home"] = Env.HomeDirectory,
                ["user"] = Env.User,
                ["host"] = Env.HostName,
            },
        };
    }

    public Variables(Variables variables)
    {
        this.variables = new Dictionary<string, object?>(variables.variables);
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

    public Variables Add(IConfiguration config)
    {
        ProcessSection(config.GetChildren(), this.variables);
        return this;
    }

    public Variables Add(Dictionary<string, object?> dictionary)
    {
        ProcessVariables(string.Empty, dictionary);
        return this;
    }

    public Variables AddVarFiles(IReadOnlyCollection<string> varFiles)
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

    public Variables Dump()
    {
        var json = JsonSerializer.Serialize(this.variables, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        Console.WriteLine(json);
        return this;
    }

    public Variables LoadAsEnvironmentVariables()
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