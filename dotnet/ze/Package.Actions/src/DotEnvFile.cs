using System.Text;

using Bearz.Extra.Strings;
using Bearz.Std;
using Bearz.Text;
using Bearz.Text.DotEnv;
using Bearz.Text.DotEnv.Document;

namespace Ze.Package.Actions;

public static class DotEnvFile
{
    public static (EnvDocument, bool) ReadFile(string envFile)
    {
        var envDoc = new EnvDocument();
        var sb = StringBuilderCache.Acquire();
        var encrypted = false;
        if (Fs.FileExists(envFile))
        {
            var contents = Fs.ReadTextFile(envFile);
            if (!contents.IsNullOrEmpty())
            {
                if (contents.Contains("sops_version="))
                {
                    encrypted = true;
                    if (!SopsUtil.IsSopsAvailable())
                    {
                        throw new InvalidOperationException(
                            $"sops is not installed and env file is encrypted {envFile}.");
                    }

                    var result1 = Env.Process.CreateCommand("sops")
                        .WithArgs("--decrypt", envFile)
                        .WithStdio(Stdio.Piped)
                        .Output();

                    foreach (var line in result1.StdOut)
                    {
                        sb.AppendLine(line);
                    }

                    sb.AppendLine();
                    contents = sb.ToString();
                    sb.Clear();
                }

                Console.WriteLine(contents);
                envDoc = DotEnvLoader.Parse(new DotEnvLoadOptions() { Content = contents, Expand = true, });
            }
        }

        return (envDoc, encrypted);
    }

    public static EnvDocument ReadFiles(IReadOnlyCollection<string> envFiles)
    {
        var envDoc = new EnvDocument();
        var sb = StringBuilderCache.Acquire();
        foreach (var envFile in envFiles)
        {
            if (Fs.FileExists(envFile))
            {
                var contents = Fs.ReadTextFile(envFile);
                if (!contents.IsNullOrEmpty())
                {
                    if (contents.Contains("sops_version="))
                    {
                        if (!SopsUtil.IsSopsAvailable())
                        {
                            throw new InvalidOperationException(
                                $"sops is not installed and env file is encrypted {envFile}.");
                        }

                        var result1 = Env.Process.CreateCommand("sops")
                            .WithArgs("--decrypt", envFile)
                            .WithStdio(Stdio.Piped)
                            .Output();

                        foreach (var line in result1.StdOut)
                        {
                            sb.AppendLine(line);
                        }

                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(contents);
                        sb.AppendLine();
                    }
                }
            }
        }

        if (sb.Length == 0)
        {
            StringBuilderCache.Release(sb);
            return envDoc;
        }

        var envContents = StringBuilderCache.GetStringAndRelease(sb);
        envDoc = DotEnvLoader.Parse(new DotEnvLoadOptions() { Content = envContents, Expand = true, });

        return envDoc;
    }

    public static void LoadRelevantVariables(string envFile, Dictionary<string, SecretSpec> secretsSpec)
    {
        var (envDoc, _) = ReadFile(envFile);
        foreach (var kvp in envDoc.ToDictionary())
        {
            if (secretsSpec.TryGetValue(kvp.Key, out _))
                Env.Set(kvp.Key, kvp.Value);
        }
    }

    public static void LoadRelevantVariables(IReadOnlyCollection<string> envFiles, Dictionary<string, SecretSpec> secretsSpec)
    {
        var envDoc = ReadFiles(envFiles);
        foreach (var kvp in envDoc.ToDictionary())
        {
            if (secretsSpec.TryGetValue(kvp.Key, out _))
                Env.Set(kvp.Key, kvp.Value);
        }
    }

    public static void LoadFiles(IReadOnlyCollection<string> envFiles)
    {
        var sb = StringBuilderCache.Acquire();
        foreach (var envFile in envFiles)
        {
            if (Fs.FileExists(envFile))
            {
                var contents = Fs.ReadTextFile(envFile);
                if (!contents.IsNullOrEmpty())
                {
                    if (contents.Contains("sops_version="))
                    {
                        if (!SopsUtil.IsSopsAvailable())
                        {
                            throw new InvalidOperationException(
                                $"sops is not installed and env file is encrypted {envFile}.");
                        }

                        var result1 = Env.Process.CreateCommand("sops")
                            .WithArgs("--decrypt", envFile)
                            .WithStdio(Stdio.Piped)
                            .Output();

                        foreach (var line in result1.StdOut)
                        {
                            sb.AppendLine(line);
                        }

                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine(contents);
                        sb.AppendLine();
                    }
                }
            }
        }

        if (sb.Length == 0)
        {
            StringBuilderCache.Release(sb);
            return;
        }

        var envContents = StringBuilderCache.GetStringAndRelease(sb);
        DotEnvLoader.Load(new DotEnvLoadOptions() { Content = envContents, Expand = true, });
    }

    public static void WriteFile(string envFile, EnvDocument envDoc, bool encrypted = false)
    {
        var sb = StringBuilderCache.Acquire();
        foreach (var element in envDoc)
        {
            if (element is EnvEmptyLine)
            {
                sb.AppendLine();
                continue;
            }

            if (element is EnvComment comment)
            {
                sb.AppendLine(comment.Value);
                continue;
            }

            if (element is EnvNameValuePair entry)
            {
                sb.Append(entry.Name)
                    .Append('=')
                    .AppendWithQuotes(entry.Value, '\"')
                    .AppendLine();
            }
        }

        Fs.EnsureDirectoryForFile(envFile);
        var contents = StringBuilderCache.GetStringAndRelease(sb);
        if (encrypted)
        {
            Fs.WriteTextFile(envFile, contents);
            var result = Env.Process.CreateCommand("sops")
                .WithArgs("--encrypt", "--in-place", envFile)
                .WithStdio(Stdio.Inherit)
                .Output();

            result.ThrowOnInvalidExitCode();
        }
        else
        {
            Fs.WriteTextFile(envFile, contents);
        }
    }
}