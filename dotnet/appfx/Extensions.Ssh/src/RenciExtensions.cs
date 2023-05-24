using Renci.SshNet;

namespace Bearz.Extensions.Ssh;

internal static class RenciExtensions
{
    public static ConnectionInfo CacheConnection(this SshStartInfo info)
    {
        var methods = new List<AuthenticationMethod>();
        foreach (var m2 in info.AuthenticationMethods)
        {
            switch (m2)
            {
                case NoAuthentication no:
                    methods.Add(new NoneAuthenticationMethod(no.Username));
                    break;

                case PasswordAuthentication password:
                    methods.Add(new PasswordAuthenticationMethod(password.Username, password.Password));
                    break;

                case PrivateKeyAuthentication privateKey:
                    PrivateKeyFile pk;
                    pk = privateKey.PassphraseString is null ? new PrivateKeyFile(privateKey.PrivateKeyPath) : new PrivateKeyFile(privateKey.PrivateKeyPath, privateKey.PassphraseString);

                    methods.Add(new PrivateKeyAuthenticationMethod(privateKey.Username, pk));

                    break;
            }
        }

        var conn2 = new ConnectionInfo(info.Host, info.Port, info.UserName, methods.ToArray());
        info.CachedConnection = conn2;
        return conn2;
    }
}