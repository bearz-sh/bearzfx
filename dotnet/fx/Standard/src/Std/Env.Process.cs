using System.Collections.Concurrent;
using System.Diagnostics;

using Bearz.Extra.Strings;

using Proc = System.Diagnostics.Process;

namespace Bearz.Std;

public static partial class Env
{
    public static class Process
    {
        private static readonly ConcurrentDictionary<string, string> ExecutableLocationCache = new();

        private static Proc? s_process;

        public static int Id => Current.Id;

        public static IReadOnlyList<string> Argv => Environment.GetCommandLineArgs();

        public static string CommandLine => Environment.CommandLine;

        public static Proc Current
        {
            get
            {
                return s_process ??= Proc.GetCurrentProcess();
            }
        }

        public static int ExitCode
        {
            get => Environment.ExitCode;
            set => Environment.ExitCode = value;
        }

        public static CommandOutput Run(
            string fileName,
            CommandArgs? args,
            string? cwd = null,
            IEnumerable<KeyValuePair<string, string?>>? env = null,
            Stdio stdout = Stdio.Inherit,
            Stdio stderr = Stdio.Inherit)
        {
            var si = new CommandStartInfo()
            {
                Cwd = cwd,
                Env = env?.ToDictionary(o => o.Key, o => o.Value),
                StdOut = stdout,
                StdErr = stderr,
            };

            if (args != null)
                si.Args = args;

            var cmd = new Command(fileName, si);

            return cmd.Output();
        }

        public static Task<CommandOutput> RunAsync(
            string fileName,
            CommandArgs? args,
            string? cwd = null,
            IEnumerable<KeyValuePair<string, string?>>? env = null,
            Stdio stdout = Stdio.Inherit,
            Stdio stderr = Stdio.Inherit,
            CancellationToken cancellationToken = default)
        {
            var si = new CommandStartInfo()
            {
                Cwd = cwd,
                Env = env?.ToDictionary(o => o.Key, o => o.Value),
                StdOut = stdout,
                StdErr = stderr,
            };

            if (args != null)
                si.Args = args;

            var cmd = new Command(fileName, si);

            return cmd.OutputAsync(cancellationToken);
        }

        public static Command CreateCommand(string fileName, CommandStartInfo? startInfo = null)
        {
            return new(fileName, startInfo);
        }

        public static Proc CreateProcess()
        {
            return new();
        }

        public static Proc CreateProcess(ProcessStartInfo startInfo)
        {
            return new()
            {
                StartInfo = startInfo,
            };
        }

        public static void Kill(int pid)
        {
            Proc.GetProcessById(pid).Kill();
        }

        public static void Exit(int code)
        {
            Environment.Exit(code);
        }

        /// <summary>
        /// Provides the equivalent functionality of which/where on windows and linux to
        /// determine the full path of an executable found on the PATH.
        /// </summary>
        /// <param name="command">The command to search for.</param>
        /// <param name="prependPaths">Additional paths that will be used to find the executable.</param>
        /// <param name="useCache">Should the lookup use the cached values.</param>
        /// <returns>Null or the full path of the command.</returns>
        /// <exception cref="ArgumentNullException">Throws when command is null.</exception>
        public static string? Which(
            string command,
            IEnumerable<string>? prependPaths = null,
            bool useCache = true)
        {
            // https://github.com/actions/runner/blob/592ce1b230985aea359acdf6ed4ee84307bbedc1/src/Runner.Sdk/Util/WhichUtil.cs
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            var rootName = FsPath.BasenameWithoutExtension(command);
            if (useCache && ExecutableLocationCache.TryGetValue(rootName, out var location))
                return location;

    #if NETLEGACY
            if (FsPath.IsPathRooted(command) && File.Exists(command))
            {
                ExecutableLocationCache[command] = command;
                ExecutableLocationCache[rootName] = command;

                return command;
            }
    #else
            if (FsPath.IsPathFullyQualified(command) && File.Exists(command))
            {
                ExecutableLocationCache[command] = command;
                ExecutableLocationCache[rootName] = command;

                return command;
            }
    #endif

            var pathSegments = new List<string>();
            if (prependPaths is not null)
                pathSegments.AddRange(prependPaths);

            pathSegments.AddRange(SplitPath());

            for (var i = 0; i < pathSegments.Count; i++)
            {
                pathSegments[i] = Env.Expand(pathSegments[i]);
            }

            foreach (var pathSegment in pathSegments)
            {
                if (string.IsNullOrEmpty(pathSegment) || !System.IO.Directory.Exists(pathSegment))
                    continue;

                IEnumerable<string> matches = Array.Empty<string>();
                if (Env.IsWindows())
                {
                    var pathExt = Env.Get("PATHEXT");
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
                            matches = System.IO.Directory.EnumerateFiles(pathSegment, command);
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
                            matches = System.IO.Directory.EnumerateFiles(pathSegment, searchPattern);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        var expandedPath = FsPath.Combine(pathSegment, command);

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
                        matches = System.IO.Directory.EnumerateFiles(pathSegment, command);
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
}