namespace Bearz.Extensions.Ssh;

public sealed class PasswordAuthentication : IAuthenticationMethod
{
    public string Kind => "Password";

    public string Username { get; set; } = string.Empty;

    public byte[] Password { get; set; } = Array.Empty<byte>();
}