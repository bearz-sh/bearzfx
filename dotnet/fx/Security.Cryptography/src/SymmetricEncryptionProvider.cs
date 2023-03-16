using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;

using Bearz.Extra.Arrays;
using Bearz.Text;

namespace Bearz.Security.Cryptography
{
    /// <summary>
    /// A configurable symmetric encryption engine that defaults to an encrypt and
    /// then MAC scheme using AES 256 in CBC mode with PKCS7 padding and
    /// uses message authentication with HMAC-SHA-256.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     The engine writes meta information to the storage mechanism which akin
    ///     to file headers at the beginning of the bytes. This is to ensure
    ///     the data can be decrypted even if the provider changes it's implementation.
    ///     </para>
    ///     <para>
    ///     The header information includes a version number, the nonce, hash,
    ///     and allows for additional decrypted information to be stored.
    ///     </para>
    ///     <para>
    ///     A symmetric key will be stored if an implementation <see cref="IEncryptionProvider" />
    ///     is provided. This is useful for leveraging certificates that encrypt and
    ///     the decrypt the symmetric key. The decrypted key is used by the symmetric
    ///     algorithm to decrypt/encrypt the data.
    ///     </para>
    /// </remarks>
    public partial class SymmetricEncryptionProvider : ISymmetricEncryptionProvider
    {
        private readonly ISymmetricEncryptionProviderOptions options;

        private readonly bool internallyControlled;

        private SymmetricAlgorithm? algorithm;

        private KeyedHashAlgorithm? signingAlgorithm;

        private bool isDisposed;

        public SymmetricEncryptionProvider(ISymmetricEncryptionProviderOptions? options = null)
        {
            if (options is null)
            {
                this.internallyControlled = true;
                this.options = new SymmetricEncryptionProviderOptions();
            }
            else
            {
                this.options = options;
            }
        }

        /// <summary>
        /// Decrypts encrypted data and returns the decrypted bytes.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="privateKey">
        /// A password or phrase used to generate the key for the symmetric algorithm. If the symmetric
        /// key is stored with the message, the key for the symmetric algorithm is used instead.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public ReadOnlySpan<byte> Decrypt(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> privateKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider)
        {
            var encrypted = data.ToArray();

            try
            {
                using (var reader = new MemoryStream(encrypted))
                using (var header = this.ReadHeader(reader, this.options, privateKey, symmetricKeyEncryptionProvider))
                {
                    if (header.SymmetricKey is null)
                        throw new InvalidOperationException("Unable to read SymmetricKey from header");

                    if (header.IV is null)
                        throw new InvalidOperationException("Unable to read IV from header");

                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);
                    var messageSize = data.Length - header.HeaderSize;
                    var message = new byte[messageSize];
                    Array.Copy(encrypted, header.HeaderSize, message, 0, messageSize);

                    if (header.Hash.Length > 0)
                    {
                        if (header.SigningKey is null)
                            throw new InvalidOperationException("SigningKey is null");

                        using (var signer = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options)!)
                        {
                            signer.Key = header.SigningKey;
                            var h1 = header.Hash.AsSpan();
                            Span<byte> h2 = signer.ComputeHash(message);

                            if (!h1.SlowEquals(h2))
                            {
                                message.Clear();
                                return null;
                            }
                        }
                    }

                    var symmetricKey = header.SymmetricKey;
                    var iv = header.IV;
                    using (var decryptor = this.algorithm.CreateDecryptor(symmetricKey, iv))
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(message.AsSpan());
                        message.Clear();
                        cs.Flush();
                        cs.FlushFinalBlock();
                        ms.Flush();
                        return ms.ToArray();
                    }
                }
            }
            finally
            {
                Array.Clear(encrypted, 0, encrypted.Length);
            }
        }

        /// <summary>
        /// Decrypts encrypted data and returns the decrypted bytes.
        /// </summary>
        /// <param name="reader">The data stream to read from.</param>
        /// <param name="writer">The data stream to write to.</param>
        /// <param name="privateKey">
        /// A password or phrase used to generate the key for the symmetric algorithm. If the symmetric
        /// key is stored with the message, the key for the symmetric algorithm is used instead.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        [SuppressMessage(
            "Design",
            "CA2000: Use dispose",
            Justification = "CryptoStream's dispose will dispose of the underlying writer")]
        public void Decrypt(
            Stream reader,
            Stream writer,
            ReadOnlySpan<byte> privateKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            var pool = ArrayPool<byte>.Shared;
            byte[] buffer = Array.Empty<byte>();
            try
            {
                buffer = pool.Rent(4096);
                using (var header = this.ReadHeader(reader, this.options, privateKey, symmetricKeyEncryptionProvider))
                {
                    if (header.SymmetricKey is null)
                        throw new InvalidOperationException("Unable to read SymmetricKey from header");

                    if (header.IV is null)
                        throw new InvalidOperationException("Unable to read IV from header");

                    if (header.Bytes is null)
                        throw new InvalidOperationException("Unable to read Bytes from header");

                    this.algorithm ??= CreateSymmetricAlgorithm(this.options);

                    if (header.Hash.Length > 0)
                    {
                        if (header.SigningKey is null)
                            throw new InvalidOperationException("SigningKey is null");

                        using (var signer = CreateSigningAlgorithm(this.options))
                        {
                            if (signer is null)
                                throw new InvalidOperationException("Unable to create signing alorithm");

                            signer.Key = header.SigningKey;
                            var h1 = header.Hash;

                            reader.Seek(header.HeaderSize, SeekOrigin.Begin);
                            var h2 = signer.ComputeHash(reader);

                            if (!h1.SlowEquals(h2))
                                return;
                        }
                    }

                    reader.Seek(header.HeaderSize, SeekOrigin.Begin);
                    using (var decryptor = this.algorithm.CreateDecryptor(header.SymmetricKey, header.IV))
                    {
                        // TODO: create a sudo stream that breaks a dispose call.
                        var cs = new CryptoStream(writer, decryptor, CryptoStreamMode.Write);

                        long bytesRead = reader.Length - header.HeaderSize;
                        while (bytesRead > 0)
                        {
                            int read = reader.Read(buffer, 0, buffer.Length);
                            bytesRead -= read;
                            cs.Write(buffer, 0, read);
                        }

                        cs.Flush();
                        cs.FlushFinalBlock();
                        writer.Flush();
                    }
                }
            }
            finally
            {
                pool.Return(buffer, true);
            }
        }

        public byte[] DecryptBytes(
           byte[] blob,
           byte[]? privateKey = default,
           IEncryptionProvider? symmetricKeyEncryptionProvider = null)
        {
#if NETSTANDARD2_0
            var span = this.Decrypt(
                blob.AsSpan(),
                privateKey.AsSpan(),
                symmetricKeyEncryptionProvider);

            return span.ToArray();
#else
            var span = this.Decrypt(blob, privateKey, symmetricKeyEncryptionProvider);
            return span.ToArray();
#endif
        }

        public byte[] EncryptBytes(
           byte[] blob,
           byte[]? privateKey = default,
           byte[]? symmetricKey = default,
           IEncryptionProvider? symmetricKeyEncryptionProvider = null)
        {
#if NETSTANDARD2_0
            var span = this.Encrypt(
                blob.AsSpan(),
                privateKey.AsSpan(),
                symmetricKey.AsSpan(),
                symmetricKeyEncryptionProvider);

            return span.ToArray();
#else
            var span = this.Encrypt(blob, privateKey, symmetricKey, symmetricKeyEncryptionProvider);
            return span.ToArray();
#endif
        }

        /// <summary>
        /// Encrypts the data and returns the encrypted bytes.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="privateKey">
        ///  A password or phrase used to generate the key for the symmetric algorithm.
        /// </param>
        /// <param name="symmetricKey">
        ///  The key for the symmetric algorithm. If used, the private key is ignored
        ///  and the symmetric key is stored with the message.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to encrypt/decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public ReadOnlySpan<byte> Encrypt(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> privateKey,
            ReadOnlySpan<byte> symmetricKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            ArrayPool<byte> pool = ArrayPool<byte>.Shared;
            byte[] headerBuffer = Array.Empty<byte>();
            try
            {
                using (var header = this.GenerateHeader(this.options, symmetricKey, privateKey, null, symmetricKeyEncryptionProvider))
                {
                    headerBuffer = pool.Rent(header.HeaderSize);
                    if (header.SymmetricKey is null)
                        throw new InvalidOperationException("Unable to read SymmetricKey from header");

                    if (header.IV is null)
                        throw new InvalidOperationException("Unable to read IV from header");

                    if (header.Bytes is null)
                        throw new InvalidOperationException("Unable to read Bytes from header");

                    byte[] encryptedBlob;
                    this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(this.options);
                    using (var encryptor = this.algorithm.CreateEncryptor(header.SymmetricKey, header.IV))
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data);
                        cs.Flush();
                        cs.FlushFinalBlock();
                        ms.Flush();
                        encryptedBlob = ms.ToArray();
                    }

                    if (!this.options.SkipSigning && header.SigningKey.Length != 0)
                    {
                        this.signingAlgorithm ??= CreateSigningAlgorithm(this.options)!;
                        this.signingAlgorithm.Key = header.SigningKey;
                        var hash = this.signingAlgorithm.ComputeHash(encryptedBlob);

                        Array.Copy(hash, 0, headerBuffer, header.Position, hash.Length);

                        hash.Clear();
                    }

                    using (var ms = new MemoryStream())
                    {
                        using (var writer = new BinaryWriter(ms, Encodings.Utf8NoBom, true))
                        {
                            writer.Write(headerBuffer, 0, header.HeaderSize);
                        }

                        ms.Write(encryptedBlob);
                        encryptedBlob.Clear();
                        ms.Flush();
                        return ms.ToArray();
                    }
                }
            }
            finally
            {
                if (headerBuffer.Length > 0)
                    pool.Return(headerBuffer);
            }
        }

        /// <summary>
        /// Encrypts the data and returns the encrypted bytes.
        /// </summary>
        /// <param name="readStream">The data stream to read from.</param>
        /// <param name="writeStream">The data stream to write to.</param>
        /// <param name="privateKey">
        ///  A password or phrase used to generate the key for the symmetric algorithm.
        /// </param>
        /// <param name="symmetricKey">
        ///  The key for the symmetric algorithm. If used, the private key is ignored
        ///  and the symmetric key is stored with the message.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to encrypt/decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        [SuppressMessage(
            "Design",
            "CA2000: Use dispose",
            Justification = "CryptoStream's dispose will dispose of the underlying writer")]
        public void Encrypt(
            Stream readStream,
            Stream writeStream,
            ReadOnlySpan<byte> privateKey,
            ReadOnlySpan<byte> symmetricKey,
            IEncryptionProvider? symmetricKeyEncryptionProvider)
        {
            if (readStream is null)
                throw new ArgumentNullException(nameof(readStream));

            if (writeStream is null)
                throw new ArgumentNullException(nameof(writeStream));

            ArrayPool<byte> pool = ArrayPool<byte>.Shared;
            byte[] buffer = pool.Rent(4096);
            byte[] headerBuffer = Array.Empty<byte>();
            try
            {
                using (var header = this.GenerateHeader(this.options, symmetricKey, privateKey, null, symmetricKeyEncryptionProvider))
                {
                    if (header.SymmetricKey is null)
                        throw new InvalidOperationException("Unable to read SymmetricKey from header");

                    if (header.IV is null)
                        throw new InvalidOperationException("Unable to read IV from header");

                    if (header.Bytes is null)
                        throw new InvalidOperationException("Unable to read Bytes from header");

                    this.algorithm ??= CreateSymmetricAlgorithm(this.options);

                    using (var encryptor = this.algorithm.CreateEncryptor(header.SymmetricKey, header.IV))
                    {
                        var cs = new CryptoStream(writeStream, encryptor, CryptoStreamMode.Write);
                        using (var bw = new BinaryWriter(writeStream, Encodings.Utf8NoBom, true))
                        {
                            bw.Write(new byte[header.HeaderSize]);
                            bw.Flush();
                        }

                        long bytesLeft = readStream.Length;

                        while (bytesLeft > 0)
                        {
                            int read = readStream.Read(buffer, 0, buffer.Length);
                            bytesLeft -= read;
                            cs.Write(buffer, 0, read);
                        }

                        cs.Flush();
                        cs.FlushFinalBlock();
                        writeStream.Flush();
                    }

                    headerBuffer = pool.Rent(header.HeaderSize);
                    header.Bytes.CopyTo(headerBuffer, header.Bytes.Length);

                    if (!this.options.SkipSigning
                        && this.options.KeyedHashedAlgorithm != KeyedHashAlgorithmType.None
                        && header.SigningKey.Length > 0)
                    {
                        using (var signer = CreateSigningAlgorithm(this.options)!)
                        {
                            signer.Key = header.SigningKey;
                            writeStream.Seek(header.HeaderSize, SeekOrigin.Begin);
                            var hash = signer.ComputeHash(writeStream);

                            Array.Copy(hash, 0, headerBuffer, header.Position, hash.Length);
                            hash.Clear();
                        }
                    }

                    writeStream.Seek(0, SeekOrigin.Begin);
                    using (var bw = new BinaryWriter(writeStream, Encodings.Utf8NoBom, true))
                    {
                        bw.Write(headerBuffer, 0, header.HeaderSize);
                        writeStream.Flush();
                        writeStream.Seek(0, SeekOrigin.End);
                    }
                }
            }
            finally
            {
                pool.Return(buffer, true);
                if (headerBuffer.Length > 0)
                    pool.Return(headerBuffer, true);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            if (disposing)
            {
                this.algorithm?.Dispose();
                this.signingAlgorithm?.Dispose();

                if (this.internallyControlled)
                    this.options.Dispose();
            }

            this.isDisposed = true;
        }
    }
}