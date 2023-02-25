using System.Security;
using System.Text;

namespace Bearz.Secrets;

public static class SecretGeneratorExtensions
{
    public static ISecretGenerator AddDefaults(
        this ISecretGenerator pg)
    {
        return pg.Add(SecretCharacterSets.LatinAlphaUpperCase)
            .Add(SecretCharacterSets.LatinAlphaLowerCase)
            .Add(SecretCharacterSets.Digits)
            .Add(SecretCharacterSets.SpecialSafe);
    }

    public static ReadOnlySpan<char> GenerateAsCharSpan(
        this ISecretGenerator pg,
        int length,
        IList<char>? characters = null,
        Func<char[], bool>? validate = null)
    {
        return pg.Generate(length, characters, validate);
    }

    public static string GenerateAsString(
        this ISecretGenerator pg,
        int length,
        IList<char>? characters = null,
        Func<char[], bool>? validate = null)
    {
        var chars = pg.Generate(length, characters, validate);
        var result = new string(chars);
        Array.Clear(chars, 0, chars.Length);
        return result;
    }

    public static unsafe SecureString GenerateAsSecureString(
        this ISecretGenerator pg,
        int length,
        char[]? characters = null,
        Func<char[], bool>? validate = null)
    {
        var password = pg.Generate(length, characters, validate);
        SecureString secureString;

        fixed (char* chPtr = password)
        {
            secureString = new SecureString(chPtr, password.Length);
        }

        Array.Clear(password, 0, password.Length);
        return secureString;
    }

    public static byte[] GenerateAsBytes(
        this ISecretGenerator pg,
        int length,
        char[]? characters = null,
        Encoding? encoding = null,
        Func<char[], bool>? validate = null)
    {
        encoding ??= Encoding.UTF8;
        var password = pg.Generate(length, characters, validate);
        var bytes = encoding.GetBytes(password);

        Array.Clear(password, 0, password.Length);
        return bytes;
    }

    public static ReadOnlySpan<byte> GenerateAsByteSpan(
        this ISecretGenerator pg,
        int length,
        char[]? characters = null,
        Encoding? encoding = null,
        Func<char[], bool>? validate = null)
    {
        return pg.GenerateAsBytes(length, characters, encoding, validate);
    }
}