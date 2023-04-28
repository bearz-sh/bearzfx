using Bearz.Extra.Strings;
using Bearz.Std;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Bearz.Casa.App.Tasks;

public class SaveSettingsTask
{
    public string Store { get; set; } = string.Empty;

    public Dictionary<string, object?> Settings { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string? File { get; set; }

    public void Run()
    {
        string filePath = this.Store;
        switch (this.Store)
        {
            case "global":
                {
                    filePath = Path.Join(Paths.ConfigDirectory, "casa.json");
                }

                break;

            case "user":
                {
                    filePath = Path.Join(Paths.UserConfigDirectory, "casa.json");
                }

                break;
        }

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        };
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
        };

        if (!this.File.IsNullOrWhiteSpace())
        {
            if (!Fs.FileExists(filePath))
            {
                Fs.CopyFile(this.File, filePath);
                return;
            }

            var jsonString = Fs.ReadTextFile(filePath);
            var obj = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject = (JObject)obj;

            jsonString = Fs.ReadTextFile(this.File);
            var obj2 = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj2 is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject2 = (JObject)obj2;
            jObject.Merge(jObject2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });
        }
        else if (this.Settings.Count > 0)
        {
            var jsonString = Fs.ReadTextFile(filePath);
            var obj = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject = (JObject)obj;
            jsonString = JsonConvert.SerializeObject(this.Settings, serializerSettings);
            var obj2 = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj2 is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject2 = (JObject)obj2;
            jObject.Merge(jObject2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });
        }
    }

    public Task RunAsync(CancellationToken cancellationToken = default)
    {
        string filePath = this.Store;
        switch (this.Store)
        {
            case "global":
                {
                    filePath = Path.Join(Paths.ConfigDirectory, "casa.json");
                }

                break;

            case "user":
                {
                    filePath = Path.Join(Paths.UserConfigDirectory, "casa.json");
                }

                break;
        }

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy(),
        };
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented,
        };

        if (!this.File.IsNullOrWhiteSpace())
        {
            if (!Fs.FileExists(filePath))
            {
                Fs.CopyFile(this.File, filePath);
                return Task.CompletedTask;
            }

            var jsonString = Fs.ReadTextFile(filePath);
            var obj = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject = (JObject)obj;

            jsonString = Fs.ReadTextFile(this.File);
            var obj2 = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj2 is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject2 = (JObject)obj2;
            jObject.Merge(jObject2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });

            return Task.CompletedTask;
        }
        else if (this.Settings.Count > 0)
        {
            var jsonString = Fs.ReadTextFile(filePath);
            var obj = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject = (JObject)obj;
            jsonString = JsonConvert.SerializeObject(this.Settings, serializerSettings);
            var obj2 = JsonConvert.DeserializeObject(jsonString, serializerSettings);
            if (obj2 is null)
                throw new InvalidOperationException("Unable to get json object from ");

            var jObject2 = (JObject)obj2;
            jObject.Merge(jObject2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}