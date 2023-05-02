// Copyright © 2017 - 2022 Chocolatey Software, Inc
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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/ChocolateyNugetLogger.cs

using System.Text;

using Bearz.Extra.Strings;

using Microsoft.Extensions.Logging;

using NuGet.Common;

using ILogger = NuGet.Common.ILogger;
using LogLevel = NuGet.Common.LogLevel;

namespace Cocoa.Nuget;

public sealed class ChocolateyNugetLogger : ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger log;

    public ChocolateyNugetLogger(Microsoft.Extensions.Logging.ILogger logger)
    {
        this.log = logger;
    }

    public void LogDebug(string data)
    {
        this.Log(LogLevel.Debug, data);
    }

    public void LogVerbose(string data)
    {
        this.Log(LogLevel.Verbose, data);
    }

    public void LogWarning(string data)
    {
        this.Log(LogLevel.Warning, data);
    }

    public void LogError(string data)
    {
        this.Log(LogLevel.Error, data);
    }

    public void LogMinimal(string data)
    {
        // We log this as informational as we do not want
        // the output being shown to the user by default.
        // This includes information such as the time taken
        // to resolve dependencies, where the package was added
        // and so on.
        this.Log(LogLevel.Information, data);
    }

    public void LogInformation(string data)
    {
        // We log this as informational as we do not want
        // the output being shown to the user by default.
        // This includes information such as the time taken
        // to resolve dependencies, where the package was added
        // and so on.
        this.Log(LogLevel.Information, data);
    }

    public void LogInformationSummary(string data)
    {
        // We log it as minimal as we want the output to
        // be shown as an informational message in this case.
        this.Log(LogLevel.Minimal, data);
    }

    public void Log(LogLevel level, string data)
    {
        var prefixedMessage = PrefixAllLines("[NuGet]", data);

        switch (level)
        {
            case LogLevel.Debug:
                this.log.LogDebug(prefixedMessage);
                break;
            case LogLevel.Warning:
                this.log.LogWarning(prefixedMessage);
                break;
            case LogLevel.Error:
                this.log.LogError(prefixedMessage);
                break;
            case LogLevel.Verbose:
                this.log.LogTrace(prefixedMessage);
                break;
            case LogLevel.Information:
                this.log.LogTrace(prefixedMessage);
                break;
            case LogLevel.Minimal:
                this.log.LogInformation(prefixedMessage);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(level));
        }
    }

    public void Log(ILogMessage message)
    {
        this.Log(message.Level, message.Message);
    }

    public Task LogAsync(LogLevel level, string data)
    {
        this.Log(level, data);

        return Task.CompletedTask;
    }

    public Task LogAsync(ILogMessage message)
    {
        return this.LogAsync(message.Level, message.Message);
    }

    private static string PrefixAllLines(string prefix, string? message)
    {
        if (message == null || (message.IsNullOrWhiteSpace() && message.IndexOf('\n') < 0))
        {
            return prefix;
        }

        if (message.IndexOf('\n') < 0)
        {
            return $"{prefix} {message}";
        }

        var builder = new StringBuilder(message.Length);
        using (var reader = new StringReader(message))
        {
            while (reader.ReadLine() is { } line)
            {
                builder.Append(prefix);

                if (!string.IsNullOrWhiteSpace(line))
                {
                    builder.Append(' ').Append(line);
                }

                builder.AppendLine();
            }
        }

        // We specify the length we want, to ensure that we doesn't add any
        // new newlines to the output.
        return builder.ToString(0, builder.Length - Environment.NewLine.Length);
    }
}