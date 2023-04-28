using System.Runtime.Versioning;
using System.Text;

using Bearz.Security.Cryptography;
using Bearz.Text;

namespace Tests;

public class Aes256EncryptionProviderTests
{
    [IntegrationTest]
    public void Verify_EncryptDecrypt(IAssert assert)
    {
        var options = new Aes256EncryptionProviderOptions() { Key = Encodings.Utf8NoBom.GetBytes("ASDFw23d@12sdk"), };
        var provider = new Aes256EncryptionProvider(options);
        var data = "Hello World";
        var encrypted = provider.Encrypt(data);
        var decrypted = provider.Decrypt(encrypted);
        assert.Equal(data, decrypted);
    }
}