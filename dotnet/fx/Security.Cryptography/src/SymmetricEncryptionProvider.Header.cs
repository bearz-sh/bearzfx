using System;
using System.IO;
using System.Security.Cryptography;

using Bearz.Text;

// ReSharper disable ParameterHidesMember
namespace Bearz.Security.Cryptography
{
    /// <summary>
    /// PArtial class.
    /// </summary>
    public partial class SymmetricEncryptionProvider
    {
        protected internal Header ReadHeader(
            Stream reader,
            ISymmetricEncryptionProviderOptions options,
            ReadOnlySpan<byte> privateKey = default,
            IEncryptionProvider? symmetricKeyEncryptionProvider = null)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            ReadOnlySpan<byte> signingKey = default;

            if (options.SigningKey != null)
                signingKey = options.SigningKey.Memory.Span;

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms, Encodings.Utf8NoBom, true))
            using (var br = new BinaryReader(reader, Encodings.Utf8NoBom, true))
            {
                var version = br.ReadInt16();
                bw.Write(version);
                Header header;
                switch (version)
                {
                    default:
                        header = new HeaderV1();
                        break;
                }

                // header shorts/ints
                // 1. version
                // 2. algo
                // 3. signing,
                // 4. metadataSize
                // 5  iterations
                // 6. symmetricSaltSize
                // 7. signingSaltSize
                // 8. ivSize
                // 9. symmetricKeySize
                // 10. hashSize

                // header values
                // 1. metadata
                // 2. symmetricSalt
                // 3. signingSalt
                // 4. iv
                // 5. symmetricKey
                // 6. hash
                header.SymmetricAlgorithmType = (SymmetricAlgorithmType)br.ReadInt16();
                header.KeyedHashAlgorithmType = (KeyedHashAlgorithmType)br.ReadInt16();
                header.MetaDataSize = br.ReadInt32();
                header.Iterations = br.ReadInt32();
                header.SymmetricSaltSize = br.ReadInt16();
                header.SigningSaltSize = br.ReadInt16();
                header.IvSize = br.ReadInt16();
                header.SymmetricKeySize = br.ReadInt16();
                header.HashSize = br.ReadInt16();

                bw.Write((short)header.SymmetricAlgorithmType);
                bw.Write((short)header.KeyedHashAlgorithmType);
                bw.Write(header.MetaDataSize);
                bw.Write(header.Iterations);
                bw.Write(header.SymmetricSaltSize);
                bw.Write(header.SigningSaltSize);
                bw.Write(header.IvSize);
                bw.Write(header.SymmetricKeySize);
                bw.Write(header.HashSize);

                if (options.SymmetricAlgorithm != header.SymmetricAlgorithmType)
                {
                    options.SymmetricAlgorithm = header.SymmetricAlgorithmType;
                    this.algorithm = null;
                }

                if (options.KeyedHashedAlgorithm != header.KeyedHashAlgorithmType)
                {
                    options.KeyedHashedAlgorithm = header.KeyedHashAlgorithmType;
                    this.signingAlgorithm = null;
                }

                var symmetricSalt = Array.Empty<byte>();
                var signingSalt = Array.Empty<byte>();
                var iv = Array.Empty<byte>();
                var symmetricKey = Array.Empty<byte>();
                byte[] hash = Array.Empty<byte>();

                if (header.MetaDataSize > 0)
                {
                    bw.Write(br.ReadBytes(header.MetaDataSize));
                }

                if (header.SymmetricSaltSize > 0)
                {
                    bw.Write(br.ReadBytes(header.SymmetricSaltSize));
                }

                if (header.SigningSaltSize > 0)
                {
                    signingSalt = br.ReadBytes(header.SigningSaltSize);
                    bw.Write(signingSalt);
                }

                if (header.IvSize > 0)
                {
                    iv = br.ReadBytes(header.IvSize);
                    bw.Write(iv);
                }

                if (header.SymmetricKeySize > 0)
                {
                    symmetricKey = br.ReadBytes(header.SymmetricKeySize);
                    bw.Write(symmetricKey);
                }

                if (header.HashSize > 0)
                {
                    hash = br.ReadBytes(header.HashSize);
                    bw.Write(hash);
                }

                bw.Flush();
                ms.Flush();
                header.Bytes = ms.ToArray();

                header.Position = reader.Position;

                if (symmetricKeyEncryptionProvider != null)
                    symmetricKey = symmetricKeyEncryptionProvider.Decrypt(symmetricKey);

                if (symmetricKey.Length == 0 && privateKey.IsEmpty)
                {
                    throw new ArgumentNullException(
                        nameof(privateKey),
                        "privateKey or symmetricKey must have a value");
                }

                if (!options.SkipSigning && privateKey == null && signingKey.IsEmpty)
                {
                    throw new ArgumentNullException(
                        nameof(privateKey),
                        "privateKey must have a value or options.SigningKey must have a value or options.SkipSigning must be true");
                }

                if (symmetricKey.Length == 0)
                {
                    if (symmetricSalt == null)
                        throw new InvalidOperationException("symmetricSalt for the privateKey could not be retrieved");

                    using var generator = new BearzRfc2898DeriveBytes(
                        privateKey,
                        symmetricSalt,
                        header.Iterations,
                        HashAlgorithmName.SHA256);
                    header.SymmetricKey = generator.GetBytes(options.KeySize / 8);
                }

                if (!options.SkipSigning && signingKey.IsEmpty)
                {
                    if (signingSalt == null)
                        throw new InvalidOperationException("symmetricSalt for the privateKey could not be retrieved");

                    var key = symmetricKey.Length != 0 ? symmetricKey : privateKey;
                    using var generator = new BearzRfc2898DeriveBytes(
                        key,
                        signingSalt,
                        header.Iterations,
                        HashAlgorithmName.SHA256);
                    generator.IterationCount = header.Iterations;
                    header.SigningKey = generator.GetBytes(options.KeySize / 8);
                }

                if (header.SymmetricKeySize > 0)
                {
                    header.SymmetricKey = symmetricKey;
                }

                header.IV = iv;
                header.Hash = hash;

                return header;
            }
        }

        protected internal Header GenerateHeader(
            ISymmetricEncryptionProviderOptions options,
            ReadOnlySpan<byte> symmetricKey = default,
            ReadOnlySpan<byte> privateKey = default,
            ReadOnlySpan<byte> metadata = default,
            IEncryptionProvider? symmetricKeyEncryptionProvider = null)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (privateKey.IsEmpty && options.Key != null)
            {
                privateKey = options.Key.Memory.Span;
            }

            ReadOnlySpan<byte> signingKey = default;
            if (options.SigningKey != null)
                signingKey = options.SigningKey.Memory.Span;

            // header values
            // 1. version
            // 2. metadataSize
            // 3. iterations
            // 4. symmetricSaltSize
            // 5. signingSaltSize
            // 6. ivSize
            // 7. symmetricKeySize
            // 8. hashSize

            // header values
            // 1. metadata (optional)
            // 2. symmetricSalt (optional)
            // 3. signingSalt (optional)
            // 4. iv
            // 5. symmetricKey (optional)
            // 6. hash
            var header = new HeaderV1 { MetaDataSize = metadata.Length, };

            bool privateKeyEmpty = privateKey.IsEmpty;
            bool symmetricKeyEmpty = symmetricKey.IsEmpty;

            if (privateKeyEmpty && symmetricKeyEmpty)
                throw new ArgumentNullException(nameof(privateKey), "privateKey or symmetricKey must have a value");

            if (!options.SkipSigning && privateKeyEmpty && signingKey.IsEmpty)
            {
                throw new ArgumentNullException(
                    nameof(privateKey),
                    "privateKey must have a value or options.SigningKey must have a value or options.SkipSigning must be true");
            }

            if (!privateKeyEmpty)
            {
                header.SymmetricSaltSize = (short)(options.SaltSize / 8);

                if (!options.SkipSigning && signingKey.IsEmpty)
                {
                    header.SigningSaltSize = (short)(options.SaltSize / 8);
                    this.signingAlgorithm ??= CreateSigningAlgorithm(options);
                }
            }

            if (!symmetricKeyEmpty)
            {
                header.SymmetricKeySize = (short)(options.KeySize / 8);
            }

            this.algorithm = this.algorithm ?? CreateSymmetricAlgorithm(options);
            this.algorithm.GenerateIV();
            var iv = this.algorithm.IV;
            header.IvSize = (short)iv.Length;
            header.IV = iv;

            if (this.signingAlgorithm != null)
                header.HashSize = (short)(this.signingAlgorithm.HashSize / 8);

            header.Iterations = options.Iterations;
            using (var ms = new MemoryStream(new byte[header.HeaderSize]))
            using (var bw = new BinaryWriter(ms, Encodings.Utf8NoBom, false))
            {
                if (!symmetricKey.IsEmpty && symmetricKeyEncryptionProvider != null)
                {
                    symmetricKey = symmetricKeyEncryptionProvider.Encrypt(symmetricKey);
                    header.SymmetricKeySize = (short)symmetricKey.Length;
                }

                header.SymmetricAlgorithmType = options.SymmetricAlgorithm;
                header.KeyedHashAlgorithmType = options.KeyedHashedAlgorithm;

                bw.Write(header.Version);
                bw.Write((short)header.SymmetricAlgorithmType);
                bw.Write((short)header.KeyedHashAlgorithmType);
                bw.Write(header.MetaDataSize);
                bw.Write(header.Iterations);
                bw.Write(header.SymmetricSaltSize);
                bw.Write(header.SigningSaltSize);
                bw.Write(header.IvSize);
                bw.Write(header.SymmetricKeySize);
                bw.Write(header.HashSize);

                if (privateKey != null)
                {
                    ReadOnlySpan<byte> symmetricSalt = GenerateSalt(header.SymmetricSaltSize);

                    if (symmetricSalt.Length != header.SymmetricSaltSize)
                    {
                        throw new InvalidOperationException(
                            $"symmetricSalt length does not match the expected sizeof {header.SymmetricSaltSize}");
                    }

                    using (var generator = new BearzRfc2898DeriveBytes(
                               privateKey,
                               symmetricSalt,
                               options.Iterations,
                               HashAlgorithmName.SHA256))
                    {
                        header.SymmetricKey = generator.GetBytes(options.KeySize / 8);
                        bw.Write(symmetricSalt);
                    }

                    if (!options.SkipSigning || !signingKey.IsEmpty)
                    {
                        var signingSalt = GenerateSalt(header.SigningSaltSize);

                        using (var generator = new BearzRfc2898DeriveBytes(
                                   privateKey, signingSalt, options.Iterations, HashAlgorithmName.SHA256))
                        {
                            header.SigningKey = generator.GetBytes(options.KeySize / 8);
                            bw.Write(signingSalt);
                        }

                        Array.Clear(signingSalt, 0, signingSalt.Length);
                    }
                }

                bw.Write(iv);
                if (!symmetricKeyEmpty)
                {
                    bw.Write(symmetricKey);
                }

                bw.Flush();
                ms.Flush();
                header.Position = ms.Position;

                header.Bytes = ms.ToArray();
            }

            return header;
        }

        protected static byte[] GenerateSalt(int length)
        {
            using (var rng = new Csrng())
            {
                return rng.NextBytes(length);
            }
        }

        private static SymmetricAlgorithm CreateSymmetricAlgorithm(
            ISymmetricEncryptionProviderOptions options)
        {
            if (options.SymmetricAlgorithm == SymmetricAlgorithmType.None)
                throw new ArgumentException("SymmetricAlgo", nameof(options));

            var algo = options.SymmetricAlgorithm.CreateSymmetricAlgorithm();
            if (algo is null)
                throw new InvalidOperationException($"Unable to create alogrithm {options.SymmetricAlgorithm}");

            algo.KeySize = options.KeySize;
            algo.Padding = options.Padding;
            algo.Mode = options.Mode;
            algo.Padding = options.Padding;

            return algo;
        }

        private static KeyedHashAlgorithm? CreateSigningAlgorithm(ISymmetricEncryptionProviderOptions options)
        {
            if (options.KeyedHashedAlgorithm == KeyedHashAlgorithmType.None)
                throw new InvalidOperationException("Unable to create keyed hash algo");

            return options.KeyedHashedAlgorithm.CreateKeyedHashAlgorithm();
        }

        protected internal class HeaderV1 : Header
        {
            // int size
            // short size.
            // meta data size.
            public override int HeaderSize =>
                (2 * 4) +
                (8 * 2) +
                this.MetaDataSize +
                this.SymmetricKeySize +
                this.SymmetricSaltSize +
                this.SigningSaltSize +
                this.IvSize +
                this.HashSize;
        }

        protected internal class Header : IDisposable
        {
            public virtual short Version { get; } = 1;

            public int MetaDataSize { get; set; }

            public short SymmetricKeySize { get; set; }

            public short SymmetricSaltSize { get; set; }

            public short SigningSaltSize { get; set; }

            public short IvSize { get; set; }

            public short HashSize { get; set; }

            public SymmetricAlgorithmType SymmetricAlgorithmType { get; set; }

            public KeyedHashAlgorithmType KeyedHashAlgorithmType { get; set; }

            public byte[] SymmetricKey { get; set; } = Array.Empty<byte>();

            public byte[] SigningKey { get; set; } = Array.Empty<byte>();

            // ReSharper disable once InconsistentNaming
            public byte[] IV { get; set; } = Array.Empty<byte>();

            public int Iterations { get; set; }

            public long Position { get; set; }

            public byte[] Bytes { get; set; } = Array.Empty<byte>();

            public byte[] Hash { get; set; } = Array.Empty<byte>();

            public virtual int HeaderSize => 0;

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                Array.Clear(this.IV, 0, this.IV.Length);
                Array.Clear(this.Bytes, 0, this.Bytes.Length);
                Array.Clear(this.SymmetricKey, 0, this.SymmetricKey.Length);
                Array.Clear(this.SigningKey, 0, this.SigningKey.Length);
                Array.Clear(this.Hash, 0, this.Hash.Length);
            }
        }
    }
}