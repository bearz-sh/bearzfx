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
// source: https://github.com/chocolatey/choco/blob/release/2.0.0/src/chocolatey/infrastructure/tolerance/FaultTolerance.cs

using Cocoa.Configuration;
using Cocoa.Logging;
using Cocoa.Threading;

using Microsoft.Extensions.Logging;

namespace Cocoa.Tolerance;

/// <summary>
/// Provides methods that are able to tolerate faults and recover.
/// </summary>
public static class FaultTolerance
{
    /// <summary>
    /// Tries an action the specified number of tries, warning on each failure and raises error on the last attempt.
    /// </summary>
    /// <param name="numberOfTries">The number of tries.</param>
    /// <param name="action">The action.</param>
    /// <param name="waitDurationMilliseconds">The wait duration in milliseconds.</param>
    /// <param name="increaseRetryByMilliseconds">The time for each try to increase the wait duration by in milliseconds.</param>
    public static void Retry(int numberOfTries, Action? action, int waitDurationMilliseconds = 100, int increaseRetryByMilliseconds = 0)
    {
        if (action == null)
            return;

        Retry(
            numberOfTries,
            () =>
                {
                    action.Invoke();
                    return true;
                },
            waitDurationMilliseconds,
            increaseRetryByMilliseconds);
    }

    /// <summary>
    /// Tries a function the specified number of tries, warning on each failure and raises error on the last attempt.
    /// </summary>
    /// <typeparam name="T">The type of the return value from the function.</typeparam>
    /// <param name="numberOfTries">The number of tries.</param>
    /// <param name="function">The function.</param>
    /// <param name="waitDurationMilliseconds">The wait duration in milliseconds.</param>
    /// <param name="increaseRetryByMilliseconds">The time for each try to increase the wait duration by in milliseconds.</param>
    /// <returns>The return value from the function.</returns>
    /// <exception cref="System.ApplicationException">You must specify a number of retries greater than zero.</exception>
    public static T Retry<T>(int numberOfTries, Func<T>? function, int waitDurationMilliseconds = 100, int increaseRetryByMilliseconds = 0)
    {
        if (function == null)
            return default(T)!;

        if (numberOfTries == 0)
            throw new InvalidOperationException("You must specify a number of tries greater than zero.");

        var returnValue = default(T);

        var debugging = InDebugMode();

        var log = Log.For("chocolatey");

        for (int i = 1; i <= numberOfTries; i++)
        {
            try
            {
                returnValue = function.Invoke();
                break;
            }
            catch (Exception ex)
            {
                if (i == numberOfTries)
                {
                    log.LogError("Maximum tries of {0} reached. Throwing error.", numberOfTries);
                    throw;
                }

                int retryWait = waitDurationMilliseconds + (i * increaseRetryByMilliseconds);

                var exceptionMessage = debugging ? ex.ToString() : ex.Message;

                log.LogWarning(
                    "This is; try {3}/{4}. Retrying after {2} milliseconds.{0} Error converted to warning:{0} " + Environment.NewLine,
                    Environment.NewLine,
                    exceptionMessage,
                    retryWait,
                    i,
                    numberOfTries);

                Thread.Sleep(retryWait);
            }
        }

        return returnValue!;
    }

    /// <summary>
    /// Tries an action the specified number of tries, warning on each failure and raises error on the last attempt.
    /// </summary>
    /// <param name="numberOfTries">The number of tries.</param>
    /// <param name="action">The action.</param>
    /// <param name="waitDurationMilliseconds">The wait duration in milliseconds.</param>
    /// <param name="increaseRetryByMilliseconds">The time for each try to increase the wait duration by in milliseconds.</param>
    /// <param name="mutexWaitDurationMilliseconds">The time for the mutex wait duration by in milliseconds.</param>
    public static void RetryWithMutex(
        int numberOfTries,
        Action? action,
        int waitDurationMilliseconds = 100,
        int increaseRetryByMilliseconds = 0,
        int mutexWaitDurationMilliseconds = 2000)
    {
        if (action == null)
            return;

        Retry(
            numberOfTries,
            () => GlobalMutex.Enter(action, mutexWaitDurationMilliseconds),
            waitDurationMilliseconds,
            increaseRetryByMilliseconds);
    }

    /// <summary>
    /// Tries a function the specified number of tries, warning on each failure and raises error on the last attempt.
    /// </summary>
    /// <typeparam name="T">The type of the return value from the function.</typeparam>
    /// <param name="numberOfTries">The number of tries.</param>
    /// <param name="function">The function.</param>
    /// <param name="waitDurationMilliseconds">The wait duration in milliseconds.</param>
    /// <param name="increaseRetryByMilliseconds">The time for each try to increase the wait duration by in milliseconds.</param>
    /// <param name="mutexWaitDurationMilliseconds">The time for the mutex wait duration by in milliseconds.</param>
    /// <returns>The return value from the function.</returns>
    /// <exception cref="System.ApplicationException">You must specify a number of retries greater than zero.</exception>
    public static T RetryWithMutex<T>(
        int numberOfTries,
        Func<T>? function,
        int waitDurationMilliseconds = 100,
        int increaseRetryByMilliseconds = 0,
        int mutexWaitDurationMilliseconds = 2000)
    {
        return Retry<T>(
            numberOfTries,
            () => GlobalMutex.Enter<T>(function, 2000),
            waitDurationMilliseconds,
            increaseRetryByMilliseconds);
    }

    /// <summary>
    /// Wraps an action with a try/catch, logging an error when it fails.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="throwError">if set to <c>true</c> [throw error].</param>
    /// <param name="logWarningInsteadOfError">if set to <c>true</c> log as warning instead of error.</param>
    /// <param name="logDebugInsteadOfError">Log to debug.</param>
    public static void TryCatchWithLoggingException(Action? action, string errorMessage, bool throwError = false, bool logWarningInsteadOfError = false, bool logDebugInsteadOfError = false)
    {
        if (action == null)
            return;

        TryCatchWithLoggingException(
            () =>
                {
                    action.Invoke();
                    return true;
                },
            errorMessage,
            throwError,
            logWarningInsteadOfError,
            logDebugInsteadOfError);
    }

    /// <summary>
    /// Wraps a function with a try/catch, logging an error when it fails.
    /// </summary>
    /// <typeparam name="T">The type of the return value from the function.</typeparam>
    /// <param name="function">The function.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="throwError">if set to <c>true</c> [throw error].</param>
    /// <param name="logWarningInsteadOfError">if set to <c>true</c> log as warning instead of error.</param>
    /// <param name="logDebugInsteadOfError">Log to debug.</param>
    /// <returns>The return value from the function.</returns>
    public static T TryCatchWithLoggingException<T>(Func<T>? function, string errorMessage, bool throwError = false, bool logWarningInsteadOfError = false, bool logDebugInsteadOfError = false)
    {
        if (function == null)
            return default(T)!;

        var returnValue = default(T);

        try
        {
            returnValue = function.Invoke();
        }
        catch (Exception ex)
        {
            var exceptionMessage = InDebugMode() ? ex.ToString() : ex.Message;
            var log = Log.For("chocolatey");
            var msg = $"{errorMessage}:{Environment.NewLine} {exceptionMessage}";
            if (logDebugInsteadOfError)
            {
                log.LogDebug(msg);
            }
            else if (logWarningInsteadOfError)
            {
                log.LogWarning(msg);
            }
            else
            {
                log.LogError(msg);
            }

            if (throwError)
            {
                throw;
            }
        }

        return returnValue!;
    }

    private static bool InDebugMode()
    {
        var debugging = false;
        try
        {
            debugging = Config.Instance.Debug;
        }
        catch
        {
            // move on - debugging is false
        }

        return debugging;
    }
}