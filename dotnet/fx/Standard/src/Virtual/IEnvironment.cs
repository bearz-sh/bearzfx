using System;
using System.Linq;
using System.Runtime.InteropServices;

using Bearz.Secrets;
using Bearz.Std;

namespace Bearz.Virtual;

public interface IEnvironment
{
    IEnumerable<string> VariableNames { get; }

    string Cwd { get; set; }

    bool IsWindows { get; }

    bool IsLinux { get; }

    bool IsMacOS { get; }

    bool IsProcess64Bit { get; }

    bool IsOs64Bit { get; }

    bool IsUserInteractive { get; set; }

    bool IsUserElevated { get; }

    string HomeDir { get; }

    Architecture OsArch { get; }

    Architecture ProcessArch { get; }

    IEnvironmentPath Path { get; }

    ISecretMasker SecretMasker { get; }

    string TmpDir { get; }

    string User { get; }

    string UserDomain { get; }

    string HostName { get; }

    string? this[string key] { get; set; }

    string? Get(string variableName);

    bool Has(string variableName);

    IDictionary<string, string> List();

    void Delete(string variableName);

    string Directory(SpecialDirectory directory);

    string Directory(string directoryName);

    string Expand(string template, bool useWindows = true);

    void Set(string variableName, string value);

    void Set(string variableName, string value, bool secret);

    bool TryGet(string variableName, out string? value);
}