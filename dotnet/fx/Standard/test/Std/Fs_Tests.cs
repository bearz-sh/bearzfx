using Bearz.Std;

namespace Test.Std;

public class Fs_Tests
{
    private static readonly string Csproj = FsPath.Combine(Util.StandardDir, "src", "Bearz.Standard.csproj");

    [IntegrationTest]
    public void Verify_Attr(IAssert assert)
    {
        var attrs = Fs.Attr(Util.StandardDir);
        assert.True(attrs.HasFlag(FileAttributes.Directory));

        attrs = Fs.Attr(Csproj);
        assert.False(attrs.HasFlag(FileAttributes.Directory));
    }

    [IntegrationTest]
    public void Verify_Stat(IAssert assert)
    {
        var fsi = Fs.Stat(Util.StandardDir);
        assert.True(fsi is DirectoryInfo);
    }

    [IntegrationTest]
    public void Verify_IsDirectory(IAssert assert)
    {
        assert.True(Fs.IsDirectory(Util.StandardDir));
        assert.False(Fs.IsDirectory(Csproj));
    }

    [IntegrationTest]
    public void Verify_IsFile(IAssert assert)
    {
        assert.False(Fs.IsFile(Util.StandardDir));
        assert.True(Fs.IsFile(Csproj));
    }

    [IntegrationTest]
    public void Verify_Open(IAssert assert)
    {
        using var stream = Fs.Open(Csproj);
        assert.True(stream.CanRead);
        assert.True(stream.Length > 0);
    }

    [IntegrationTest]
    public void Verify_MakeAndDeleteDirectory(IAssert assert)
    {
        var tmp = FsPath.TempDir();
        var dir = FsPath.Combine(tmp, "foo", "bar");
        var parent = FsPath.Dirname(dir)!;
        Fs.RemoveDirectory(parent, true);
        assert.False(Fs.DirectoryExists(dir));
        Fs.MakeDirectory(dir);
        assert.True(Fs.DirectoryExists(dir));

        Fs.RemoveDirectory(parent, true);
        assert.False(Fs.DirectoryExists(dir));
    }

    [IntegrationTest]
    public void Verify_CopyDirectory(IAssert assert)
    {
        var tmp = FsPath.TempDir();
        var src = FsPath.Combine(Util.StandardDir);
        var dst = FsPath.Combine(tmp, "dst");
        if (Fs.DirectoryExists(dst))
            Fs.RemoveDirectory(dst, true);

        try
        {
            assert.False(Fs.DirectoryExists(dst));
            Fs.CopyDirectory(src, dst, true, false);

            assert.True(Fs.DirectoryExists(dst));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst)));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "src")));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "test")));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "test", "Std")));
            assert.True(Fs.FileExists(FsPath.Combine(dst, "test", "Std", "Fs_Tests.cs")));
        }
        finally
        {
            if (Fs.DirectoryExists(dst))
                Fs.RemoveDirectory(dst, true);
        }
    }
}