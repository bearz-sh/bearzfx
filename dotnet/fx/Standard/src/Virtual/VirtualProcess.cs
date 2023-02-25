using System.Collections.Concurrent;
using System.Diagnostics;

using Bearz.Extra.Strings;
using Bearz.Std;

using Path = Bearz.Std.Path;
using Proc = System.Diagnostics.Process;
using Process = Bearz.Std.Process;

// ReSharper disable ParameterHidesMember
namespace Bearz.Virtual;

public class VirtualProcess : IProcess
{
    private static readonly ConcurrentDictionary<string, string> ExecutableLocationCache = new();

    private static Proc? s_process;

    private readonly IEnvironment env;

    public VirtualProcess(IEnvironment env)
    {
        this.env = env;
    }

    public string CommandLine => System.Environment.CommandLine;

    public IReadOnlyList<string> Argv => System.Environment.GetCommandLineArgs();

    public int ExitCode { get; set; }

    public int Id => Current.Id;

    public TextWriter Out => Console.Out;

    public TextWriter Error => Console.Error;

    private static Proc Current => s_process ??= Proc.GetCurrentProcess();

    public Command CreateCommand(string fileName, CommandStartInfo? startInfo = null)
    {
        startInfo ??= new CommandStartInfo() { StdOut = Stdio.Inherit, StdErr = Stdio.Inherit, };
        startInfo.Cwd ??= this.env.Cwd;
        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in this.env.List())
        {
            data[kvp.Key] = kvp.Value;
        }

        if (startInfo.Env != null)
        {
            foreach (var kvp in startInfo.Env)
            {
                data[kvp.Key] = kvp.Value;
            }
        }

        startInfo.Env = data;

        return new Command(fileName, startInfo);
    }

    public void Kill(int pid)
    {
        Process.Exit(pid);
    }

    public void Exit(int code)
    {
        System.Environment.Exit(code);
    }

    public CommandOutput Run(
        string fileName,
        CommandArgs? args,
        string? cwd = null,
        IEnumerable<KeyValuePair<string, string>>? env = null,
        Stdio stdout = Stdio.Inherit,
        Stdio stderr = Stdio.Inherit)
    {
        var si = new CommandStartInfo()
        {
            Cwd = cwd ?? this.env.Cwd,
            Env = env?.ToDictionary(o => o.Key, o => o.Value),
            StdOut = stdout,
            StdErr = stderr,
        };

        if (args is not null)
            si.Args = args;

        var cmd = new Command(fileName, si);
        return cmd.Output();
    }

    public Task<CommandOutput> RunAsync(
        string fileName,
        CommandArgs? args,
        string? cwd = null,
        IEnumerable<KeyValuePair<string, string>>? env = null,
        Stdio stdout = Stdio.Inherit,
        Stdio stderr = Stdio.Inherit,
        CancellationToken cancellationToken = default)
    {
        var si = new CommandStartInfo()
        {
            Cwd = cwd ?? this.env.Cwd,
            Env = env?.ToDictionary(o => o.Key, o => o.Value),
            StdOut = stdout,
            StdErr = stderr,
        };

        if (args is not null)
            si.Args = args;

        var cmd = new Command(fileName, si);
        return cmd.OutputAsync(cancellationToken);
    }

    public string? Which(string command, IEnumerable<string>? prependPaths = null, bool useCache = true)
    {
        // https://github.com/actions/runner/blob/592ce1b230985aea359acdf6ed4ee84307bbedc1/src/Runner.Sdk/Util/WhichUtil.cs
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentNullException(nameof(command));

        var rootName = Path.BasenameWithoutExtension(command);
        if (useCache && ExecutableLocationCache.TryGetValue(rootName, out var location))
            return location;

#if NETLEGACY
        if (Path.IsPathRooted(command) && File.Exists(command))
        {
            ExecutableLocationCache[command] = command;
            ExecutableLocationCache[rootName] = command;

            return command;
        }
#else
        if (Path.IsPathFullyQualified(command) && File.Exists(command))
        {
            ExecutableLocationCache[command] = command;
            ExecutableLocationCache[rootName] = command;

            return command;
        }
#endif

        var pathSegments = new List<string>();
        if (prependPaths is not null)
            pathSegments.AddRange(prependPaths);

        pathSegments.AddRange(EnvPath.Split());

        for (var i = 0; i < pathSegments.Count; i++)
        {
            pathSegments[i] = this.env.Expand(pathSegments[i]);
        }

        foreach (var pathSegment in pathSegments)
        {
            if (string.IsNullOrEmpty(pathSegment) || !Directory.Exists(pathSegment))
                continue;

            IEnumerable<string> matches = Array.Empty<string>();
            if (Env.IsWindows())
            {
                var pathExt = this.env.Get("PATHEXT");
                if (pathExt.IsNullOrWhiteSpace())
                {
                    // XP's system default value for PATHEXT system variable
                    pathExt = ".com;.exe;.bat;.cmd;.vbs;.vbe;.js;.jse;.wsf;.wsh";
                }

                var pathExtSegments = pathExt.ToLowerInvariant()
                                             .Split(
                                                 new[] { ";", },
                                                 StringSplitOptions.RemoveEmptyEntries);

                // if command already has an extension.
                if (pathExtSegments.Any(command.EndsWithIgnoreCase))
                {
                    try
                    {
                        matches = Directory.EnumerateFiles(pathSegment, command);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    var result = matches.FirstOrDefault();
                    if (result is null)
                        continue;

                    ExecutableLocationCache[rootName] = result;
                    return result;
                }
                else
                {
                    string searchPattern = $"{command}.*";
                    try
                    {
                        matches = Directory.EnumerateFiles(pathSegment, searchPattern);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    var expandedPath = Path.Combine(pathSegment, command);

                    foreach (var match in matches)
                    {
                        foreach (var ext in pathExtSegments)
                        {
                            var fullPath = expandedPath + ext;
                            if (!match.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            ExecutableLocationCache[rootName] = fullPath;
                            return fullPath;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    matches = Directory.EnumerateFiles(pathSegment, command);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                var result = matches.FirstOrDefault();
                if (result is null)
                    continue;

                ExecutableLocationCache[rootName] = result;
                return result;
            }
        }

        return null;
    }
}