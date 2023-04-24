using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Json;

using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Templates.Handlebars;
using Bearz.Templating.Handlebars;

using DotNet.Globbing;

using HandlebarsDotNet;

using Microsoft.Extensions.Configuration;

namespace Bearz.Casa.Core.Templating;

public class CasaTemplatingEngine
{
    private readonly IHandlebars handlebars;

    private readonly CasaTemplateOptions options;

    public CasaTemplatingEngine(CasaTemplateOptions options)
    {
        this.options = options;
        this.handlebars = Handlebars.Create(new HandlebarsConfiguration());
        this.handlebars.RegisterEnvHelpers();
        this.handlebars.RegisterStringHelpers();
        this.handlebars.RegisterDateTimeHelpers();
        this.handlebars.RegisterJsonHelpers();
        this.handlebars.RegisterSecretHelpers(options.Vault);
    }

    public static void Render(CasaTemplateOptions options)
    {
        var engine = new CasaTemplatingEngine(options);
        engine.Run();
    }

    private void Run()
    {
        var checksumFile = FsPath.Join(this.options.StackDirectory, "checksums.json");
        var checksums = new Dictionary<string, byte[]>();
        var checksumEntries = JsonSerializer.Deserialize<ChecksumEntry[]>(checksumFile);
        if (checksumEntries is not null)
        {
            foreach (var entry in checksumEntries)
            {
                checksums[entry.Path] = Convert.FromBase64String(entry.Checksum);
            }
        }

        var configFiles = new[]
        {
            FsPath.Join(Env.Directory(SpecialDirectory.Etc), "casa", "casa.json"),
            FsPath.Join(Env.Directory(SpecialDirectory.Etc), "casa", $"casa.{this.options.Environment}.json"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", "casa.json"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", $"casa.{this.options.Environment}.json"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", "casa.yaml"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", "casa.yml"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", $"casa.{this.options.Environment}.yaml"),
            FsPath.Join(Env.Directory(SpecialDirectory.ApplicationData), "casa", $"casa.{this.options.Environment}.yml"),
            FsPath.Join(this.options.StackDirectory, "casa", "casa.json"),
            FsPath.Join(this.options.StackDirectory, "casa", $"casa.{this.options.Environment}.json"),
            FsPath.Join(this.options.StackDirectory, "casa", "casa.yaml"),
            FsPath.Join(this.options.StackDirectory, "casa", "casa.yml"),
            FsPath.Join(this.options.StackDirectory, "casa", $"casa.{this.options.Environment}.yaml"),
            FsPath.Join(this.options.StackDirectory, "casa", $"casa.{this.options.Environment}.yml"),
        };

        var builder = new ConfigurationBuilder();
        bool configChanged = false;

        using var sha256 = SHA256.Create();

        foreach (var configFile in configFiles)
        {
            if (File.Exists(configFile))
            {
                var ext = FsPath.Extension(configFile);
                switch (ext)
                {
                    case ".json":
                        builder.AddJsonFile(configFile, true, false);
                        break;

                    case "yml":
                    case ".yaml":
                        builder.AddYamlFile(configFile, true, false);
                        break;
                }

                if (checksums.TryGetValue(configFile, out var checksum))
                {
                    using var stream = File.OpenRead(configFile);
                    var newChecksum = sha256.ComputeHash(stream);
                    if (!checksum.SequenceEqual(newChecksum))
                    {
                        checksums[configFile] = newChecksum;
                        configChanged = true;
                    }
                }
                else
                {
                    using var stream = File.OpenRead(configFile);
                    var newChecksum = sha256.ComputeHash(stream);
                    checksums[configFile] = newChecksum;
                    configChanged = true;
                }
            }
        }

        var config = builder.Build();

        var stackName = config.GetValue<string?>("stack:name");
        if (stackName.IsNullOrWhiteSpace())
        {
            stackName = FsPath.Basename(this.options.StackDirectory);
            stackName = $"{stackName}-{this.options.Environment}";
        }

        Env.Set("STACK_NAME", stackName);
        this.handlebars.RegisterConfHelpers(config);

        var context = new Dictionary<string, object?>()
        {
            ["dirs"] = new Dictionary<string, object>()
            {
                ["etc"] = "./etc",
                ["certs"] = "./certs",
                ["log"] = "./log",
                ["data"] = "./data",
                ["run"] = "./run",
                ["cache"] = "./cache",
            },
            ["environment"] = this.options.Environment,
            ["tz"] = "America/Chicago",
            ["guid"] = "0",
            ["puid"] = "0",
            ["vnetName"] = "docker-vnet",
        };

        var stackStoreDir = config.GetValue<string?>("stacks:store");
        if (stackStoreDir.IsNullOrWhiteSpace())
        {
            stackStoreDir = Path.Join(Env.Directory(SpecialDirectory.Opt), "casa", "stacks");
        }
        else
        {
            stackStoreDir = Env.Expand(stackStoreDir);
        }

        var stackDestDir = Path.Combine(stackStoreDir, stackName);
        Env.Set("STACK_DIR", stackDestDir);
        this.ProcessSection(config.GetSection("stacks:variables"), context);
        var ignoreFile = Path.Join(this.options.StackDirectory, ".casaignore");
        var fileIgnore = new FileIgnore(ignoreFile, this.options.StackDirectory);

        if (context["dirs"] is Dictionary<string, object> dirs)
        {
            foreach (var kvp in dirs)
            {
                var sourceDir = Path.Combine(this.options.StackDirectory, kvp.Key);
                if (!Fs.DirectoryExists(sourceDir))
                    continue;

                if (fileIgnore.IsMatch(sourceDir))
                    continue;

                if (kvp.Value is not string dest)
                {
                    throw new InvalidCastException("Expected string value for 'dirs' key.");
                }

                var destDir = dest;
                if (!FsPath.IsPathRooted(destDir))
                    destDir = Path.GetFullPath(Path.Combine(stackDestDir, dest));

                this.RenderDirectory(sourceDir, destDir, context, checksums, configChanged, this.options.Overwrite, fileIgnore);
            }
        }

        this.RenderRootFiles(this.options.StackDirectory, stackDestDir, context, checksums, configChanged, this.options.Overwrite);
    }

    private void RenderRootFiles(
        string stackDir,
        string destDir,
        Dictionary<string, object?> context,
        Dictionary<string, byte[]> checksums,
        bool configChanged,
        bool overwrite)
    {
        var composeFiles = new List<string>()
        {
            Path.Combine(stackDir, $"{this.options.Environment}.compose.yml.hbs"),
            Path.Combine(stackDir, $"{this.options.Environment}.compose.yaml.hbs"),
            Path.Combine(stackDir, "compose.yml.hbs"),
            Path.Combine(stackDir, "compose.yaml.hbs"),
            Path.Combine(stackDir, $"{this.options.Environment}.docker-compose.yml.hbs"),
            Path.Combine(stackDir, $"{this.options.Environment}.docker-compose.yaml.hbs"),
            Path.Combine(stackDir, "docker-compose.yml.hbs"),
            Path.Combine(stackDir, "docker-compose.yaml.hbs"),
        };

        var composeTemplate = composeFiles.FirstOrDefault(File.Exists);
        var envFiles = new List<string>()
        {
            Path.Combine(stackDir, $"{this.options.Environment}.env.hbs"),
            Path.Combine(stackDir, "env.hbs"),
        };

        envFiles = envFiles.Where(File.Exists).ToList();
        var templates = new List<string>();
        if (composeTemplate != null)
        {
            templates.Add(composeTemplate);
        }

        if (envFiles.Count > 0)
        {
            templates.AddRange(envFiles);
        }

        if (templates.Count == 0)
            return;

        this.ProcessTemplates(
            templates,
            stackDir,
            destDir,
            context,
            checksums,
            configChanged,
            overwrite);
    }

    private void RenderDirectory(
        string sourceDir,
        string destDir,
        Dictionary<string, object?> context,
        Dictionary<string, byte[]> checksums,
        bool configChanged,
        bool overwrite,
        FileIgnore fileIgnore)
    {
        using var sha256 = SHA256.Create();
        var templates = new List<string>();
        var files = Fs.ReadDirectory(sourceDir, "*", SearchOption.AllDirectories);

        var sameDir = sourceDir.Equals(destDir);
        foreach (var file in files)
        {
            if (Fs.IsDirectory(file))
                continue;

            if (FsPath.Extension(file) == ".hbs")
            {
                templates.Add(file);
                continue;
            }

            if (sameDir)
                continue;

            if (fileIgnore.IsMatch(file))
                continue;

            var relative = FsPath.RelativePath(sourceDir, file);
            var dest = FsPath.Join(destDir, relative);
            var dir = FsPath.Dirname(dest)!;
            if (!File.Exists(dest))
            {
                if (!Fs.DirectoryExists(dir))
                    Fs.MakeDirectory(dir);

                using var stream = File.OpenRead(file);
                var checksum = sha256.ComputeHash(stream);
                checksums[dest] = checksum;
                checksums[file] = checksum;
                File.Copy(file, dest);
                continue;
            }

            // skip copy if the file at the destination has changed.
            if (!overwrite && checksums.TryGetValue(dest, out var oldDestChecksum))
            {
                using var stream = File.OpenRead(file);
                var checksum = sha256.ComputeHash(stream);
                if (!checksum.SequenceEqual(oldDestChecksum))
                {
                    continue;
                }
            }

            using var stream2 = File.OpenRead(file);
            var currentSrcChecksum = sha256.ComputeHash(stream2);

            if (!overwrite && checksums.TryGetValue(file, out var oldSrcChecksum)
                           && currentSrcChecksum.SequenceEqual(oldSrcChecksum))
                continue;

            checksums[file] = currentSrcChecksum;
            checksums[dest] = currentSrcChecksum;
            File.Copy(file, dest, true);
        }

        this.ProcessTemplates(
            templates,
            sourceDir,
            destDir,
            context,
            checksums,
            configChanged,
            overwrite);
    }

    private void ProcessTemplates(
        List<string> templates,
        string sourceDir,
        string destDir,
        Dictionary<string, object?> context,
        Dictionary<string, byte[]> checksums,
        bool configChanged,
        bool overwrite)
    {
        using var sha256 = SHA256.Create();
        foreach (var template in templates)
        {
            var relative = FsPath.RelativePath(sourceDir, template);

            // get rid of the .hbs extension
            relative = relative.Substring(0, relative.Length - 4);
            var dest = FsPath.Resolve(FsPath.Join(destDir, relative));

            var dir = FsPath.Dirname(dest)!;

            if (!File.Exists(dest))
            {
                using var stream = File.OpenRead(template);
                var checksum = sha256.ComputeHash(stream);
                checksums[template] = checksum;
                stream.Position = 0;

                using var sr = new StreamReader(stream);
                var compiled = this.handlebars.Compile(sr);

                using var ms = new MemoryStream();
                using var sw = new StreamWriter(ms);
                compiled(sw, context);
                sw.Flush();
                ms.Flush();

                ms.Position = 0;
                var checksum2 = sha256.ComputeHash(ms);
                checksums[dest] = checksum2;
                ms.Position = 0;
                if (!Fs.DirectoryExists(dir))
                    Fs.MakeDirectory(dir);

                using var fs = File.OpenWrite(dest);
                ms.CopyTo(fs);
                fs.Flush();
            }
            else
            {
                using var templateStream = File.OpenRead(template);
                var templateChecksum = sha256.ComputeHash(templateStream);
                templateStream.Position = 0;
                if (!overwrite && !configChanged)
                {
                    if (checksums.TryGetValue(template, out var oldTemplateChecksum) && templateChecksum.SequenceEqual(oldTemplateChecksum))
                    {
                        continue;
                    }

                    if (checksums.TryGetValue(dest, out var oldDestChecksum))
                    {
                        using var stream = File.OpenRead(dest);
                        var checksum = sha256.ComputeHash(stream);

                        if (!checksum.Equals(oldTemplateChecksum))
                            continue;
                    }
                }

                // update template checksum
                checksums[template] = templateChecksum;
                using var sr = new StreamReader(templateStream);
                var compiled = this.handlebars.Compile(sr);
                using var ms = new MemoryStream();
                using var sw = new StreamWriter(ms);
                compiled(sw, context);
                sw.Flush();
                ms.Flush();

                ms.Position = 0;
                var checksum2 = sha256.ComputeHash(ms);
                checksums[dest] = checksum2;
                ms.Position = 0;
                if (!Fs.DirectoryExists(dir))
                    Fs.MakeDirectory(dir);

                using var fs = File.OpenWrite(dest);
                ms.CopyTo(fs);
                fs.Flush();
            }
        }
    }

    private void ProcessSection(IConfigurationSection section, Dictionary<string, object?> context)
    {
        foreach (var child in section.GetChildren())
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
                this.ProcessSection(child, childContext);
            }
        }
    }

    private sealed class FileIgnore
    {
        private readonly List<Glob> ignorePatterns;

        public FileIgnore(string ignoreFile, string stackDir)
        {
            this.ignorePatterns = new List<Glob>();
            if (File.Exists(ignoreFile))
            {
                foreach (var line in Fs.ReadAllLines(ignoreFile))
                {
                    if (line.StartsWith("#"))
                        continue;

                    if (line.IsNullOrWhiteSpace())
                        continue;

                    var g = Glob.Parse($"{stackDir}{FsPath.DirectorySeparator}{line}");
                    this.ignorePatterns.Add(g);
                }
            }
        }

        public bool IsMatch(string path)
        {
            if (this.ignorePatterns.Count == 0)
                return false;

            foreach (var pattern in this.ignorePatterns)
            {
                if (pattern.IsMatch(path))
                    return true;
            }

            return false;
        }
    }

    private sealed class ChecksumEntry
    {
        public string Path { get; set; } = string.Empty;

        public string Checksum { get; set; } = string.Empty;
    }
}