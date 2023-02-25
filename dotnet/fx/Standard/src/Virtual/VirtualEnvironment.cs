using System.Runtime.InteropServices;

using Bearz.Secrets;
using Bearz.Std;

namespace Bearz.Virtual;

public class VirtualEnvironment : IEnvironment
{
    private readonly Dictionary<string, string> env;

    public VirtualEnvironment()
        : this(Env.GetAll(), Bearz.Secrets.SecretMasker.Default)
    {
    }

    public VirtualEnvironment(IDictionary<string, string> env, ISecretMasker masker)
    {
        this.env = new Dictionary<string, string>(env, StringComparer.OrdinalIgnoreCase);
        this.SecretMasker = masker;
        this.Path = new VirtualEnvironmentPath(this);
        this.Cwd = System.Environment.CurrentDirectory;
    }

    public IEnumerable<string> VariableNames => this.env.Keys;

    public string Cwd { get; set; }

    public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public bool IsProcess64Bit => System.Environment.Is64BitProcess;

    public bool IsOs64Bit => System.Environment.Is64BitOperatingSystem;

    public bool IsUserInteractive { get; set; } = Env.IsUserInteractive;

    public bool IsUserElevated { get; } = Env.IsUserElevated;

    public string HomeDir => this.Directory(SpecialDirectory.UserProfile);

    public Architecture OsArch { get; } = RuntimeInformation.OSArchitecture;

    public Architecture ProcessArch { get; } = RuntimeInformation.ProcessArchitecture;

    public IEnvironmentPath Path { get; }

    public ISecretMasker SecretMasker { get; }

    public string TmpDir => System.IO.Path.GetTempPath();

    public string User => System.Environment.UserName;

    public string UserDomain => System.Environment.UserDomainName;

    public string HostName => System.Environment.MachineName;

    public string? this[string key]
    {
        get => this.Get(key);
        set
        {
            if (value is null)
            {
                this.env.Remove(key);
            }
            else
            {
                this.env[key] = value;
            }
        }
    }

    public string? Get(string variableName)
    {
        return !this.env.TryGetValue(variableName, out var value) ? null : value;
    }

    public bool Has(string variableName)
        => this.env.ContainsKey(variableName);

    public IDictionary<string, string> List()
        => new Dictionary<string, string>(this.env, StringComparer.OrdinalIgnoreCase);

    public void Delete(string variableName)
        => this.env.Remove(variableName);

    public string Directory(SpecialDirectory directory)
        => Env.Directory(directory);

    public string Directory(string directoryName)
        => Env.Directory(directoryName);

    public string Expand(string template, bool useWindows = true)
        => Env.Expand(template, useWindows, (name) =>
        {
            if (!Env.TryGet(name, out var value))
                return null;

            return value;
        });

    public void Set(string variableName, string value)
    {
        this.env[variableName] = value;
    }

    public void Set(string variableName, string value, bool secret)
    {
        if (secret)
        {
            this.SecretMasker.Add(value);
        }

        this.env[variableName] = value;
    }

    public bool TryGet(string variableName, out string? value)
    {
        value = null;
        if (this.env.TryGetValue(variableName, out var v))
        {
            value = v;
            return true;
        }

        return false;
    }
}