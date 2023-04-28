using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.Json;

using Bearz.Casa.App.Services;
using Bearz.Casa.Data.Services;
using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Templates.Handlebars;
using Bearz.Templating.Handlebars;

using HandlebarsDotNet;

using Microsoft.Extensions.Configuration;

namespace Bearz.Casa.App.Tasks;

public class EvaluateTemplateTask
{
    private readonly IHandlebars handlebars = Handlebars.Create();

    public EvaluateTemplateTask(EnvironmentSet set)
    {
        this.Environments = set;
    }

    public string TemplateDirectory { get; set; } = string.Empty;

    public string Environment { get; set; } = "default";

    public bool Overwrite { get; set; }

    public bool Import { get; set; }

    protected EnvironmentSet Environments { get; set; }

    public Task RunAsync(CancellationToken cancellationToken)
    {
        this.handlebars.RegisterEnvHelpers();
        this.handlebars.RegisterStringHelpers();
        this.handlebars.RegisterDateTimeHelpers();
        this.handlebars.RegisterJsonHelpers();

        var env = this.Environments.GetOrCreate(this.Environment);
        var templateDir = this.TemplateDirectory;
        var vault = new CasaSecretVault(env);
        this.handlebars.RegisterSecretHelpers(vault);
        var casaStackConfigFile = FsPath.Join(this.TemplateDirectory, "casa.json");

        var tempConfig = new ConfigurationBuilder()
            .AddJsonFile(casaStackConfigFile, true, false)
            .Build();

        var stackName = tempConfig.GetValue<string?>("compose:name");
        stackName ??= FsPath.Basename(this.TemplateDirectory);

        if (!this.Environment.EqualsIgnoreCase("default"))
            stackName = $"{stackName}-{this.Environment}";

        var stackDirectory = this.TemplateDirectory;
        if (this.Import)
            stackDirectory = FsPath.Join(Paths.ComposeDirectory, stackName);

        var lockFile = FsPath.Join(stackDirectory, "lock.json");
        if (File.Exists(lockFile))
            return Task.CompletedTask;

        var checksumFile = FsPath.Join(stackDirectory, "checksums.json");
        var checksums = new Dictionary<string, byte[]>();
        var checksumEntries = new List<ChecksumEntry>();
        if (Fs.FileExists(checksumFile))
        {
            var json2 = Fs.ReadTextFile(checksumFile);
            checksumEntries = JsonSerializer.Deserialize<List<ChecksumEntry>>(json2);
            if (checksumEntries is not null)
            {
                foreach (var entry in checksumEntries)
                {
                    checksums[entry.Path] = Convert.FromBase64String(entry.Checksum);
                }
            }
        }

        var configFiles = new[]
        {
            FsPath.Join(Paths.ConfigDirectory, "casa.json"),
            FsPath.Join(Paths.ConfigDirectory, $"casa.{this.Environment}.json"),
            FsPath.Join(Paths.UserConfigDirectory, "casa.json"),
            FsPath.Join(Paths.UserConfigDirectory, $"casa.{this.Environment}.json"),
            FsPath.Join(Paths.UserConfigDirectory, "casa.yaml"), FsPath.Join(Paths.UserConfigDirectory, "casa.yml"),
            FsPath.Join(Paths.UserConfigDirectory, $"casa.{this.Environment}.yaml"),
            FsPath.Join(Paths.UserConfigDirectory, $"casa.{this.Environment}.yml"),
            FsPath.Join(templateDir, "casa.json"), FsPath.Join(templateDir, $"casa.{this.Environment}.json"),
            FsPath.Join(templateDir, "casa.yaml"), FsPath.Join(templateDir, "casa.yml"),
            FsPath.Join(templateDir, $"casa.{this.Environment}.yaml"),
            FsPath.Join(templateDir, $"casa.{this.Environment}.yml"),
        };

        var builder = new ConfigurationBuilder();
        bool configChanged = false;

        using var sha256 = SHA256.Create();

        foreach (var configFile in configFiles)
        {
            if (File.Exists(configFile))
            {
                var ext = FsPath.GetExtension(configFile);
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
        var context = new Dictionary<string, object?>()
        {
            ["environment"] = this.Environment,
            ["templateDir"] = templateDir,
            ["composeDir"] = stackDirectory,
            ["localIp"] = GetLocalIp(),
            ["hostname"] = Env.Get("HOSTNAME") ?? Env.Get("COMPUTERNAME") ?? "unknown",
            ["isWindows"] = OperatingSystem.IsWindows(),
            ["isLinux"] = OperatingSystem.IsLinux(),
            ["isDarwin"] = OperatingSystem.IsMacOS(),
            ["isMacOS"] = OperatingSystem.IsMacOS(),
            ["isFreeBsd"] = OperatingSystem.IsFreeBSD(),
        };

        this.ProcessSection(config.GetSection("compose:variables"), context);
        var ignoreFiles = Directory.EnumerateFiles(templateDir, ".casaignore", SearchOption.AllDirectories);
        var ignoreFile = new CasaIgnore(ignoreFiles);

        var directories = Directory.EnumerateDirectories(templateDir);
        var sameRoot = templateDir.Equals(stackDirectory);
        Console.WriteLine(templateDir);
        Console.WriteLine(stackDirectory);
        Console.WriteLine();
        Console.WriteLine($"same root: {sameRoot}");
        foreach (var dir in directories)
        {
            if (dir.Equals(this.TemplateDirectory))
                continue;

            if (ignoreFile.IsMatch(dir))
                continue;

            var destDir = dir;
            if (!sameRoot)
            {
                var folderName = FsPath.Basename(dir);
                destDir = Path.Join(stackDirectory, folderName);
            }

            Console.WriteLine(dir);
            Console.WriteLine(destDir);
            Console.WriteLine();
            this.RenderDirectory(dir, destDir, context, checksums, configChanged, this.Overwrite, ignoreFile);
        }

        Console.WriteLine(templateDir);
        Console.WriteLine(stackDirectory);
        Console.WriteLine();
        this.RenderRootFiles(templateDir, stackDirectory, context, checksums, configChanged, this.Overwrite);

        checksumEntries ??= new List<ChecksumEntry>();
        checksumEntries.Clear();
        foreach (var kvp in checksums)
        {
            checksumEntries.Add(new ChecksumEntry() { Path = kvp.Key, Checksum = Convert.ToBase64String(kvp.Value), });
        }

        var json = JsonSerializer.Serialize(
            checksumEntries,
            new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase, });
        File.WriteAllText(checksumFile, json);

        return Task.CompletedTask;
    }

    private static string? GetLocalIp()
    {
        string? localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint?.Address.ToString();
        }

        return localIP;
    }

    private void RenderRootFiles(
        string sourceDir,
        string destDir,
        Dictionary<string, object?> context,
        Dictionary<string, byte[]> checksums,
        bool configChanged,
        bool overwrite)
    {
        var composeFiles = new List<string>()
        {
            Path.Combine(sourceDir, $"{this.Environment}.compose.yml.hbs"),
            Path.Combine(sourceDir, $"{this.Environment}.compose.yaml.hbs"),
            Path.Combine(sourceDir, "compose.yml.hbs"),
            Path.Combine(sourceDir, "compose.yaml.hbs"),
            Path.Combine(sourceDir, $"{this.Environment}.docker-compose.yml.hbs"),
            Path.Combine(sourceDir, $"{this.Environment}.docker-compose.yaml.hbs"),
            Path.Combine(sourceDir, "docker-compose.yml.hbs"),
            Path.Combine(sourceDir, "docker-compose.yaml.hbs"),
        };

        var composeTemplate = composeFiles.FirstOrDefault(File.Exists);
        var envFiles = new List<string>()
        {
            Path.Combine(sourceDir, $"{this.Environment}.env.hbs"),
            Path.Combine(sourceDir, ".env.hbs"),
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

        foreach (var tpl in templates)
            Console.WriteLine(tpl);

        if (templates.Count == 0)
            return;

        this.ProcessTemplates(
            templates,
            sourceDir,
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
        CasaIgnore fileIgnore)
    {
        using var sha256 = SHA256.Create();
        var templates = new List<string>();
        var files = Fs.ReadDirectory(sourceDir, "*", SearchOption.AllDirectories);

        var sameDir = sourceDir.Equals(destDir);
        foreach (var file in files)
        {
            if (Fs.IsDirectory(file))
                continue;

            if (FsPath.GetExtension(file) == ".hbs")
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
            var sameDir = sourceDir.Equals(destDir);
            Console.WriteLine();
            Console.WriteLine(template);
            Console.WriteLine(sourceDir);
            Console.WriteLine(destDir);
            string dest;
            if (!sameDir)
            {
                var relative = FsPath.RelativePath(sourceDir, template);

                // get rid of the .hbs extension
                relative = relative.Substring(0, relative.Length - 4);
                dest = FsPath.Resolve(FsPath.Join(destDir, relative));
            }
            else
            {
                dest = template.Substring(0, template.Length - 4);
            }

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

    private sealed class ChecksumEntry
    {
        public string Path { get; set; } = string.Empty;

        public string Checksum { get; set; } = string.Empty;
    }
}