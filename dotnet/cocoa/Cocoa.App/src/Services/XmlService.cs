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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/services/XmlService.cs

using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Bearz.Extra.Strings;
using Bearz.Std;

using Cocoa.Adapters;
using Cocoa.Cryptography;
using Cocoa.Logging;
using Cocoa.Tolerance;

using Microsoft.Extensions.Logging;

namespace Cocoa.Services;

/// <summary>
///   XML interaction.
/// </summary>
public sealed class XmlService : IXmlService
{
    private const int MutexTimeout = 2000;

    private readonly IFileSystem fileSystem;
    private readonly IHashProvider hashProvider;

    private readonly ILogger log;

    public XmlService(IFileSystem fileSystem, IHashProvider hashProvider, ILogger<XmlService>? logger = null)
    {
        this.fileSystem = fileSystem;
        this.hashProvider = hashProvider;
        this.log = logger ?? Log.For<XmlService>();
    }

    public static XmlService Create()
    {
        return new XmlService(new CocoaFileSystem(), new CryptoHashProvider());
    }

    public T Deserialize<T>(string xmlFilePath)
    {
        return this.Deserialize<T>(xmlFilePath, 3);
    }

    public T Deserialize<T>(string xmlFilePath, int retryCount)
    {
        return FaultTolerance.RetryWithMutex(
            retryCount,
            () =>
            {
                if (this.log.IsEnabled(LogLevel.Trace))
                    this.log.LogTrace($"Entered mutex to deserialize '{xmlFilePath}'");

                return FaultTolerance.TryCatchWithLoggingException(
                    () => this.InnerDeserialize<T>(xmlFilePath),
                    $"Error deserializing response of type {typeof(T)}",
                    throwError: true);
            },
            waitDurationMilliseconds: 200,
            increaseRetryByMilliseconds: 200,
            mutexWaitDurationMilliseconds: MutexTimeout);
    }

    public void Serialize<T>(T xmlType, string xmlFilePath)
    {
        xmlFilePath = FsPath.Resolve(xmlFilePath);
        var dir = FsPath.GetDirectoryName(xmlFilePath);
        if (dir.IsNullOrWhiteSpace())
            throw new ArgumentException($"Unable to find directory for {xmlFilePath}", nameof(xmlType));

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        this.fileSystem.EnsureDirectoryExists(this.fileSystem.GetDirectoryName(xmlFilePath));

        var traceEnabled = this.log.IsEnabled(LogLevel.Trace);

        FaultTolerance.RetryWithMutex(
            3,
            () =>
            {
                if (traceEnabled)
                    this.log.LogTrace($"Entered mutex to serialize '{xmlFilePath}'");

                FaultTolerance.TryCatchWithLoggingException(
                    () => this.InnerSerialize(xmlType, xmlFilePath, traceEnabled),
                    errorMessage: $"Error serializing type {typeof(T)}",
                    throwError: true);
            },
            waitDurationMilliseconds: 200,
            increaseRetryByMilliseconds: 200,
            mutexWaitDurationMilliseconds: MutexTimeout);
    }

    private T InnerDeserialize<T>(string xmlFilePath)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var fileStream = this.fileSystem.OpenFileReadonly(xmlFilePath);
        using var fileReader = new StreamReader(fileStream);
        using var xmlReader = XmlReader.Create(fileReader);
        if (!xmlSerializer.CanDeserialize(xmlReader))
        {
            if (this.log.IsEnabled(LogLevel.Warning))
                this.log.LogWarning($"Cannot deserialize response of type {typeof(T)}");

            return default(T)!;
        }

        try
        {
            return (T)xmlSerializer.Deserialize(xmlReader)!;
        }
        catch (InvalidOperationException ex)
        {
            // Check if its just a malformed document.
            if (ex.Message.Contains("There is an error in XML document") && this.fileSystem.FileExists(xmlFilePath + ".backup"))
            {
                // If so, check for a backup file and try an parse that.
                using (var backupStream =
                       this.fileSystem.OpenFileReadonly(xmlFilePath + ".backup"))
                using (var backupReader = new StreamReader(backupStream))
                using (var backupXmlReader = XmlReader.Create(backupReader))
                {
                    var validConfig = (T)xmlSerializer.Deserialize(backupXmlReader)!;

                    // If there's no errors and it's valid, go ahead and replace the bad file with the backup.
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    if (validConfig != null)
                    {
                        // Close fileReader so that we can copy the file without it being locked.
                        fileReader.Close();
                        this.fileSystem.CopyFile(
                            xmlFilePath + ".backup",
                            xmlFilePath,
                            overwriteExisting: true);
                    }

                    return validConfig!;
                }
            }

            throw;
        }
        finally
        {
            foreach (var updateFile in this.fileSystem.GetFiles(
                         this.fileSystem.GetDirectoryName(xmlFilePath), "*.update"))
            {
                if (this.log.IsEnabled(LogLevel.Debug))
                    this.log.LogDebug($"Removing '{updateFile}'");

                FaultTolerance.TryCatchWithLoggingException(
                    () => this.fileSystem.DeleteFile(updateFile),
                    errorMessage: "Unable to remove update file",
                    logDebugInsteadOfError: true);
            }
        }
    }

    private void InnerSerialize<T>(T xmlType, string xmlFilePath, bool traceEnabled)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        if (traceEnabled)
            this.log.LogTrace($"Opening file stream for '{xmlFilePath}'");

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: true))
        {
            AutoFlush = true,
        };
        xmlSerializer.Serialize(streamWriter, xmlType);
        streamWriter.Flush();

        // Grab the hash of both files and compare them.
        if (traceEnabled)
            this.log.LogTrace($"Hashing original file at '{xmlFilePath}'");

        var originalFileHash = this.hashProvider.ComputeFileHash(xmlFilePath);
        memoryStream.Position = 0;
        if (!originalFileHash.IsEqualTo(this.hashProvider.ComputeStreamHash(memoryStream)))
        {
            if (traceEnabled)
                this.log.LogTrace($"Hashes were different, writing update file for '{xmlFilePath}'");

            // If there wasn't a file there in the first place, just write the new one out directly.
            if (string.IsNullOrEmpty(originalFileHash))
            {
                if (this.log.IsEnabled(LogLevel.Debug))
                    this.log.LogDebug($"There was no original file at '{xmlFilePath}'");

                memoryStream.Position = 0;
                this.fileSystem.WriteFile(xmlFilePath,  memoryStream);

                if (traceEnabled)
                    this.log.LogTrace("Closing xml memory stream.");
                memoryStream.Close();
                streamWriter.Close();

                return;
            }

            // Otherwise, create an update file, and resiliently move it into place.
            var tempUpdateFile = xmlFilePath + "." + Env.Process.Id + ".update";
            if (traceEnabled)
                this.log.LogTrace($"Creating a temp file at '{tempUpdateFile}'");

            memoryStream.Position = 0;
            if (traceEnabled)
                this.log.LogTrace($"Writing file '{tempUpdateFile}'");

            this.fileSystem.WriteFile(tempUpdateFile, memoryStream);

            memoryStream.Close();
            streamWriter.Close();

            if (traceEnabled)
                this.log.LogTrace($"Replacing file '{xmlFilePath}' with '{tempUpdateFile}'");

            this.fileSystem.ReplaceFile(tempUpdateFile, xmlFilePath, xmlFilePath + ".backup");
        }
    }
}