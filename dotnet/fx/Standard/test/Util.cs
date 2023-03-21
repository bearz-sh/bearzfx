using Bearz.Std;

using Path = System.IO.Path;

namespace Test;

public static class Util
{
    private static string? s_location;

    private static string? s_fxRoot;

    private static string? s_standardDir;

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
#if DEBUG
            if (Env.IsWindows())
            {
                s_testConsolePath = Path.Combine(StandardDir, "console", "bin", "Debug", "net7.0", "test-console.exe");
            }
            else
            {
                s_testConsolePath = Path.Combine(StandardDir, "console", "bin", "Debug", "net7.0", "test-console");
            }
#else
            if (Env.IsWindows())
            {
                s_testConsolePath =
                    Path.Combine(StandardDir, "console", "bin", "Release", "net7.0", "test-console.exe");
            }
            else
            {
                s_testConsolePath =
                    Path.Combine(StandardDir, "console", "bin", "Release", "net7.0", "test-console");
            }
#endif
            return s_testConsolePath;
        }
    }

    public static string StandardDir
    {
        get
        {
            if (s_fxRoot is not null)
                return s_fxRoot;

            var location = Location;
            while (location is not null)
            {
                if (Path.GetFileName(location) == "Standard")
                {
                    s_standardDir = location;
                    return s_standardDir;
                }

                location = Path.GetDirectoryName(location);
            }

            throw new Exception("Could not find fx root");
        }
    }

    public static string FxRootDir
    {
        get
        {
            if (s_fxRoot is not null)
                return s_fxRoot;

            var location = Location;
            while (location is not null)
            {
                if (Path.GetFileName(location) == "fx")
                {
                    s_fxRoot = location;
                    return s_fxRoot;
                }

                location = Path.GetDirectoryName(location);
            }

            throw new Exception("Could not find fx root");
        }
    }
}