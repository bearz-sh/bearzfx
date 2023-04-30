using Cocoa.Domain;

namespace Cocoa.Configuration.Sections;

[Serializable]
public sealed class ApiKeyCommandConfiguration
{
    public string? Key { get; set; }

    public ApiKeyCommandType Command { get; set; }
}