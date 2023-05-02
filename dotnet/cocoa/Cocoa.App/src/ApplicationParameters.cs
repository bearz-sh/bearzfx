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

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;

using Bearz.Extra.Strings;
using Bearz.Std;

using Cocoa.Platform;

namespace Cocoa
{
    /// <summary>
    ///   Application constants and settings for the application.
    /// </summary>
    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded")]
    public static class ApplicationParameters
    {
        public static readonly string Name = "Chocolatey";

        public static readonly string LoggingFile = @"chocolatey.log";
        public static readonly string LoggingSummaryFile = @"choco.summary.log";
        public static readonly string Log4NetConfigurationAssembly = @"chocolatey";
        public static readonly string Log4NetConfigurationResource = @"chocolatey.infrastructure.logging.log4net.config.xml";
        public static readonly string ChocolateyFileResources = "chocolatey.resources";
        public static readonly string ChocolateyConfigFileResource = @"chocolatey.infrastructure.app.configuration.chocolatey.config";

        public static readonly string LicensedChocolateyAssemblySimpleName = "chocolatey.licensed";
        public static readonly string LicensedComponentRegistry = @"chocolatey.licensed.infrastructure.app.registration.ContainerBinding";
        public static readonly string LicensedConfigurationBuilder = @"chocolatey.licensed.infrastructure.app.builders.ConfigurationBuilder";
        public static readonly string LicensedEnvironmentSettings = @"chocolatey.licensed.infrastructure.app.configuration.EnvironmentSettings";
        public static readonly string PackageNamesSeparator = ";";
        public static readonly string UnofficialChocolateyPublicKey = "fd112f53c3ab578c";
        public static readonly string OfficialChocolateyPublicKey = "79d02ea9cad655eb";
        public static readonly string HookPackageIdExtension = ".hook";
        public static readonly string ChocolateyCommunityFeedPushSourceOld = "https://chocolatey.org/";
        public static readonly string ChocolateyCommunityFeedPushSource = "https://push.chocolatey.org/";
        public static readonly string ChocolateyCommunityGalleryUrl = "https://community.chocolatey.org/";
        public static readonly string ChocolateyCommunityFeedSource = "https://community.chocolatey.org/api/v2/";
        public static readonly string ChocolateyLicensedFeedSource = "https://licensedpackages.chocolatey.org/api/v2/";
        public static readonly string ChocolateyLicensedFeedSourceName = "chocolatey.licensed";
        public static readonly string UserAgent = "Chocolatey Command Line";
        public static readonly string RegistryValueInstallLocation = "InstallLocation";
        public static readonly string AllPackages = "all";
        public static readonly string LocalSystemSidString = "S-1-5-18";
        public static readonly SecurityIdentifier LocalSystemSid = new SecurityIdentifier(LocalSystemSidString);

        /// <summary>
        ///   Default is 45 minutes.
        /// </summary>
        public static readonly int DefaultWaitForExitInSeconds = 2700;

        public static readonly int DefaultWebRequestTimeoutInSeconds = 30;

        public static readonly string[] ConfigFileExtensions = new string[] { ".autoconf", ".config", ".conf", ".cfg", ".jsc", ".json", ".jsonp", ".ini", ".xml", ".yaml" };
        public static readonly string ConfigFileTransformExtension = ".install.xdt";
        public static readonly string[] ShimDirectorFileExtensions = new string[] { ".gui", ".ignore" };

        public static readonly string HashProviderFileTooBig = "UnableToDetectChanges_FileTooBig";
        public static readonly string HashProviderFileLocked = "UnableToDetectChanges_FileLocked";

        /// <summary>
        /// This is a readonly bool set to true. It is only shifted for specs.
        /// </summary>
        public static readonly bool LockTransactionalInstallFiles = true;
        public static readonly string PackagePendingFileName = ".chocolateyPending";

        /// <summary>
        /// This is a readonly bool set to true. It is only shifted for specs.
        /// </summary>
        public static readonly bool AllowPrompts = true;

        public static bool IsDebugModeCliPrimitive()
        {
            var args = Environment.GetCommandLineArgs();
            var isDebug = false;

            // no access to the good stuff here, need to go a bit primitive in parsing args
            foreach (var arg in args)
            {
                if (arg.Contains("-debug", StringComparison.InvariantCultureIgnoreCase) || arg.EqualsInvariant("-d") || arg.EqualsInvariant("/d"))
                {
                    isDebug = true;
                    break;
                }
            }

            return isDebug;
        }

        public static class Paths
        {
            public static readonly string ChocolateyAppDataDirectory =
                FsPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Name);

            public static readonly string InstallDirectory = GetInstallLocation();

            public static readonly string LogDirectory = FsPath.Combine(InstallDirectory, "logs");

            public static readonly string GlobalConfigFile =
                FsPath.Combine(InstallDirectory, "config", "chocolatey.config");

            public static readonly string LicenseFile =
                FsPath.Combine(InstallDirectory, "license", "chocolatey.license.xml");

            public static readonly string UserProfileDirectory = GetUserProfile();

            public static readonly string HttpCacheUserDirectory =
                FsPath.Combine(UserProfileDirectory, ".chocolatey", "http-cache");

            public static readonly string HttpCacheDirectory = GetHttpCacheLocation();

            public static readonly string UserLicenseFile =
                FsPath.Combine(UserProfileDirectory, "chocolatey.license.xml");

            public static readonly string PackagesDirectory = FsPath.Combine(InstallDirectory, "lib");

            public static readonly string PackageFailuresDirectory = FsPath.Combine(InstallDirectory, "lib-bad");

            public static readonly string PackageBackupDirectory = FsPath.Combine(InstallDirectory, "lib-bkp");

            public static readonly string ShimsDirectory = FsPath.Combine(InstallDirectory, "bin");

            public static readonly string ChocolateyPackageInfoStoreDirectory =
                FsPath.Combine(InstallDirectory, ".chocolatey");

            public static readonly string ExtensionsDirectory = FsPath.Combine(InstallDirectory, "extensions");

            public static readonly string TemplatesDirectory = FsPath.Combine(InstallDirectory, "templates");

            public static readonly string HooksDirectory = FsPath.Combine(InstallDirectory, "hooks");

            public static readonly string PowerShellMachineModuleDirectory = GetPowershellMachineModulePath();

            public static readonly string PowerShellUserModuleDirectory = GetPowerShellUserModulePath();

            public static readonly string ShimGenExe = FsPath.Combine(InstallDirectory, "tools", "shimgen.exe");

            private static string GetPowershellMachineModulePath()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (Environment.Version.Major > 5)
                    {
                        return FsPath.Combine(Env.GetDirectory(SpecialDirectory.ProgramFiles), "PowerShell", "Modules");
                    }

                    return FsPath.Combine(
                        Env.GetDirectory(SpecialDirectory.ProgramFiles),
                        "WindowsPowerShell",
                        "Modules");
                }

                return FsPath.Combine("/usr/local/share", "powershell", "Modules");
            }

            private static string GetPowerShellUserModulePath()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (Environment.Version.Major > 5)
                    {
                        return FsPath.Combine(Env.GetDirectory(SpecialDirectory.MyDocuments), "PowerShell", "Modules");
                    }

                    return FsPath.Combine(
                        Env.GetDirectory(SpecialDirectory.MyDocuments),
                        "WindowsPowerShell",
                        "Modules");
                }

                return FsPath.Combine(Env.GetDirectory(SpecialDirectory.LocalApplicationData), "powershell", "Modules");
            }

            private static string GetUserProfile()
            {
                var userProfile = Environment.GetFolderPath(
                    Environment.SpecialFolder.UserProfile,
                    Environment.SpecialFolderOption.DoNotVerify);

                // why?
                if (userProfile.IsNullOrWhiteSpace())
                    return ChocolateyAppDataDirectory;

                return userProfile;
            }

            private static string GetInstallLocation()
            {
                if (Env.TryGet(EnvironmentKeys.ChocolateyInstall, out var installLocation))
                    return installLocation;

                var execution = Assembly.GetExecutingAssembly().Location ?? Assembly.GetEntryAssembly()?.Location;
                if (execution.IsNullOrWhiteSpace())
                    return ChocolateyAppDataDirectory;

                if (execution.StartsWith("file:///"))
                    execution = execution.Substring(8);
                else if (execution.StartsWith("file://"))
                    execution = execution.Substring(7);
                else if (execution.StartsWith("file:/"))
                    execution = execution.Substring(6);
                else if (execution.StartsWith("file:"))
                    execution = execution.Substring(5);

                return FsPath.Dirname(execution)!;
            }

            private static string GetHttpCacheLocation()
            {
                if (User.IsElevated() || string.IsNullOrEmpty(Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile,
                        Environment.SpecialFolderOption.DoNotVerify)))
                {
                    // CommonAppDataChocolatey is always set to ProgramData\Chocolatey.
                    // So we append HttpCache to that name if it is possible.
                    return ChocolateyAppDataDirectory + "HttpCache";
                }
                else
                {
                    return HttpCacheUserDirectory;
                }
            }
        }

        public static class ExitCodes
        {
            public static readonly int ErrorFailNoActionReboot = 350;
            public static readonly int ErrorInstallSuspend = 1604;
        }

        public static class ConfigSettings
        {
            public static readonly string CacheLocation = "cacheLocation";
            public static readonly string CommandExecutionTimeoutSeconds = "commandExecutionTimeoutSeconds";
            public static readonly string Proxy = "proxy";
            public static readonly string ProxyUser = "proxyUser";
            public static readonly string ProxyPassword = "proxyPassword";
            public static readonly string ProxyBypassList = "proxyBypassList";
            public static readonly string ProxyBypassOnLocal = "proxyBypassOnLocal";
            public static readonly string WebRequestTimeoutSeconds = "webRequestTimeoutSeconds";
            public static readonly string UpgradeAllExceptions = "upgradeAllExceptions";
            public static readonly string DefaultTemplateName = "defaultTemplateName";
            public static readonly string DefaultPushSource = "defaultPushSource";
        }

        public static class Features
        {
            public static readonly string ChecksumFiles = "checksumFiles";
            public static readonly string AllowEmptyChecksums = "allowEmptyChecksums";
            public static readonly string AllowEmptyChecksumsSecure = "allowEmptyChecksumsSecure";
            public static readonly string AutoUninstaller = "autoUninstaller";
            public static readonly string FailOnAutoUninstaller = "failOnAutoUninstaller";
            public static readonly string AllowGlobalConfirmation = "allowGlobalConfirmation";
            public static readonly string FailOnStandardError = "failOnStandardError";
            public static readonly string UsePowerShellHost = "powershellHost";
            public static readonly string LogEnvironmentValues = "logEnvironmentValues";
            public static readonly string VirusCheck = "virusCheck";
            public static readonly string FailOnInvalidOrMissingLicense = "failOnInvalidOrMissingLicense";
            public static readonly string IgnoreInvalidOptionsSwitches = "ignoreInvalidOptionsSwitches";
            public static readonly string UsePackageExitCodes = "usePackageExitCodes";
            public static readonly string UseEnhancedExitCodes = "useEnhancedExitCodes";
            public static readonly string UseFipsCompliantChecksums = "useFipsCompliantChecksums";
            public static readonly string ShowNonElevatedWarnings = "showNonElevatedWarnings";
            public static readonly string ShowDownloadProgress = "showDownloadProgress";
            public static readonly string StopOnFirstPackageFailure = "stopOnFirstPackageFailure";
            public static readonly string UseRememberedArgumentsForUpgrades = "useRememberedArgumentsForUpgrades";
            public static readonly string IgnoreUnfoundPackagesOnUpgradeOutdated = "ignoreUnfoundPackagesOnUpgradeOutdated";
            public static readonly string SkipPackageUpgradesWhenNotInstalled = "skipPackageUpgradesWhenNotInstalled";
            public static readonly string RemovePackageInformationOnUninstall = "removePackageInformationOnUninstall";
            public static readonly string LogWithoutColor = "logWithoutColor";
            public static readonly string ExitOnRebootDetected = "exitOnRebootDetected";
            public static readonly string LogValidationResultsOnWarnings = "logValidationResultsOnWarnings";
            public static readonly string UsePackageRepositoryOptimizations = "usePackageRepositoryOptimizations";
            public static readonly string DisableCompatibilityChecks = "disableCompatibilityChecks";
        }

        public static class Messages
        {
            public static readonly string ContinueChocolateyAction = "Moving forward with chocolatey actions.";
            public static readonly string NugetEventActionHeader = "Nuget called an event";
        }
    }
}