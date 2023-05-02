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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/cryptography/IHashProvider.cs

using System.Security.Cryptography;

namespace Cocoa.Cryptography;

/// <summary>
/// A hash provider for hashing files.
/// </summary>
public interface IHashProvider
{
    /// <summary>
    /// Changes the algorithm.
    /// </summary>
    /// <param name="algorithmType">Type of the algorithm.</param>
    void SetHashAlgorithm(HashAlgorithmName algorithmType);

    /// <summary>
    /// Returns a hash of the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>A computed hash of the file, based on the contents.</returns>
    string ComputeFileHash(string filePath);

    /// <summary>
    /// Returns a hash of the specified stream.
    /// </summary>
    /// <param name="inputStream">The stream.</param>
    /// <returns>A computed hash of the stream, based on the contents.</returns>
    string ComputeStreamHash(Stream inputStream);

    /// <summary>
    /// Returns a hash of the specified byte array.
    /// </summary>
    /// <param name="buffer">The byte array.</param>
    /// <returns>A computed hash of the array, based on the contents.</returns>
    string ComputeByteArrayHash(byte[] buffer);
}