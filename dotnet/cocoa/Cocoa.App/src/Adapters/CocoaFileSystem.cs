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
// source: https://github.com/chocolatey/choco/blob/develop/src/chocolatey/infrastructure/filesystem/FileSystem.cs

using System.ComponentModel;
using System.Text;

using Bearz.Std;

using Cocoa.Logging;
using Cocoa.Tolerance;

using Microsoft.Extensions.Logging;

namespace Cocoa.Adapters;

    /// <summary>
    ///   Implementation of IFileSystem for Dot Net.
    /// </summary>
    /// <remarks>Normally we avoid regions, however this has so many methods that we are making an exception.</remarks>
    public sealed partial class CocoaFileSystem : IFileSystem
    {
        private const int MaxPathFile = 255;
        private const int MaxPathDirectory = 248;
        private static Lazy<IEnvironment> s_environmentInitializer = new Lazy<IEnvironment>(() => new CocoaEnvironment());
        private readonly int timesToTryOperation = 3;
        private ILogger log;

        public CocoaFileSystem()
        {
            this.log = Log.For<CocoaFileSystem>();
        }

        private static IEnvironment Environment
        {
            get { return s_environmentInitializer.Value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void InitializeWith(Lazy<IEnvironment> environment)
        {
            s_environmentInitializer = environment;
        }

        public void EnsureFileAttributeSet(string path, FileAttributes attributes)
        {
#if !NETLEGACY
            try
            {
                var info = Fs.Stat(path);
                if ((info.Attributes & attributes) != attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Trace))
                        this.log.LogTrace($"Adding '{attributes}' attribute(s) to '{path}'");
                    info.Attributes |= attributes;
                }
            }
            catch (Exception ex)
            {
                this.log.LogDebug(ex, "Error ensuring file attribute is added");
            }
#else
            if (Fs.IsDirectory(path))
            {
                var directoryInfo = this.GetDirectoryInfo(path);
                if ((directoryInfo.Attributes & attributes) != attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Adding '{attributes}' attribute(s) to '{path}'");

                    directoryInfo.Attributes |= attributes;
                }
            }
            else if (Fs.IsFile(path))
            {
                var fileInfo = this.GetFileInfoFor(path);
                if ((fileInfo.Attributes & attributes) != attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Adding '{attributes}' attribute(s) to '{path}'");

                    fileInfo.Attributes |= attributes;
                }
            }
#endif
        }

        public void EnsureFileAttributeRemoved(string path, FileAttributes attributes)
        {
#if !NETLEGACY
            try
            {
                var info = Fs.Stat(path);

                if ((info.Attributes & attributes) != attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Trace))
                        this.log.LogTrace($"Adding '{attributes}' attribute(s) to '{path}'");
                    info.Attributes |= attributes;
                }
            }
            catch (Exception ex)
            {
                this.log.LogDebug(ex, "Error ensuring file attribute is removed");
            }
#else
            if (this.DirectoryExists(path))
            {
                var directoryInfo = this.GetDirectoryInfo(path);
                if ((directoryInfo.Attributes & attributes) == attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Removing '{attributes}' attribute(s) from '{path}'.");

                    directoryInfo.Attributes &= ~attributes;
                }
            }
            else if (this.FileExists(path))
            {
                var fileInfo = this.GetFileInfoFor(path);
                if ((fileInfo.Attributes & attributes) == attributes)
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Removing '{attributes}' attribute(s) from '{path}'.");

                    fileInfo.Attributes &= ~attributes;
                }
            }
            else
            {
                if (this.log.IsEnabled(LogLevel.Debug))
                    this.log.LogDebug($"Unable to remove attributes {attributes} from path. Path not found: {path}'.");
            }
#endif
        }

        /// <summary>
        ///   Takes a guess at the file encoding by looking to see if it has a BOM.
        /// </summary>
        /// <param name="filePath">Path to the file name.</param>
        /// <returns>A best guess at the encoding of the file.</returns>
        /// <remarks>http://www.west-wind.com/WebLog/posts/197245.aspx.</remarks>
        public Encoding GetFileEncoding(string filePath)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            var buffer = new byte[5];
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var bytesRead = file.Read(buffer, 0, 5);
            file.Close();

            if (bytesRead < 4)
                return Encoding.UTF8;

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf) enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff) enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff) enc = Encoding.UTF32;
#pragma warning disable SYSLIB0001 // retain for backwards compatibility
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76) enc = Encoding.UTF7;
#pragma warning restore SYSLIB0001

            // assume xml is utf8
            // if (enc == Encoding.Default && get_file_extension(filePath).is_equal_to(".xml")) enc = Encoding.UTF8;
            return enc;
        }

        private void AllowRetries(Action action)
        {
            FaultTolerance.Retry(
                this.timesToTryOperation,
                action,
                waitDurationMilliseconds: 200,
                increaseRetryByMilliseconds: 100);
        }
    }