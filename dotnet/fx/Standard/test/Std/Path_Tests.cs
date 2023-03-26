using Bearz.Std;

using Xunit.Abstractions;

using Path = Bearz.Std.Path;

namespace Test.Std;

// ReSharper disable once InconsistentNaming
public class Path_Tests
{
    [UnitTest]
    public void Resolve_BasePathMustBeAbsolute(IAssert assert)
    {
        assert.Throws<InvalidOperationException>(() => Path.Resolve("foo", "bar"));

        if (Env.IsWindows())
        {
            assert.Throws<InvalidOperationException>(() => Path.Resolve("foo", "c:bar"));
            assert.Throws<InvalidOperationException>(() => Path.Resolve("foo", "/bar"));
        }
        else
        {
            assert.Throws<InvalidOperationException>(() => Path.Resolve("foo", "c:\\bar"));
        }
    }

    [UnitTest]
    public void Resolve(IAssert assert)
    {
        // test resolve relative path
        var cwd = Env.Cwd;
        var hd = Env.HomeDir();
        assert.Equal(hd, Path.Resolve("~"));

        assert.Equal(hd, Path.Resolve("~", cwd));
        var rel = Path.Resolve("foo", cwd);
        assert.Equal(Path.Combine(cwd, "foo"), rel);

        assert.Equal(cwd, Path.Resolve(".", cwd));
        assert.Equal(cwd, Path.Resolve("./", cwd));
        assert.Equal(cwd, Path.Resolve(".\\", cwd));

        if (Env.IsWindows())
        {
            assert.Equal($"{hd}\\Desktop", Path.Resolve("~/Desktop"));

            // test resolve and absolute paths
            var abs = Path.Resolve("c:\\foo", cwd);
            assert.Equal("c:\\foo", abs);

            // test resolve and relative paths
            var rel2 = Path.Resolve("foo", "c:\\bar");
            assert.Equal("c:\\bar\\foo", rel2);

            // test resolve and absolute paths
            var abs2 = Path.Resolve("/foo", "c:\\bar");
            assert.Equal("c:\\foo", abs2);
        }
        else
        {
            assert.Equal($"{hd}/Desktop", Path.Resolve("~/Desktop"));

            // test resolve and absolute paths
            var abs = Path.Resolve("/foo", cwd);
            assert.Equal("/foo", abs);

            // test resolve and relative paths
            var rel2 = Path.Resolve("foo", "/bar");
            assert.Equal("/bar/foo", rel2);

            // test resolve and absolute paths
            var abs2 = Path.Resolve("/foo", "/bar");
            assert.Equal("/foo", abs2);

            // test resolve and absolute paths
            var abs3 = Path.Resolve("c:\\foo", "/bar");
            assert.Equal("/bar/c:\\foo", abs3);

            var abs4 = Path.Resolve("../foo", "/bar/irish/tab");
            assert.Equal("/bar/irish/foo", abs4);

            var abs5 = Path.Resolve("./foo", "/bar/irish/tab");
            assert.Equal("/bar/irish/tab/foo", abs5);

            assert.Equal("/bar/irish/tab", Path.Resolve(".", "/bar/irish/tab"));
            assert.Equal("/bar/irish/tab", Path.Resolve("./", "/bar/irish/tab"));
            assert.Equal("/bar/irish/tab", Path.Resolve(".\\", "/bar/irish/tab"));

            assert.Equal("/bar/irish", Path.Resolve("./..", "/bar/irish/tab"));
            assert.Equal("/bar/irish", Path.Resolve(".\\..", "/bar/irish/tab"));
        }
    }
}