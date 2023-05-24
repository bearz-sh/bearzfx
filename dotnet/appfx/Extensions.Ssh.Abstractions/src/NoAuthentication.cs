namespace Bearz.Extensions.Ssh;

public class NoAuthentication : IAuthenticationMethod
{
    public string Kind => "none";

    public string Username { get; set; } = string.Empty;
}