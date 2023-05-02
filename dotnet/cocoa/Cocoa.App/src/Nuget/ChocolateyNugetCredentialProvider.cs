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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure.app/nuget/ChocolateyNugetCredentialProvider.cs

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.RegularExpressions;

using Bearz.Extra.Strings;

using Cocoa.Configuration;
using Cocoa.Configuration.Sections;
using Cocoa.Logging;
using Cocoa.Services;

using Microsoft.Extensions.Logging;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Credentials;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Cocoa.Nuget;

public sealed class ChocolateyNugetCredentialProvider : ICredentialProvider
{
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded")]
    private const string InvalidUrl = "http://somewhere123zzaafasd.invalid";

    private readonly ChocolateyConfiguration config;

    private readonly ICredentialsPromptProvider promptProvider;

    private readonly ILogger log;

    public ChocolateyNugetCredentialProvider(
        ChocolateyConfiguration config,
        ICredentialsPromptProvider promptProvider,
        ILogger<ChocolateyNugetCredentialProvider>? logger = null)
    {
        this.config = config;
        this.Id = $"{nameof(ChocolateyNugetCredentialProvider)}_{Guid.NewGuid()}";
        this.promptProvider = promptProvider;
        this.log = logger ?? Log.For<ChocolateyNugetCredentialProvider>();
    }

    /// <summary>
    /// Gets the unique identifier of this credential provider.
    /// </summary>
    public string Id { get; }

    public Task<CredentialResponse> GetAsync(
        Uri uri,
        IWebProxy proxy,
        CredentialRequestType type,
        string message,
        bool isRetry,
        bool nonInteractive,
        CancellationToken cancellationToken)
    {
        if (uri == null)
        {
            throw new ArgumentNullException("uri");
        }

        if (isRetry && this.log.IsEnabled(LogLevel.Warning))
        {
            this.log.LogWarning("Invalid credentials specified.");
        }

        var configSourceUri = new Uri(InvalidUrl);

        if (this.log.IsEnabled(LogLevel.Debug))
            this.log.LogDebug($"Attempting to gather credentials for '{uri}'");

        try
        {
            // the source to validate against is typically passed in
            var firstSpecifiedSource = this.config.Sources.ToSafeString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault().ToSafeString();
            if (!string.IsNullOrWhiteSpace(firstSpecifiedSource))
            {
                configSourceUri = new Uri(firstSpecifiedSource);
            }
        }
        catch (Exception ex)
        {
            if (this.log.IsEnabled(LogLevel.Warning))
                this.log.LogWarning(ex, $"Cannot determine uri from specified source: {ex.Message}");
        }

        // did the user pass credentials and a source?
        if ((this.config.Sources.ToSafeString().TrimEnd('/').IsEqualTo(uri.OriginalString.TrimEnd('/')) ||
             configSourceUri.Host.IsEqualTo(uri.Host)) &&
            !this.config.SourceCommand.Username.IsNullOrWhiteSpace() &&
            this.config.SourceCommand.Password.IsNullOrWhiteSpace())
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug("Using passed credentials");

            var creds = new NetworkCredential(this.config.SourceCommand.Username, this.config.SourceCommand.Password);
            return Task.FromResult(new CredentialResponse(creds));
        }

        // credentials were not explicit
        // discover based on closest match in sources
        var candidateSources = this.config.MachineSources.Where(
            s =>
            {
                var sourceUrl = s.Key.ToSafeString().TrimEnd('/');

                try
                {
                    var sourceUri = new Uri(sourceUrl);
                    return sourceUri.Host.IsEqualTo(uri.Host)
                        && !string.IsNullOrWhiteSpace(s.Username)
                        && !string.IsNullOrWhiteSpace(s.EncryptedPassword);
                }
                catch (Exception ex)
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug(ex, $"Invalid Uri {sourceUrl}.");

                    this.log.LogError($"Source '{sourceUrl}' is not a valid Uri");
                }

                return false;
            }).ToList();

        MachineSourceConfiguration? source = null;

        if (candidateSources.Count == 1)
        {
            // only one match, use it
            source = candidateSources.FirstOrDefault();
        }
        else if (candidateSources.Count > 1)
        {
            // find the source that is the closest match
            foreach (var candidateSource in candidateSources)
            {
                var candidateRegEx = new Regex(
                    Regex.Escape(candidateSource.Key.ToSafeString().TrimEnd('/')),
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                if (candidateRegEx.IsMatch(uri.OriginalString.TrimEnd('/')))
                {
                    if (this.log.IsEnabled(LogLevel.Debug))
                        this.log.LogDebug($"Source selected will be {candidateSource.Key.ToSafeString().TrimEnd('/')}");

                    source = candidateSource;
                    break;
                }
            }

            if (source == null && !isRetry)
            {
                // use the first source. If it fails, fall back to grabbing credentials from the user
                var candidateSource = candidateSources.First();
                if (this.log.IsEnabled(LogLevel.Debug))
                    this.log.LogDebug($"Evaluated {candidateSources.Count} candidate sources but was unable to find a match, using {candidateSource.Key.ToSafeString().TrimEnd('/')}");

                source = candidateSource;
            }
        }

        if (source == null)
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug($"Asking user for credentials for '{uri.OriginalString}'");

            var creds = this.GetUserCredentials(uri, proxy, type);
            return Task.FromResult(new CredentialResponse(creds));
        }
        else
        {
            if (this.log.IsEnabled(LogLevel.Debug))
                this.log.LogDebug("Using saved credentials.");
        }

        // nuget uses DPAPI to encrypt passwords on Windows or Mono
        // its not available for .NET core.
        NetworkCredential cred2;
        if (RuntimeEnvironmentHelper.IsWindows || RuntimeEnvironmentHelper.IsMono)
        {
            cred2 = new NetworkCredential(source.Username, EncryptionUtility.DecryptString(source.EncryptedPassword));
        }
        else
        {
            cred2 = new NetworkCredential(source.Username, source.EncryptedPassword);
        }

        return Task.FromResult(new CredentialResponse(cred2));
    }

    public ICredentials GetUserCredentials(Uri uri, IWebProxy proxy, CredentialRequestType credentialType)
    {
        if (!this.config.Information.IsInteractive)
        {
            // https://blogs.msdn.microsoft.com/buckh/2004/07/28/authentication-in-web-services-with-httpwebrequest/
            // return credentialType == CredentialType.ProxyCredentials ? CredentialCache.DefaultCredentials : CredentialCache.DefaultNetworkCredentials;
            return CredentialCache.DefaultCredentials;
        }

        string message = credentialType == CredentialRequestType.Proxy
            ? "Please provide proxy credentials:"
            : $"Please provide credentials for: {uri.OriginalString}";

        var creds = this.promptProvider.PromptForCredentials(message);

        if (string.IsNullOrWhiteSpace(creds.Password))
        {
            if (this.log.IsEnabled(LogLevel.Warning))
                this.log.LogWarning("No password specified, this will probably error.");

            return CredentialCache.DefaultNetworkCredentials;
        }

        var credentials = new NetworkCredential
            {
                UserName = creds.Username,
                Password = creds.Password,
            };

        return credentials;
    }
}