using System.Text.Json;

using Bearz.Diagnostics;
using Bearz.Std;

using Xunit.Abstractions;

namespace Test.Std;

// ReSharper disable once InconsistentNaming
public class Command_Tests
{
    [IntegrationTest]
    public void Output(IAssert assert)
    {
        // generate tests for Command
        var cmd = new Command(Util.TestConsolePath, new CommandStartInfo()
        {
            Args = "default",
        });

        var result = cmd.Output();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.Empty(result.StdErr);
        assert.Empty(result.StdOut);

        cmd = new Command(Util.TestConsolePath)
                .WithArgs("streams")
                .WithStdio(Stdio.Piped);

        result = cmd.Output();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.NotEmpty(result.StdErr);
        assert.NotEmpty(result.StdOut);

        cmd = new Command(Util.TestConsolePath)
            .WithArgs("error")
            .WithStdio(Stdio.Piped);

        result = cmd.Output();
        assert.NotNull(result);
        assert.NotEqual(0, result.ExitCode);
        assert.NotEmpty(result.StdErr);
        assert.Empty(result.StdOut);
    }

    [IntegrationTest]
    public async Task OutputAsync(IAssert assert)
    {
        // generate tests for Command
        var cmd = new Command(Util.TestConsolePath, new CommandStartInfo()
        {
            Args = "default",
        });

        var result = await cmd.OutputAsync();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.Empty(result.StdErr);
        assert.Empty(result.StdOut);

        cmd = new Command(Util.TestConsolePath)
            .WithArgs("streams")
            .WithStdio(Stdio.Piped);

        result = await cmd.OutputAsync();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.NotEmpty(result.StdErr);
        assert.NotEmpty(result.StdOut);

        cmd = new Command(Util.TestConsolePath)
            .WithArgs("error")
            .WithStdio(Stdio.Piped);

        result = await cmd.OutputAsync();
        assert.NotNull(result);
        assert.NotEqual(0, result.ExitCode);
        assert.NotEmpty(result.StdErr);
        assert.Empty(result.StdOut);
    }

    [IntegrationTest]
    public void Invoke_TestConsole(IAssert assert)
    {
        var cmd = new Command(Util.TestConsolePath)
            .WithArgs("default")
            .WithStdio(Stdio.Piped);

        var result = cmd.Output();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.Empty(result.StdErr);
        assert.NotEmpty(result.StdOut);
        assert.Equal(result.StdOut[0], "Hello, World!");
    }

    [IntegrationTest]
    public void Spawn(IAssert assert)
    {
        var list = new List<string>();
        var cmd = new Command(Util.TestConsolePath)
            .WithArgs("default")
            .RedirectTo(list);

        var process = cmd.Spawn();
        process.WaitForExit();
        assert.Equal(0, process.ExitCode);
        assert.NotEmpty(list);
    }

    [IntegrationTest]
    public void Cwd(IAssert assert)
    {
        var cmd = new Command(Util.TestConsolePath)
            .WithArgs("cwd")
            .WithStdio(Stdio.Piped)
            .WithCwd(Bearz.Std.Env.GetDirectory(SpecialDirectory.Home));

        var result = cmd.Output();
        assert.NotNull(result);
        var cwd = result.StdOut[0];
        assert.Equal(Bearz.Std.Env.GetDirectory(SpecialDirectory.Home), cwd);
    }

    [IntegrationTest]
    public void Env(IAssert assert, ITestOutputHelper output)
    {
        var cmd = new Command(Util.TestConsolePath)
            .WithArgs("dump-env")
            .WithEnv(new Dictionary<string, string?>()
            {
                ["TEST"] = "Hello, World!",
                ["HOME"] = null,
            })
            .WithStdio(Stdio.Piped);

        var result = cmd.Output();
        assert.NotNull(result);
        var json = string.Join("\n", result.StdOut);
        var env = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        output.WriteLine(json);
        assert.NotNull(env);
        assert.False(env.ContainsKey("HOME"));
        assert.Equal("Hello, World!", env["TEST"]);
        output.WriteLine(string.Join("\n", result.StdOut));
    }

    [IntegrationTest]
    public void RedirectWithInput(IAssert assert, ITestOutputHelper output)
    {
        var count = 0;
        var fail = true;

        // pipe everything to get the output and also redirect stdout with an action.
        var cmd = new Command(Util.TestConsolePath)
            .WithArgs("echo")
            .WithStdio(Stdio.Piped)
            .RedirectTo((line, p) =>
            {
                output.WriteLine(line);
                if (line == "Enter a value: ")
                {
                    if (count >= 2)
                    {
                        fail = false;
                        p.StandardInput.WriteLine("exit");
                        return;
                    }

                    count++;
                    p.StandardInput.WriteLine("Hello, World!");
                }
            });

        var result = cmd.Output();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.False(fail);
        assert.NotEmpty(result.StdOut);
        assert.Equal("exiting...", result.StdOut[result.StdOut.Count - 1]);
        output.WriteLine(string.Empty);

        // only test stdin piping if the platform supports it
        var count2 = 0;
        fail = true;
        cmd = new Command(Util.TestConsolePath)
            .WithArgs("echo")
            .WithStdIn(Stdio.Piped)
            .RedirectTo((line, p) =>
            {
                output.WriteLine(line);
                if (line == "Enter a value: ")
                {
                    if (count2 >= 2)
                    {
                        fail = false;
                        p.StandardInput.WriteLine("exit");
                        return;
                    }

                    count2++;
                    p.StandardInput.WriteLine("Hello, World!");
                }
            });

        result = cmd.Output();
        assert.NotNull(result);
        assert.Equal(0, result.ExitCode);
        assert.False(fail);
        assert.Empty(result.StdOut);
    }
}