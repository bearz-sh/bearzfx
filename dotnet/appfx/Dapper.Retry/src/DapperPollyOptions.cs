using System.ComponentModel;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

namespace Bearz.Dapper.Retry;

public static class DapperPollyOptions
{
    public static ILogger Log { get; } = Bearz.Extensions.Logging.Log.For("Solovis.Cloud.Data.Polly");

    public static AsyncRetryPolicy DefaultAsyncRetryPolicy { get; set; } = Policy
        .Handle<TimeoutException>()
        .OrInner<Win32Exception>(Win32TransientExceptionDetector.ShouldRetryOn)
        .WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                Log.LogWarning(
                    exception,
                    "Error connecting to database or a timeout issue. Execution will retry after {RetryTimeSpan}. Retry attempt {RetryCount}",
                    timeSpan,
                    retryCount);
            });

    public static RetryPolicy DefaultRetryPolicy { get; set; } = Policy
        .Handle<TimeoutException>()
        .OrInner<Win32Exception>(Win32TransientExceptionDetector.ShouldRetryOn)
        .WaitAndRetry(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                Log.LogWarning(
                    exception,
                    "Error connecting to database or a timeout issue. Execution will retry after {RetryTimeSpan}. Retry attempt {RetryCount}",
                    timeSpan,
                    retryCount);
            });
}