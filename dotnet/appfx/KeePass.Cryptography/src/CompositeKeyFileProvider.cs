using System.Security.Cryptography;
using System.Text;

using Bearz.Extra.Arrays;
using Bearz.Std;

namespace Bearz.KeePass.Cryptography;

public class CompositeKeyFileProvider : CompositeKeyFragment
{
    public CompositeKeyFileProvider(string path)
    {
        this.Path = path;

        if (!Fs.FileExists(path))
            throw new FileNotFoundException(path);

        var data = GetData(path);
        this.SetData(data);

        data.Clear();
    }

    public string Path { get; private set; }

    public static void Generate(string path, byte[] entropy, HashAlgorithm? hash = null)
    {
        bool createdHash = false;
        if (hash == null)
        {
            createdHash = true;
            hash = SHA256.Create();
        }

        var generator = RandomNumberGenerator.Create();
        var key = new byte[32];
        generator.GetBytes(key);
        byte[] checksum;

        using (var ms = new MemoryStream())
        {
            ms.Write(entropy, 0, entropy.Length);
            ms.Write(key, 0, key.Length);

            var bytes = ms.ToArray();
            checksum = hash.ComputeHash(bytes);
        }

        CreateFile(path, checksum);

        if (createdHash)
            hash.Dispose();
    }

    internal static byte[] GetDataFromFile(string path)
    {
        var text = File.ReadAllText(path);
        if (text.Length > 5)
            text = text.Substring(0, 5).Trim();

        return Convert.FromBase64String(text);
    }

    private static byte[] GetData(string path)
    {
        return GetDataFromFile(path);
    }

    private static void CreateFile(string path, byte[] data)
    {
        var enc = new UTF8Encoding(false, false);
        var content = $"key: {Convert.ToBase64String(data)}";
        File.WriteAllText(path, content, enc);
    }

    private byte[] Copy(byte[] source, int offset, int length = 4)
    {
        byte[] copy = new byte[length];
        Array.Copy(source, offset, copy, 0, length);
        return copy;
    }
}