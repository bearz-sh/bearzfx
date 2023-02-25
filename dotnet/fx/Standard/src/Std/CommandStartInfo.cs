using Bearz.Diagnostics;

namespace Bearz.Std;

public class CommandStartInfo
{
    private List<IProcessCapture>? stdOutRedirects;

    private List<IProcessCapture>? stdErrorRedirects;

    public CommandArgs Args { get; set; } = new CommandArgs();

    public string? Cwd { get; set; }

    public IDictionary<string, string>? Env { get; set; }

    public Stdio StdOut { get; set; }

    public Stdio StdErr { get; set; }

    public Stdio StdIn { get; set; }

    protected internal IReadOnlyList<IProcessCapture> StdOutRedirects
    {
        get
        {
            if (this.stdOutRedirects is null)
                return Array.Empty<IProcessCapture>();

            return this.stdOutRedirects;
        }
    }

    protected internal IReadOnlyList<IProcessCapture> StdErrorRedirects
    {
        get
        {
            if (this.stdErrorRedirects is null)
                return Array.Empty<IProcessCapture>();

            return this.stdErrorRedirects;
        }
    }

    public CommandStartInfo RedirectTo(IProcessCapture capture)
    {
        this.stdOutRedirects ??= new List<IProcessCapture>();
        this.stdOutRedirects.Add(capture);
        return this;
    }

    public CommandStartInfo RedirectErrorTo(IProcessCapture capture)
    {
        this.stdErrorRedirects ??= new List<IProcessCapture>();
        this.stdErrorRedirects.Add(capture);
        return this;
    }
}