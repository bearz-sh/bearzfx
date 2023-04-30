namespace Bearz.KeePass.Cryptography;

public interface IProtectedBuffer
{
    ReadOnlySpan<byte> Unprotect();
}