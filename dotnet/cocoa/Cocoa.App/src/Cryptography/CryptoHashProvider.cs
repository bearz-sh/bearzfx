// Copyright © 2017 - 2021 Chocolatey Software, Inc
// Copyright © 2011 - 2017 RealDimensions Software, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
//
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/cryptography/CryptoHashProvider.cs

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

using Bearz.Std;

using Cocoa.Adapters;
using Cocoa.Logging;

using Microsoft.Extensions.Logging;

using HashAlgorithm = System.Security.Cryptography.HashAlgorithm;

namespace Cocoa.Cryptography;

public class CryptoHashProvider : IHashProvider
{
    private const int ErrorLockViolation = 33;
    private const int ErrorSharingViolation = 32;
    private readonly ILogger log;
    private IHashAlgorithm hashAlgorithm;

    public CryptoHashProvider(IHashAlgorithm? hashAlgorithm = null)
    {
        this.hashAlgorithm = hashAlgorithm ?? GetHashAlgorithm(HashAlgorithmName.SHA256);
        this.log = Log.For<CryptoHashProvider>();
    }

    public static string ComputeStringHash(string originalText, HashAlgorithmName providerType)
    {
        var hashAlgorithm = GetHashAlgorithm(providerType);

        var hash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(originalText));
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    public void SetHashAlgorithm(HashAlgorithmName algorithmType)
    {
         this.hashAlgorithm = GetHashAlgorithm(algorithmType);
    }

    public string ComputeFileHash(string filePath)
    {
        if (!Fs.FileExists(filePath)) return string.Empty;

        try
        {
            var hash = this.hashAlgorithm.ComputeHash(Fs.ReadFile(filePath));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
        catch (IOException ex)
        {
            this.log.LogWarning(
                $"Error computing hash for '{filePath}'{Environment.NewLine} Hash will be special code for locked file or file too big instead.{Environment.NewLine} Captured error:{Environment.NewLine}  {ex.Message}");

            if (IsFileLocked(ex))
            {
                return ApplicationParameters.HashProviderFileLocked;
            }

            // IO.IO_FileTooLong2GB (over Int32.MaxValue)
            return ApplicationParameters.HashProviderFileTooBig;
        }
    }

    public string ComputeByteArrayHash(byte[] buffer)
    {
        var hash = this.hashAlgorithm.ComputeHash(buffer);

        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    public string ComputeStreamHash(Stream inputStream)
    {
        var hash = this.hashAlgorithm.ComputeHash(inputStream);

        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    [SuppressMessage("Security", "SCS0006:Weak hashing function.")]
    private static IHashAlgorithm GetHashAlgorithm(HashAlgorithmName algorithmType)
    {
        HashAlgorithm algo;
        switch (algorithmType.Name)
        {
            case "SHA1":
                algo = SHA1.Create();
                break;
            case "SHA256":
                algo = SHA256.Create();
                break;
            case "SHA384":
                algo = SHA384.Create();
                break;

            case "SHA512":
                algo = SHA512.Create();
                break;

            case "MD5":
                algo = MD5.Create();
                break;

            default:
                throw new NotSupportedException($"Hash algorithm not supported ${algorithmType.Name} ");
        }

        return new Adapters.HashAlgorithm(algo);
    }

    private static bool IsFileLocked(Exception exception)
    {
        var errorCode = 0;

        var hresult = Marshal.GetHRForException(exception);

        errorCode = hresult & ((1 << 16) - 1);

        return errorCode == ErrorSharingViolation || errorCode == ErrorLockViolation;
    }
}