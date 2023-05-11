using Bearz.Std;

using Ze.Package.Actions;

namespace Ze.Package;

public class ZeExtractedPackage
{
    public ZeExtractedPackage(
        string packageDir,
        IPathSpec paths,
        string? target,
        IReadOnlyCollection<string>? varFiles)
    {
        if (!Fs.DirectoryExists(packageDir))
            throw new DirectoryNotFoundException($"Unable to find package directory '{packageDir}'");

        target ??= "local";
        this.Target = target;
        this.Paths = paths;
        this.TemplatesDirectory = FsPath.Combine(packageDir, "templates");
        var ymlExtensions = new[] { ".yml", ".yaml" };
        string? plankSpecFile = FsPath.Combine(packageDir, "plankspec.yml");
        string? varsFile = FsPath.Combine(packageDir, "vars.yml");
        string? secretsFile = FsPath.Combine(packageDir, "secrets.yml");

        this.GlobalEnvFile = FsPath.Combine(paths.EtcDir, ".secrets", ".env");
        this.EnvFile = FsPath.Combine(paths.EtcDir, ".secrets", $"{target}.env");

        plankSpecFile = Fs.GetExistingFile(plankSpecFile, ymlExtensions);
        varsFile = Fs.GetExistingFile(varsFile, new[] { ".yml", ".yaml", ".json", ".jsonc" });
        secretsFile = Fs.GetExistingFile(secretsFile, ymlExtensions);

        if (plankSpecFile is null)
            throw new FileNotFoundException($"Unable to find plankspec.yml file in {packageDir}");

        if (varsFile is null)
            throw new FileNotFoundException($"Unable to find vars.yml in {packageDir}");

        this.Spec = ZeSpec.ParseFile(plankSpecFile);
        this.Variables = new ZeVariables()
            .Add(this.Spec)
            .Add(paths);

        if (varFiles is null || varFiles.Count == 0)
        {
            var confDir = Env.GetAppFolder(AppFolder.UserConfig, "plank", true, option: EnvFolderOption.DoNotVerify);
            var app = this.Spec.Name;
            varFiles = new[]
            {
                FsPath.Combine(packageDir, "vars.yml"),
                FsPath.Combine(packageDir, $"{target}.vars.yml"),
                FsPath.Combine(paths.EtcDir, ".vars", "vars.yml"),
                FsPath.Combine(paths.EtcDir, ".vars", $"{target}.vars.yml"),
                FsPath.Combine(paths.EtcDir, ".vars", app, "vars.yml"),
                FsPath.Combine(paths.EtcDir, ".vars", app, $"{target}.vars.yml"),
                FsPath.Combine(confDir, "vars.yml"),
                FsPath.Combine(confDir, $"{target}.vars.yml"),
                FsPath.Combine(confDir, app,  $"vars.yml"),
                FsPath.Combine(confDir, app,  $"{target}.vars.yml"),
            };
        }

        var existingVarFiles = Fs.GetExistingFiles(varFiles, ymlExtensions);
        foreach (var file in existingVarFiles)
            Console.WriteLine(file);
        this.Variables.AddVarFiles(existingVarFiles);

        this.Variables.Dump();

        if (secretsFile is not null)
            this.Secrets = SecretSpec.ParseFile(secretsFile);
    }

    public ZeSpec Spec { get; set; }

    public Dictionary<string, SecretSpec> Secrets { get; set; } = new();

    public ZeVariables Variables { get; }

    public string TemplatesDirectory { get; }

    public string Target { get; }

    public IPathSpec Paths { get; set; }

    public string GlobalEnvFile { get; }

    public string EnvFile { get; }
}