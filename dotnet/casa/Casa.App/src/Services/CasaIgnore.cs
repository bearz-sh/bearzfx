using Bearz.Extra.Strings;
using Bearz.Std;

using DotNet.Globbing;

namespace Bearz.Casa.App.Services;

public class CasaIgnore
{
    private readonly List<Glob> ignorePatterns;

    public CasaIgnore(IEnumerable<string> ignoreFiles)
    {
        this.ignorePatterns = new List<Glob>();
        foreach (var ignoreFile in ignoreFiles)
        {
            if (!Fs.FileExists(ignoreFile))
                continue;

            var parentDir = FsPath.Dirname(ignoreFile);
            if (parentDir is null)
                continue;

            foreach (var line in Fs.ReadAllLines(ignoreFile))
            {
                if (line.StartsWith("#"))
                    continue;

                if (line.IsNullOrWhiteSpace())
                    continue;

                var g = Glob.Parse($"{parentDir}{FsPath.DirectorySeparator}{line}");
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