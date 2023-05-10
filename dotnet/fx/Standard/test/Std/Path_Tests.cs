using Bearz.Std;

namespace Test.Std;

// ReSharper disable once InconsistentNaming
public class Path_Tests
{
    [UnitTest]
    public void Resolve_BasePathMustBeAbsolute(IAssert assert)
    {
        assert.Throws<InvalidOperationException>(() => FsPath.Resolve("foo", "bar"));

        if (Env.IsWindows)
        {
            assert.Throws<InvalidOperationException>(() => FsPath.Resolve("foo", "c:bar"));
            assert.Throws<InvalidOperationException>(() => FsPath.Resolve("foo", "/bar"));
        }
        else
        {
            assert.Throws<InvalidOperationException>(() => FsPath.Resolve("foo", "c:\\bar"));
        }
    }

    [UnitTest]
    public void Resolve(IAssert assert)
    {
        // test resolve relative path
        var cwd = Env.Cwd;
        var hd = Env.GetDirectory(SpecialDirectory.Home);
        assert.Equal(hd, FsPath.Resolve("~"));

        assert.Equal(hd, FsPath.Resolve("~", cwd));
        var rel = FsPath.Resolve("foo", cwd);
        assert.Equal(FsPath.Combine(cwd, "foo"), rel);

        assert.Equal(cwd, FsPath.Resolve(".", cwd));
        assert.Equal(cwd, FsPath.Resolve("./", cwd));
        assert.Equal(cwd, FsPath.Resolve(".\\", cwd));

        if (Env.IsWindows)
        {
            assert.Equal($"{hd}\\Desktop", FsPath.Resolve("~/Desktop"));

            // test resolve and absolute paths
            var abs = FsPath.Resolve("c:\\foo", cwd);
            assert.Equal("c:\\foo", abs);

            // test resolve and relative paths
            var rel2 = FsPath.Resolve("foo", "c:\\bar");
            assert.Equal("c:\\bar\\foo", rel2);

            // test resolve and absolute paths
            var abs2 = FsPath.Resolve("/foo", "c:\\bar");
            assert.Equal("c:\\foo", abs2);
        }
        else
        {
            assert.Equal($"{hd}/Desktop", FsPath.Resolve("~/Desktop"));

            // test resolve and absolute paths
            var abs = FsPath.Resolve("/foo", cwd);
            assert.Equal("/foo", abs);

            // test resolve and relative paths
            var rel2 = FsPath.Resolve("foo", "/bar");
            assert.Equal("/bar/foo", rel2);

            // test resolve and absolute paths
            var abs2 = FsPath.Resolve("/foo", "/bar");
            assert.Equal("/foo", abs2);

            // test resolve and absolute paths
            var abs3 = FsPath.Resolve("c:\\foo", "/bar");
            assert.Equal("/bar/c:\\foo", abs3);

            var abs4 = FsPath.Resolve("../foo", "/bar/irish/tab");
            assert.Equal("/bar/irish/foo", abs4);

            var abs5 = FsPath.Resolve("./foo", "/bar/irish/tab");
            assert.Equal("/bar/irish/tab/foo", abs5);

            assert.Equal("/bar/irish/tab", FsPath.Resolve(".", "/bar/irish/tab"));
            assert.Equal("/bar/irish/tab", FsPath.Resolve("./", "/bar/irish/tab"));
            assert.Equal("/bar/irish/tab", FsPath.Resolve(".\\", "/bar/irish/tab"));

            assert.Equal("/bar/irish", FsPath.Resolve("./..", "/bar/irish/tab"));
            assert.Equal("/bar/irish", FsPath.Resolve(".\\..", "/bar/irish/tab"));
        }
    }
}