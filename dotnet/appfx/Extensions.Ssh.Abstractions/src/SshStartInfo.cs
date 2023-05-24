namespace Bearz.Extensions.Ssh;

public class SshStartInfo
{
    private readonly List<IAuthenticationMethod> authenticationMethods = new();

    public string Host { get; set; } = string.Empty;

    public int Port { get;  set; } = 22;

    public string UserName { get; set; } = string.Empty;

    public IReadOnlyList<IAuthenticationMethod> AuthenticationMethods => this.authenticationMethods;

    public object? CachedConnection { get; set; }

    public void Add(IAuthenticationMethod method)
        => this.authenticationMethods.Add(method);
}