namespace Ze.Package.Actions;

public class InstallActionOptions
{
    public InstallActionOptions(ZeExtractedPackage package)
    {
        this.Package = package;
    }

    public ZeExtractedPackage Package { get; set; }

    public string Target { get; set; } = "local";

    public bool Force { get; set; }

    public IPathSpec Paths { get; set; } = PathSpec.Create();

    public IReadOnlyList<string> VarFiles { get; set; } = Array.Empty<string>();
}