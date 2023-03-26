using Bearz.Std;

using Path = Bearz.Std.Path;

namespace Test.Std;

public class Fs_Tests
{
    private static readonly string Csproj = Path.Combine(Util.StandardDir, "src", "Bearz.Standard.csproj");

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
        var tmp = Path.TempDir();
        var dir = Path.Combine(tmp, "foo", "bar");
        var parent = Path.Dirname(dir)!;
        Fs.RemoveDirectory(parent, true);
        assert.False(Fs.DirectoryExits(dir));
        Fs.MakeDirectory(dir);
        assert.True(Fs.DirectoryExits(dir));

        Fs.RemoveDirectory(parent, true);
        assert.False(Fs.DirectoryExits(dir));
    }
}