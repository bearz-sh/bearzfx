using Bearz.Std;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Bearz.Casa.Settings;

public static class SettingSaver
{
    public static void SaveSetting(string store, string key, object value)
    {
        string filePath = store;
        switch (store)
        {
            case "global":
                {
                    var dir = Env.Directory(SpecialDirectory.Etc);
                    filePath = Std.FsPath.Combine(dir, "casa", "settings.json");
                }

                break;

            case "user":
                {
                    var dir = Env.Directory(SpecialDirectory.ApplicationData);
                    filePath = Std.FsPath.Combine(dir, "casa", "settings.json");
                }

                break;
        }

        string jsonString = File.ReadAllText(filePath);
        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        };
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
        };

        // Convert the JSON string to a JObject:
        var obj = JsonConvert.DeserializeObject(jsonString, serializerSettings);
        if (obj is null)
            throw new InvalidOperationException("Unable to get json object from ");

        var jObject = (JObject)obj;
        var kv = jObject.SelectToken(key);

        if (kv is null)
        {
            var segments = key.Split('.');
            JToken target = jObject;
            var last = segments.Length - 1;
            for (var i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];
                if (i == last)
                {
                    target[segment] = JValue.CreateNull();
                    kv = target[segment];
                    break;
                }

                var next = target[segment];
                if (next is null)
                {
                    next = new JObject();
                    target[segment] = next;
                }

                target = next;
            }
        }

        if (kv is not null)
        {
            switch (value)
            {
                case null:
                    kv.Replace(JValue.CreateNull());
                    break;

                case string s:
                    kv.Replace(s);
                    break;

                case bool b:
                    kv.Replace(b);
                    break;

                case int i:
                    kv.Replace(i);
                    break;

                default:
                    // ReSharper disable once AccessToStaticMemberViaDerivedType
                    var jv = JValue.FromObject(value);
                    kv.Replace(jv);
                    break;
            }
        }

        var json = JsonConvert.SerializeObject(jObject, serializerSettings);

        Fs.WriteTextFile(filePath, json);
    }
}