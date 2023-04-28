# Bearz.Security.Cryptography
<a name="top"></a>

## Description

A cryptography library for BearzFx to help with encryption for automation
purposes.

## Features

- `IEncryptionProvider` interface with a AesGcm and AES encrypt then MAC implementations.
- ChaCha20 implementation of `SymmetricAlgorithm`
- `BeaerzRfc2898DeriveBytes` is a backwards compatible implementation of PBKDF2 that enables
  Sha256, Sha512, etc for .NET Core and .NET Full Framework.

## License

MIT