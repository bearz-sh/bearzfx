// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
namespace Bearz;

internal static partial class Interop
{
    internal static partial class Sys
    {
#pragma warning disable S2344
#pragma warning disable S2346
#pragma warning disable SA1025
#pragma warning disable S1939
        internal enum Signals : int
        {
            None = 0,
            SIGKILL = 9,
            SIGSTOP = 19,
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.SystemNative, EntryPoint = "SystemNative_Kill", SetLastError = true)]
        internal static partial int Kill(int pid, Signals signal);
#else
        [DllImport(Libraries.SystemNative, EntryPoint = "SystemNative_Kill", SetLastError = true)]
        internal static extern int Kill(int pid, Signals signal);
#endif
    }
}