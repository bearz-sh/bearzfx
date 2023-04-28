using Bearz.Std;

namespace Bearz.PowerShell.Standard;

public class ScriptResolver
{
    public string Shell { get; set; } = string.Empty;

    public string? ScriptFormat { get; set; }

    public string? LastArgumentFormat { get; set; }

    public string Extension { get; set; } = string.Empty;

    public CommandArgs? ArgumentList { get; set; } = new();
}