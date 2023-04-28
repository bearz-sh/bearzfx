using System.Collections;

using Bearz.Diagnostics;

namespace Bearz.Std;

public class CommandOutput : IEnumerable<string>
{
    public int Id { get; internal set; }

    public string Executable { get; set; } = string.Empty;

    public int ExitCode { get; internal set; }

    public DateTime? StartedAt { get; internal set; }

    public DateTime? ExitedAt { get; internal set; }

    public IReadOnlyList<string> StdOut { get; internal set; } = Array.Empty<string>();

    public IReadOnlyList<string> StdErr { get; internal set; } = Array.Empty<string>();

    public CommandOutput ThrowOnInvalidExitCode(Func<int, bool>? validate = null)
    {
        validate ??= code => code == 0;
        if (!validate(this.ExitCode))
            throw new ProcessException(this.ExitCode, this.Executable);

        return this;
    }

    public CommandOutput ThrowOnStdError()
    {
        if (this.StdErr.Count > 0)
        {
            throw new ProcessException(
                $"{this.Executable} returned standard errors: {string.Join("\n", this.StdErr.Take(5))}")
            {
                FileName = this.Executable,
                ExitCode = this.ExitCode,
            };
        }

        return this;
    }

    public string ReadFirstLine()
    {
        if (this.StdOut.Count == 0 && this.StdErr.Count == 0)
            return string.Empty;

        if (this.StdOut.Count > 0)
            return this.StdOut[0];

        return this.StdErr[0];
    }

    public IEnumerator<string> GetEnumerator()
        => this.StdOut.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}