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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/synchronization/GlobalMutex.cs

using System.Reflection;
using System.Runtime.InteropServices;

using Cocoa.Logging;

using Microsoft.Extensions.Logging;

namespace Cocoa.Threading;

/// <summary>
///   global mutex used for synchronizing multiple processes based on appguid.
/// </summary>
/// <remarks>
///   Based on http://stackoverflow.com/a/7810107/2279385.
/// </remarks>
public sealed class GlobalMutex : IDisposable
{
    private const string AppGuid = "bd59231e-97d1-4fc0-a975-80c3fed498b7"; // matches the one in Assembly
    private readonly bool hasHandle;
    private readonly Semaphore mutex;

    private GlobalMutex(int timeout, ILogger<GlobalMutex>? logger = null)
    {
        var log = logger ?? Log.For<GlobalMutex>();
        var attr = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttribute<GuidAttribute>();
        var semaphoreName = attr is null ? @"Global\" + AppGuid : @"Global\" + attr.Value;
        try
        {
            this.mutex = Semaphore.OpenExisting(semaphoreName);
        }
        catch (Exception ex)
        {
            if (log.IsEnabled(LogLevel.Trace))
                log.LogTrace(ex, "Unable to open existing semaphore. Creating new one.");

            this.mutex = new Semaphore(0, 1, semaphoreName);
        }

        try
        {
            if (log.IsEnabled(LogLevel.Trace))
                log.LogTrace($"Waiting on the mutex handle for {timeout} milliseconds");

            this.hasHandle = this.mutex.WaitOne(timeout < 1 ? Timeout.Infinite : timeout, exitContext: false);

            if (!this.hasHandle)
            {
                throw new TimeoutException("Timeout waiting for exclusive access to value.");
            }
        }
        catch (InvalidOperationException)
        {
            this.hasHandle = true;
        }
    }

    /// <summary>
    /// Enters the Global Mutex.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    /// <param name="timeout">The timeout in milliseconds.</param>
    public static void Enter(Action? action, int timeout)
    {
        if (action is null)
            return;

        using (new GlobalMutex(timeout))
        {
            action.Invoke();
        }
    }

    /// <summary>
    /// Enters the Global Mutex.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="func">The function to perform.</param>
    /// <param name="timeout">The timeout in seconds.</param>
    /// <returns>The result.</returns>
    public static T Enter<T>(Func<T>? func, int timeout)
    {
        if (func is null)
            return default(T)!;

        using var mutex = new GlobalMutex(timeout);
        return func.Invoke();
    }

    public void Dispose()
    {
        if (this.hasHandle)
        {
            this.mutex.Release();
        }

        this.mutex.Dispose();
    }
}