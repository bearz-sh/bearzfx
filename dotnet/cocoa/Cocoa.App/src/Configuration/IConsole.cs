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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/adapters/IConsole.cs

namespace Cocoa.Configuration;

public interface IConsole
{
    /// <summary>
    ///   Gets the standard error output stream.
    /// </summary>
    /// <returns>
    ///   A <see cref="T:System.IO.TextWriter" /> that represents the standard error output stream.
    /// </returns>
    /// <filterpriority>1.</filterpriority>
    TextWriter Error { get; }

    /// <summary>
    ///   Gets the standard output stream.
    /// </summary>
    /// <returns>
    ///   A <see cref="T:System.IO.TextWriter" /> that represents the standard output stream.
    /// </returns>
    /// <filterpriority>1.</filterpriority>
    TextWriter Out { get; }

    /// <summary>
    /// Gets a value indicating whether output has been redirected from the standard output stream.
    /// </summary>
    bool IsOutputRedirected { get; }

    /// <summary>
    /// Gets a value indicating whether the error output stream has been redirected from the standard error stream.
    /// </summary>
    bool IsErrorRedirected { get; }

    /// <summary>
    /// Gets a value indicating whether input has been redirected from the standard input stream.
    /// </summary>
    bool IsInputRedirected { get; }

    ConsoleColor BackgroundColor { get; set; }

    ConsoleColor ForegroundColor { get; set; }

    int BufferWidth { get; set; }

    int BufferHeight { get; set; }

    string Title { get; set; }

    bool KeyAvailable { get; }

    int CursorSize { get; set; }

    int LargestWindowWidth { get; }

    int LargestWindowHeight { get; }

    int WindowWidth { get; set; }

    int WindowHeight { get; set; }

    int WindowLeft { get; set; }

    int WindowTop { get; set; }

    /// <summary>
    ///   Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>
    ///   The next line of characters from the input stream, or null if no more lines are available.
    /// </returns>
    /// <exception cref="T:System.IO.IOException">
    ///   An I/O error occurred.
    /// </exception>
    /// <exception cref="T:System.OutOfMemoryException">
    ///   There is insufficient memory to allocate a buffer for the returned string.
    /// </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    ///   The number of characters in the next line of characters is greater than <see cref="F:System.Int32.MaxValue" />.
    /// </exception>
    /// <filterpriority>1.</filterpriority>
    string ReadLine();

    string ReadLine(int timeoutMilliseconds);

    System.ConsoleKeyInfo ReadKey(bool intercept);

    System.ConsoleKeyInfo ReadKey(int timeoutMilliseconds);

    /// <summary>
    ///   Writes the specified string value to the standard output stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
    /// <filterpriority>1.</filterpriority>
    void Write(object value);

    /// <summary>
    ///   Writes the current line terminator to the standard output stream.
    /// </summary>
    /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
    /// <filterpriority>1.</filterpriority>
    void WriteLine();

    /// <summary>
    ///  Writes the text representation of the specified object, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
    /// <filterpriority>1.</filterpriority>
    void WriteLine(object value);

    void SetWindowSize(int width, int height);

    void SetBufferSize(int width, int height);

    void SetWindowPosition(int width, int height);
}