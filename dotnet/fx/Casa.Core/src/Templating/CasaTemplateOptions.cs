using Bearz.Extensions.Secrets;

namespace Bearz.Casa.Core.Templating;

public class CasaTemplateOptions
{
    public string StackDirectory { get; set; } = string.Empty;

    public string Environment { get; set; } = string.Empty;

    public ISecretVault Vault { get; set; } = NullSecretVault.Instance;

    public bool Overwrite { get; set; }
}