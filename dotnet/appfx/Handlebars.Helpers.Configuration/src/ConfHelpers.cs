using Bearz.Extra.Strings;
using Bearz.Templates.Handlebars;

using HandlebarsDotNet;

using Microsoft.Extensions.Configuration;

namespace Bearz.Handlebars.Helpers;

public static class ConfHelpers
{
    [CLSCompliant(false)]
    public static void RegisterConfHelpers(this IHandlebars? hb, IConfiguration config)
    {
        void ConfHelper(EncodedTextWriter writer, Context context, Arguments arguments)
        {
            if (arguments.Length == 0) throw new InvalidOperationException("conf helper requires at least one argument");

            var key = arguments[0].ToString();
            if (key.IsNullOrWhiteSpace())
                throw new InvalidOperationException("key must not be null or whitespace");
            var defaultValue = arguments.Length > 1 ? arguments[1].ToString() : string.Empty;
            var section = config.GetSection(NormalizeConfigKey(key));
            if (section.Value == null)
            {
                writer.WriteSafeString(defaultValue);
                return;
            }

            writer.WriteSafeString(section.Value);
        }

        HandlebarsReturnHelper confBoolHelper = (_, args) =>
        {
            if (args.Length == 0) throw new InvalidOperationException("conf helper requires at least one argument");

            var key = args[0].ToString();
            if (key.IsNullOrWhiteSpace())
                throw new InvalidOperationException("key must not be null or whitespace");
            var section = config.GetSection(NormalizeConfigKey(key));

            if (section.Value is null)
                return false;

            return section.Value.AsBool(false);
        };

        if (hb is null)
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("conf", (HandlebarsHelper)ConfHelper);
            HandlebarsDotNet.Handlebars.RegisterHelper("conf-value", (HandlebarsHelper)ConfHelper);
            HandlebarsDotNet.Handlebars.RegisterHelper("conf-bool", confBoolHelper);
        }
        else
        {
            hb.RegisterHelper("conf", (HandlebarsHelper)ConfHelper);
            hb.RegisterHelper("conf-value", (HandlebarsHelper)ConfHelper);
            hb.RegisterHelper("conf-bool", confBoolHelper);
        }
    }

    private static string NormalizeConfigKey(string key)
    {
        var sb = Bearz.Text.StringBuilderCache.Acquire();
        foreach (var c in key)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else if (c is '(' or ')' && Std.Env.IsWindows)
            {
                sb.Append(c);
            }
            else if (c is '-' or '.' or '_' or '/' or ':')
            {
                sb.Append(':');
            }
            else
            {
                throw new InvalidOperationException($"Invalid character '{c}' in configuration key '{key}'");
            }
        }

        return Text.StringBuilderCache.GetStringAndRelease(sb);
    }
}