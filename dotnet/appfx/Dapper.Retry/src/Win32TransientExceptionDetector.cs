using System.ComponentModel;

namespace Bearz.Dapper.Retry;

public static class Win32TransientExceptionDetector
{
    public static bool ShouldRetryOn(Win32Exception ex)
    {
        switch (ex.NativeErrorCode)
        {
            // Timeout expired
            case 0x102:
            // Semaphore timeout expired
            case 0x121:
                return true;
            default:
                return false;
        }
    }
}