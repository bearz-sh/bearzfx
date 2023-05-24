namespace Bearz.Extensions.Ssh;

public abstract class CommandBuilder
{
    private readonly IDictionary<string, string?> env;

    protected CommandBuilder(IDictionary<string, string?> env)
    {
        this.env = env;
    }

    protected string Script { get; set; }

    protected string ScriptFile { get; set; }

    public CommandBuilder WithEnv(IDictionary<string, string?> env)
    {
        foreach (var kvp in env)
            this.env[kvp.Key] = kvp.Value;

        return this;
    }

    public CommandBuilder WithEnv(string key, string? value)
    {
        this.env[key] = value;
        return this;
    }

    public CommandBuilder WithScript(string script)
    {
        this.Script = script;
        return this;
    }
}