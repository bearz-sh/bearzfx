using System;
using System.IO;

namespace Bearz.Security.Cryptography
{
    public interface ISymmetricEncryptionProvider : IDisposable
    {
        ReadOnlySpan<byte> Encrypt(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> privateKey,
            ReadOnlySpan<byte> symmetricKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider);

        void Encrypt(
            Stream readStream,
            Stream writeStream,
            ReadOnlySpan<byte> privateKey,
            ReadOnlySpan<byte> symmetricKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider);

        ReadOnlySpan<byte> Decrypt(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> privateKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider);

        void Decrypt(
            Stream reader,
            Stream writer,
            ReadOnlySpan<byte> privateKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider);
    }
}