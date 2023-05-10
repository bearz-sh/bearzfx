using Bearz.Std;

using Path = System.IO.Path;

namespace Test;

public static class Util
{
    private static string? s_location;

    private static string? s_testConsolePath;

    public static string Location
    {
        get
        {
            if (s_location is not null)
                return s_location;

            var assembly = typeof(Util).Assembly;
            var codeBase = assembly.Location;
            if (codeBase.StartsWith("file:///"))
                codeBase = codeBase.Substring(8);

            s_location = Path.GetDirectoryName(codeBase);
            return s_location!;
        }
    }

    public static string TestConsolePath
    {
        get
        {
            if (s_testConsolePath is not null)
                return s_testConsolePath;

            var tmp = Path.GetTempPath();
            if (Env.IsWindows)
            {
                s_testConsolePath = Path.Combine(tmp, "bearz-test-console", "test-console.exe");
            }
            else
            {
                s_testConsolePath = Path.Combine(tmp, "bearz-test-console", "test-console");
            }

            return s_testConsolePath;
        }
    }
}