using System.Buffers;
using System.Diagnostics.CodeAnalysis;

using Bearz.Security.Cryptography;

namespace Bearz.KeePass.Cryptography;

[SuppressMessage("Minor Code Smell", "S3963:\"static\" fields should be initialized inline")]
public static class ProtectionOptions
{
    private static DataProtectionAction s_dataProtectionAction;

    private static bool s_locked = false;

    static ProtectionOptions()
    {
        var algo = ChaCha20.Create();
        algo.GenerateKey();
        var key = algo.Key;

        // This is defaulted to the Salsa20 Stream Cipher
        // because the ProtectedMemory Api is windows specific.
        s_dataProtectionAction = (data, state, operation) =>
        {
            var iv = (byte[])state;
            var transform = operation == DataProtectionActionType.Encrypt
                ? algo.CreateEncryptor(key, iv)
                : algo.CreateDecryptor(key, iv);

            var copy = ArrayPool<byte>.Shared.Rent(data.Length);
            data.CopyTo(copy);
            var result = transform.TransformFinalBlock(copy, 0, data.Length);
            ArrayPool<byte>.Shared.Return(copy, true);
            return result;
        };
    }

    public static DataProtectionAction DataProtectionAction
    {
        get
        {
            s_locked = true;
            return s_dataProtectionAction;
        }

        set
        {
            if (s_locked)
                throw new InvalidOperationException("Once the DataProtectionAction get method is called, it cannot be changed.");

            s_dataProtectionAction = value;
        }
    }
}