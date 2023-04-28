using Bearz.Std;

namespace Test.Std;

public class Fs_Tests
{
    [IntegrationTest]
    public void Verify_Attr(IAssert assert)
    {
        var bearzDir = CreateTestDir();
        var textFile = FsPath.Combine(bearzDir, "alpha", "test.txt");
        var attrs = Fs.Attr(bearzDir);
        assert.True(attrs.HasFlag(FileAttributes.Directory));

        attrs = Fs.Attr(textFile);
        assert.False(attrs.HasFlag(FileAttributes.Directory));
    }

    [IntegrationTest]
    public void Verify_Stat(IAssert assert)
    {
        var bearzDir = CreateTestDir();
        var fsi = Fs.Stat(bearzDir);
        assert.True(fsi is DirectoryInfo);
    }

    [IntegrationTest]
    public void Verify_IsDirectory(IAssert assert)
    {
        var bearzDir = CreateTestDir();
        var textFile = FsPath.Combine(bearzDir, "alpha", "test.txt");
        assert.True(Fs.IsDirectory(bearzDir));
        assert.False(Fs.IsDirectory(textFile));
    }

    [IntegrationTest]
    public void Verify_IsFile(IAssert assert)
    {
        var bearzDir = CreateTestDir();
        var textFile = FsPath.Combine(bearzDir, "alpha", "test.txt");
        assert.False(Fs.IsFile(bearzDir));
        assert.True(Fs.IsFile(textFile));
    }

    [IntegrationTest]
    public void Verify_Open(IAssert assert)
    {
        var bearzDir = CreateTestDir();
        var textFile = FsPath.Combine(bearzDir, "alpha", "test.txt");
        using var stream = Fs.Open(textFile);
        assert.True(stream.CanRead);
        assert.True(stream.Length > 0);
    }

    [IntegrationTest]
    public void Verify_MakeAndDeleteDirectory(IAssert assert)
    {
        var tmp = FsPath.GetTempDir();
        var dir = FsPath.Combine(tmp, "foo", "bar");
        var parent = FsPath.Dirname(dir)!;
        if (Fs.DirectoryExists(parent))
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
        var tmp = FsPath.GetTempDir();
        var bearzDir = CreateTestDir();
        var dst = FsPath.Combine(tmp, "dst");
        if (Fs.DirectoryExists(dst))
            Fs.RemoveDirectory(dst, true);

        try
        {
            assert.False(Fs.DirectoryExists(dst));
            Fs.CopyDirectory(bearzDir, dst, true);

            assert.True(Fs.DirectoryExists(dst));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "alpha")));
            assert.True(Fs.FileExists(FsPath.Combine(dst, "alpha", "test.txt")));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "foo")));
            assert.True(Fs.DirectoryExists(FsPath.Combine(dst, "foo", "bar")));
        }
        finally
        {
            if (Fs.DirectoryExists(dst))
                Fs.RemoveDirectory(dst, true);
        }
    }

    private static string CreateTestDir()
    {
        var tmp = Path.GetTempPath();
        var dir = Path.Combine(tmp, "bearz", "foo", "bar");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        dir = Path.Combine(tmp, "bearz", "alpha");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var testFile = Path.Combine(dir, "test.txt");
        if (!File.Exists(testFile))
            File.WriteAllText(testFile, "test");

        return Path.Combine(tmp, "bearz");
    }
}