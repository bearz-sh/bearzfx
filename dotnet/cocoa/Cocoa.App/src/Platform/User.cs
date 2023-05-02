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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/information/ProcessInformation.cs

using System.Runtime.InteropServices;
using System.Security.Principal;

using Bearz.Extra.Strings;
using Bearz.Std;

using Cocoa.Logging;

using Microsoft.Extensions.Logging;

namespace Cocoa.Platform;

public static class User
{
    public static bool IsAdministrator()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

        if (Environment.OSVersion.Version.Major < 6)
        {
            return isAdmin;
        }

        if (isAdmin)
        {
            return true;
        }

        // Processes subject to UAC actually have the Administrators group
        // stripped out from the process, and will return false for any
        // check about being an administrator, including a check against
        // the native `CheckTokenMembership` or `UserIsAdmin`. Instead we
        // need to perform a not 100% answer on whether they are an admin
        // based on if we have a split token.
        // Crediting http://www.davidmoore.info/blog/2011/06/20/how-to-check-if-the-current-user-is-an-administrator-even-if-uac-is-on/
        // and http://blogs.msdn.com/b/cjacks/archive/2006/10/09/how-to-determine-if-a-user-is-a-member-of-the-administrators-group-with-uac-enabled-on-windows-vista.aspx
        // NOTE: from the latter (the original) -
        //    Note that this technique detects if the token is split or not.
        //    In the vast majority of situations, this will determine whether
        //    the user is running as an administrator. However, there are
        //    other user types with advanced permissions which may generate a
        //    split token during an interactive login (for example, the
        //    Network Configuration Operators group). If you are using one of
        //    these advanced permission groups, this technique will determine
        //    the elevation type, and not the presence (or absence) of the
        //    administrator credentials.
        ILogger log = Log.For("chocolatey");
        log.LogDebug(@"User may be subject to UAC, checking for a split token (not 100% effective).");

        int tokenInfLength = Marshal.SizeOf(typeof(int));
        IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);

        try
        {
            IntPtr token = identity.Token;
            bool successfulCall = GetTokenInformation(
                token,
                TokenInformationType.TokenElevationType,
                tokenInformation,
                tokenInfLength,
                out tokenInfLength);

            if (!successfulCall)
            {
                log.LogWarning($"Error during native GetTokenInformation call - {Marshal.GetLastWin32Error()}");
                if (tokenInformation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(tokenInformation);
                }
            }

            TokenElevationType elevationType = (TokenElevationType)Marshal.ReadInt32(tokenInformation);

            switch (elevationType)
            {
                // TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
                case TokenElevationType.TokenElevationTypeFull:
                // TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
                case TokenElevationType.TokenElevationTypeLimited:
                    isAdmin = true;
                    break;
            }
        }
        finally
        {
            if (tokenInformation != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(tokenInformation);
            }
        }

        return isAdmin;
    }

    public static bool IsElevated()
    {
        if (Env.IsUserElevated)
        {
            return Env.IsUserElevated;
        }

        using var identity =
            WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.Duplicate);
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static bool IsTerminalServices()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        return Environment.GetEnvironmentVariable("SESSIONNAME")
            .ToSafeString()
            .Contains("rdp-", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsRemote()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        return IsTerminalServices() || Environment.GetEnvironmentVariable("SESSIONNAME")
            .ToSafeString() == string.Empty;
    }

    public static bool IsSystem()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        bool isSystem = false;
        using WindowsIdentity? identity = WindowsIdentity.GetCurrent();
        isSystem = identity.IsSystem;

        return isSystem;
    }

    /*
     https://msdn.microsoft.com/en-us/library/windows/desktop/aa376402.aspx
     BOOL WINAPI ConvertStringSidToSid(
       _In_   LPCTSTR StringSid,
       _Out_  PSID *Sid
     );
     */

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool ConvertStringSidToSid(string stringSid, out IntPtr sid);

    /*
     https://msdn.microsoft.com/en-us/library/windows/desktop/aa376389.aspx
     BOOL WINAPI CheckTokenMembership(
        _In_opt_  HANDLE TokenHandle,
        _In_      PSID SidToCheck,
        _Out_     PBOOL IsMember
     );

     */

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool CheckTokenMembership(IntPtr tokenHandle, IntPtr sidToCheck, out bool isMember);

    /*
      https://msdn.microsoft.com/en-us/library/windows/desktop/aa446671.aspx
      BOOL WINAPI GetTokenInformation(
        _In_       HANDLE TokenHandle,
        _In_       TOKEN_INFORMATION_CLASS TokenInformationClass,
        _Out_opt_  LPVOID TokenInformation,
        _In_       DWORD TokenInformationLength,
        _Out_      PDWORD ReturnLength
      );
    */
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern bool GetTokenInformation(
        IntPtr tokenHandle,
        TokenInformationType tokenInformationClass,
        IntPtr tokenInformation,
        int tokenInformationLength,
        out int returnLength);

    /// <summary>
    /// Passed to <see cref="GetTokenInformation"/> to specify what
    /// information about the token to return.
    /// </summary>
#pragma warning disable SA1201
    private enum TokenInformationType
    {
        TokenUser = 1,
        TokenGroups,
        TokenPrivileges,
        TokenOwner,
        TokenPrimaryGroup,
        TokenDefaultDacl,
        TokenSource,
        TokenType,
        TokenImpersonationLevel,
        TokenStatistics,
        TokenRestrictedSids,
        TokenSessionId,
        TokenGroupsAndPrivileges,
        TokenSessionReference,
        TokenSandBoxInert,
        TokenAuditPolicy,
        TokenOrigin,
        TokenElevationType,
        TokenLinkedToken,
        TokenElevation,
        TokenHasRestrictions,
        TokenAccessInformation,
        TokenVirtualizationAllowed,
        TokenVirtualizationEnabled,
        TokenIntegrityLevel,
        TokenUiAccess,
        TokenMandatoryPolicy,
        TokenLogonSid,
        MaxTokenInfoClass,
    }

    /// <summary>
    /// The elevation type for a user token.
    /// </summary>
    private enum TokenElevationType
    {
        TokenElevationTypeDefault = 1,
        TokenElevationTypeFull,
        TokenElevationTypeLimited,
    }
}