namespace Bearz.Std.Unix;

public enum Signals
{
    Sighup = 1, // Hangup (POSIX).
    Sigint = 2, // Interrupt (ANSI).
    Sigquit = 3, // Quit (POSIX).
    Sigill = 4, // Illegal instruction (ANSI).
    Sigtrap = 5, // Trace trap (POSIX).
    Sigabr = 6, // Abort (ANSI).
    Sigiot = 6, // IOT trap (4.2 BSD).
    Sigbus = 7, // BUS error (4.2 BSD).
    Sigfpe = 8, // Floating-point exception (ANSI).
    Sigkill = 9, // Kill, unblockable (POSIX).
    Sigusr1 = 10, // User-defined signal 1 (POSIX).
    Sigsegv = 11, // Segmentation violation (ANSI).
    Sigusr2 = 12, // User-defined signal 2 (POSIX).
    Sigpipe = 13, // Broken pipe (POSIX).
    Sigalrm = 14, // Alarm clock (POSIX).
    Sigterm = 15, // Termination (ANSI).
    Sigstkflt = 16, // Stack fault.
    Sigcld = Sigchld, // Same as SIGCHLD (System V).
    Sigchld = 17, // Child status has changed (POSIX).
    Sigcont = 18, // Continue (POSIX).
    Sigstop = 19, // Stop, unblockable (POSIX).
    Sigtstp = 20, // Keyboard stop (POSIX).
    Sigttin = 21, // Background read from tty (POSIX).
    Sigttou = 22, // Background write to tty (POSIX).
    Sigurg = 23, // Urgent condition on socket (4.2 BSD).
    Sigxcpu = 24, // CPU limit exceeded (4.2 BSD).
    Sigxfsz = 25, // File size limit exceeded (4.2 BSD).
    Sigvtalrm = 26, // Virtual alarm clock (4.2 BSD).
    Sigprof = 27, // Profiling alarm clock (4.2 BSD).
    Sigwinch = 28, // Window size change (4.3 BSD, Sun).
    Sigpoll = Sigio, // Pollable event occurred (System V).
    Sigio = 29, // I/O now possible (4.2 BSD).
    Sigpwr = 30, // Power failure restart (System V).
    Sigsys = 31, // Bad system call.
#pragma warning disable RCS1234
    Sigunused = 31,
#pragma warning restore RCS1234
}