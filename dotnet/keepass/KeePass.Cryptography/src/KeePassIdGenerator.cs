using System.Security.Cryptography;

namespace Bearz.KeePass.Cryptography;

public static class KeePassIdGenerator
{
    private static readonly List<byte[]> Ids = new List<byte[]>();
    private static readonly object SyncLock = new object();
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public static byte[] GenerateId()
    {
        lock (SyncLock)
        {
            var iv = new byte[8];
            Rng.GetBytes(iv);

            while (Ids.Any(o => o.SequenceEqual(iv)))
            {
                Rng.GetBytes(iv);
            }

            Ids.Add(iv);

            return iv;
        }
    }
}