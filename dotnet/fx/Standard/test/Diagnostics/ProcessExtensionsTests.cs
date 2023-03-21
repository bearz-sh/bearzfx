using System.Diagnostics;
using System.Text;

using Bearz.Diagnostics;

namespace Test.Diagnostics;

public static class ProcessExtensionsTests
{
    [IntegrationTest]
    public static void Run(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";
        var r = process.Run();

        assert.Equal(0, r.ExitCode);
    }

    [IntegrationTest]
    public static async Task RunAsync(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";
        var r = await process.RunAsync().ConfigureAwait(false);
        assert.Equal(0, r.ExitCode);
    }

    [IntegrationTest]
    public static void RedirectToTextWriter(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        process.RedirectTo(sw);
        process.RedirectErrorTo(sw);
        var r = process.Run();
        assert.Equal(0, r.ExitCode);
        assert.True(sb.Length > 0);
        assert.True(sb.ToString().Contains("."));
    }

    [IntegrationTest]
    public static void RedirectToStream(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";

        using var ms = new MemoryStream();
        process.RedirectTo(ms);

        var r = process.Run();

        ms.Flush();
        var bytes = ms.ToArray();
        var s = Encoding.UTF8.GetString(bytes);
        assert.Equal(0, r.ExitCode);
        assert.True(s.Length > 0);
        assert.True(s.Contains("."));
    }

    [IntegrationTest]
    public static void RedirectToCollection(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";

        var list = new List<string>();
        process.RedirectTo(list);

        var r = process.Run();
        assert.Equal(0, r.ExitCode);
        assert.True(list.Count == 1);
        assert.True(list[0].Contains("."));
    }

    [IntegrationTest]
    public static void RedirectToAction(IAssert assert)
    {
        using var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = "--version";

        var list = new List<string>();
        process.RedirectTo((line, _) => list.Add(line));

        var r = process.Run();
        assert.Equal(0, r.ExitCode);
        assert.True(list.Count == 1);
        assert.True(list[0].Contains("."));
    }
}