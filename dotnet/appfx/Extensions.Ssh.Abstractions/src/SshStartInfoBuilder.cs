namespace Bearz.Extensions.Ssh;

public class SshStartInfoBuilder
{
    private string? Host { get; set; }

    private int Port { get; set; } = 22;

    private string? UserName { get; set; }

    private List<IAuthenticationMethod> AuthenticationMethods { get; } = new();

    public SshStartInfoBuilder WithHost(string host)
    {
        this.Host = host;
        return this;
    }

    public SshStartInfoBuilder WithPort(int port)
    {
        this.Port = port;
        return this;
    }

    public SshStartInfoBuilder WithUserName(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username));

        this.UserName = username;
        return this;
    }

    public SshStartInfoBuilder WithPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

        var userName = this.UserName ?? throw new InvalidOperationException("UserName is not set.");
        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = userName,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
        });
        return this;
    }

    public SshStartInfoBuilder WithPassword(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(password));

        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = username,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
        });
        return this;
    }

    public SshStartInfoBuilder WithPassword(char[] password)
    {
        var userName = this.UserName ?? throw new InvalidOperationException("UserName is not set.");
        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = userName,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
        });
        return this;
    }

    public SshStartInfoBuilder WithPassword(string username, char[] password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));

        if (password.Length == 0)
            throw new ArgumentException("Value cannot be empty.", nameof(password));

        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = username,
            Password = System.Text.Encoding.UTF8.GetBytes(password),
        });
        return this;
    }

    public SshStartInfoBuilder WithPassword(byte[] password)
    {
        var userName = this.UserName ?? throw new InvalidOperationException("UserName is not set.");
        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = userName,
            Password = password,
        });
        return this;
    }

    public SshStartInfoBuilder WithPassword(string username, byte[] password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(username));

        if (password.Length == 0)
            throw new ArgumentException("Value cannot be empty.", nameof(password));

        this.AuthenticationMethods.Add(new PasswordAuthentication
        {
            Username = username,
            Password = password,
        });
        return this;
    }

    public SshStartInfoBuilder WithPrivateKey(string privateKeyPath, string? passphrase = null)
    {
        if (string.IsNullOrWhiteSpace(privateKeyPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(privateKeyPath));

        var userName = this.UserName ?? throw new InvalidOperationException("UserName is not set.");
        var secret = passphrase.IsNullOrWhiteSpace()
            ? Array.Empty<byte>()
            : System.Text.Encoding.UTF8.GetBytes(passphrase);
        this.AuthenticationMethods.Add(new PrivateKeyAuthentication
        {
            Username = userName,
            PrivateKeyPath = privateKeyPath,
            Passphrase = secret,
        });
        return this;
    }

    public SshStartInfoBuilder WithPrivateKey(string privateKeyPath, byte[] passphrase)
    {
        if (string.IsNullOrWhiteSpace(privateKeyPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(privateKeyPath));

        var userName = this.UserName ?? throw new InvalidOperationException("UserName is not set.");
        this.AuthenticationMethods.Add(new PrivateKeyAuthentication
        {
            Username = userName,
            PrivateKeyPath = privateKeyPath,
            Passphrase = passphrase,
        });
        return this;
    }

    public SshStartInfoBuilder WithPrivateKey(string username, string privateKeyPath, string? passphrase)
    {
        if (username.IsNullOrWhiteSpace())
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(privateKeyPath));

        if (username.IsNullOrWhiteSpace())
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(passphrase));

        var secret = passphrase.IsNullOrWhiteSpace()
            ? Array.Empty<byte>()
            : System.Text.Encoding.UTF8.GetBytes(passphrase);
        this.AuthenticationMethods.Add(new PrivateKeyAuthentication
        {
            Username = username,
            PrivateKeyPath = privateKeyPath,
            Passphrase = secret,
        });
        return this;
    }

    public SshStartInfoBuilder WithPrivateKey(string username, string privateKeyPath, byte[] passphrase)
    {
        if (username.IsNullOrWhiteSpace())
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(privateKeyPath));

        if (username.IsNullOrWhiteSpace())
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(passphrase));

        this.AuthenticationMethods.Add(new PrivateKeyAuthentication
        {
            Username = username,
            PrivateKeyPath = privateKeyPath,
            Passphrase = passphrase,
        });
        return this;
    }

    public SshStartInfo Build()
    {
        if (this.UserName.IsNullOrWhiteSpace())
            throw new InvalidOperationException("UserName is not set.");

        if (this.Host.IsNullOrWhiteSpace())
            throw new InvalidOperationException("Host is not set.");

        var connection = new SshStartInfo()
        {
            Host = this.Host,
            Port = this.Port,
            UserName = this.UserName,
        };

        foreach (var authenticationMethod in this.AuthenticationMethods)
        {
            connection.Add(authenticationMethod);
        }

        return connection;
    }
}