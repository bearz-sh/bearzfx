namespace Cocoa.Services;

public interface ICredentialsPromptProvider
{
    Credentials PromptForCredentials(string? title = null);
}

public class Credentials
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}