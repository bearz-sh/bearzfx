using Bearz.Extra.Strings;
using Bearz.Handlebars.Helpers;
using Bearz.Secrets;
using Bearz.Std;
using Bearz.Text.DotEnv.Document;
using Bearz.Virtual;

using HandlebarsDotNet;

namespace Ze.Package.Actions;

public class InstallAction
{
    public void Run(ZeExtractedPackage package, bool force = false)
    {
        var paths = package.Paths;
        var globalEnvFile = package.GlobalEnvFile;
        var targetEnvFile = package.EnvFile;
        if (package.Secrets.Count > 0)
        {
            var secretsSpec = package.Secrets;
            var (envDoc, encrypted) = DotEnvFile.ReadFile(targetEnvFile);
            foreach (var item in envDoc.ToDictionary())
            {
                Console.WriteLine(item.Key);
            }

            bool changed = false;
            foreach (var kvp in secretsSpec)
            {
                var next = kvp.Value;
                if (envDoc.TryGetValue(kvp.Key, out _))
                    continue;

                if (!changed)
                {
                    envDoc.Add(new EnvEmptyLine());
                    var c = new EnvComment($"# {package.Spec.Name} secrets".AsSpan());
                    envDoc.Add(c);
                }

                next ??= new SecretSpec();

                if (next is { Generate: true, Length: > 0 })
                {
                    var sg = new SecretGenerator();
                    if (next.Digit)
                        sg.Add(SecretCharacterSets.Digits);
                    if (next.Lower)
                        sg.Add(SecretCharacterSets.LatinAlphaLowerCase);
                    if (next.Upper)
                        sg.Add(SecretCharacterSets.LatinAlphaUpperCase);

                    if (next.Special)
                    {
                        sg.Add(next.SpecialCharacters.IsNullOrWhiteSpace()
                            ? SecretCharacterSets.SpecialSafe
                            : next.SpecialCharacters);
                    }

                    if (!next.Digit || !next.Special || !next.Upper || !next.Lower)
                        sg.SetValidator(_ => true);

                    var value = sg.GenerateAsString(next.Length);
                    envDoc.Add(kvp.Key, value);
                    changed = true;
                }
                else
                {
                    changed = true;
                    envDoc.Add(kvp.Key, string.Empty);
                }
            }

            if (changed)
            {
                DotEnvFile.WriteFile(targetEnvFile, envDoc, encrypted);
            }
        }

        // must load before calling RegisterEnvHelpers
        DotEnvFile.LoadFiles(new[] { globalEnvFile, targetEnvFile });

        var hbs = Handlebars.Create();
        hbs.RegisterEnvHelpers();
        hbs.RegisterJsonHelpers();
        hbs.RegisterRegexHelpers();
        hbs.RegisterStringHelpers();
        hbs.RegisterDateTimeHelpers();

        var di = Fs.StatDirectory(package.TemplatesDirectory);
        var dest = FsPath.Combine(paths.ComposeDir, package.Spec.Name);
        if (!Fs.DirectoryExists(dest))
            Fs.MakeDirectory(dest);

        var data = package.Variables.ToDictionary();
        foreach (var item in di.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
        {
            Console.WriteLine();
            Console.WriteLine(dest);
            if (item is IFileInfo fi)
            {
                Console.WriteLine(fi.FullName);
                if (fi.Extension.Equals(".hbs"))
                {
                    Console.WriteLine(dest);
                    var templateDest = FsPath.Combine(dest, fi.Name.Substring(0, fi.Name.Length - 4));
                    Console.WriteLine(templateDest);
                    if (Fs.FileExists(templateDest) && !force)
                        continue;

                    Console.WriteLine("write file " + fi.FullName);
                    var template = hbs.Compile(Fs.ReadTextFile(fi.FullName));
                    var output = template(data);
                    Fs.WriteTextFile(templateDest, output);
                    continue;
                }

                var fileDest = FsPath.Combine(dest, fi.Name);
                Console.WriteLine(fileDest);
                if (Fs.FileExists(fileDest) && !force)
                    continue;

                Fs.CopyFile(fi.FullName, fileDest, force);
            }

            if (item is IDirectoryInfo dir)
            {
                Console.WriteLine();
                Console.WriteLine(dir.FullName);
                if (dir.Name.Equals("etc", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleSpecialDirectory(
                        dir,
                        Path.Combine(paths.EtcDir, package.Spec.Name),
                        hbs,
                        data,
                        force);
                }
                else if (dir.Name.Equals("data", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleSpecialDirectory(
                        dir,
                        Path.Combine(paths.DataDir, package.Spec.Name),
                        hbs,
                        data,
                        force);
                }
                else if (dir.Name.Equals("run", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleSpecialDirectory(
                        dir,
                        Path.Combine(paths.RunDir, package.Spec.Name),
                        hbs,
                        data,
                        force);
                }
                else
                {
                    this.HandleSpecialDirectory(dir, FsPath.Combine(dest, dir.Name), hbs, data, force, true);
                }
            }
        }
    }

    private void HandleSpecialDirectory(
        IDirectoryInfo dir,
        string dest,
        IHandlebars hbs,
        IDictionary<string, object?> data,
        bool force = false,
        bool compose = false)
    {
        Console.WriteLine(dest);
        if (!Fs.DirectoryExists(dest))
            Fs.MakeDirectory(dest);

        foreach (var item in dir.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
        {
            if (compose && item.Name is "etc" or "bin" or "data" or "run" or "log")
                continue;

            Console.WriteLine(item.FullName);

            if (item is IDirectoryInfo childDir)
            {
                this.HandleSpecialDirectory(childDir, Path.Combine(dest, childDir.Name), hbs, data, force, compose);
                continue;
            }

            if (item is IFileInfo fi)
            {
                if (fi.Extension.Equals(".hbs"))
                {
                    var templateDest = FsPath.Combine(dest, fi.Name.Substring(0, fi.Name.Length - 4));
                    if (Fs.FileExists(templateDest) && !force)
                        continue;

                    var template = hbs.Compile(Fs.ReadTextFile(fi.FullName));
                    var output = template(data);
                    Fs.WriteTextFile(templateDest, output);
                    continue;
                }

                Console.WriteLine("dest " + dest);
                var fileDest = FsPath.Combine(dest, fi.Name);
                if (Fs.FileExists(fileDest) && !force)
                    continue;

                Fs.CopyFile(fi.FullName, fileDest, force);
            }
        }
    }
}