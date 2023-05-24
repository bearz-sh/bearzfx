using Bearz.Extensions.Secrets;
using Bearz.Extra.Strings;
using Bearz.Secrets;
using Bearz.Templates.Handlebars;

using HandlebarsDotNet;

namespace Bearz.Handlebars.Helpers;

public static class SecretHelper
{
    public static void GetSecretValue(
        ISecretVault vault,
        EncodedTextWriter writer,
        Context context,
        Arguments arguments)
    {
        if (arguments.Length == 0)
            throw new InvalidOperationException("conf helper requires at least one argument");

        var key = arguments[0].ToString();
        if (key.IsNullOrWhiteSpace())
            throw new InvalidOperationException("key must not be null or whitespace");

        var secret = vault.GetSecretValue(key);
        if (secret is not null)
        {
            writer.WriteSafeString(secret);
            return;
        }

        int length = 16;
        string special = "~`#@|:;^-_/";
        bool excludeUpper = false;
        bool excludeLower = false;
        bool excludeNumber = false;

        for (var i = 1; i < arguments.Length; i++)
        {
            var arg = arguments[i];
            if (i == 1)
            {
                length = arg.AsInt32(16);
                continue;
            }

            if (i == 2)
            {
                special = arg.AsString("~`#@|:;^-_/");
                continue;
            }

            if (i == 3)
            {
                excludeUpper = arg.AsBool();
                continue;
            }

            if (i == 4)
            {
                excludeLower = arg.AsBool();
                continue;
            }

            if (i == 5)
            {
                excludeNumber = arg.AsBool();
                break;
            }
        }

        var sg = new SecretGenerator();
        if (!excludeNumber)
            sg.Add(SecretCharacterSets.Digits);

        if (!excludeLower)
            sg.Add(SecretCharacterSets.LatinAlphaLowerCase);

        if (!excludeUpper)
            sg.Add(SecretCharacterSets.LatinAlphaUpperCase);

        sg.Add(special.IsNullOrWhiteSpace() ? SecretCharacterSets.SpecialSafe : special);

        secret = sg.GenerateAsString(length);
        vault.SetSecretValue(key, secret);

        writer.WriteSafeString(secret);
    }

    [CLSCompliant(false)]
    public static void RegisterSecretHelpers(this IHandlebars? hb, ISecretVault vault)
    {
        if (hb is null)
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("secret", (w, c, a) => GetSecretValue(vault, w, c, a));
            return;
        }

        hb.RegisterHelper("secret", (w, c, a) => GetSecretValue(vault, w, c, a));
    }
}