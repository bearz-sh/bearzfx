using System.Text.Json;

using Bearz.Std;

namespace Bearz.Casa.Settings;

public class SettingsStore
{
    private readonly Dictionary<string, object?> envSettings = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, object?> mergedSettings = new(StringComparer.OrdinalIgnoreCase);

    private readonly string localSettingsPath;

    private Dictionary<string, object?> globalSettings = new(StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, object?> userSettings = new(StringComparer.OrdinalIgnoreCase);

    private Dictionary<string, object?> localSettings = new(StringComparer.OrdinalIgnoreCase);

    public SettingsStore(string? localBasePath = null)
    {
        this.localSettingsPath = Env.Cwd;
        if (Env.Has(ConfigKeys.LocalSettings))
            this.localSettingsPath = ConfigKeys.LocalSettings;

        var map = new Dictionary<string, string>()
        {
            ["CASA_ENV"] = ConfigKeys.Env,
            ["CASA_JSON_VAULT_KEY"] = ConfigKeys.JsonVaultKey,
            ["CASA_LOCAL_SETTINGS"] = ConfigKeys.LocalSettings,
        };

        foreach (var kvp in map)
        {
            if (Env.Has(kvp.Key))
            {
                this.envSettings[kvp.Value] = Env.Get(kvp.Key);
            }
        }

        this.Reload(localBasePath);
    }

    public void Reload(string? localBasePath = null)
    {
        this.mergedSettings.Clear();
        var configDir = Env.Directory(SpecialDirectory.Etc);
        this.globalSettings = LoadSettings(configDir);
        configDir = Env.Directory(SpecialDirectory.HomeConfig);
        this.userSettings = LoadSettings(configDir);
        configDir = localBasePath ?? this.localSettingsPath;
        this.localSettings = LoadSettings(configDir);

        foreach (var kvp in this.globalSettings)
        {
            this.mergedSettings[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in this.userSettings)
        {
            this.mergedSettings[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in this.localSettings)
        {
            this.mergedSettings[kvp.Key] = kvp.Value;
        }

        foreach (var kvp in this.envSettings)
        {
            this.mergedSettings[kvp.Key] = kvp.Value;
        }
    }

    public object? GetSetting(string key)
    {
        if (this.mergedSettings.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }

    public T GetSetting<T>(string key)
    {
        if (this.mergedSettings.TryGetValue(key, out var value))
        {
            return (T)value!;
        }

        return default!;
    }

    public string? GetString(string key)
    {
        if (this.mergedSettings.TryGetValue(key, out var value))
        {
            return (string?)value;
        }

        return null;
    }

    public void SetGlobalSetting(string key, object? value)
    {
        if (!Env.IsUserElevated)
        {
            throw new NotSupportedException("Cannot set global settings when not elevated.");
        }

        this.globalSettings[key] = value;
        var configDir = Env.Directory(SpecialDirectory.Etc);
        SaveSettings(configDir, this.localSettings);
    }

    public void SetUserSetting(string key, object? value)
    {
        this.userSettings[key] = value;
        var configDir = Env.Directory(SpecialDirectory.HomeConfig);
        SaveSettings(configDir, this.localSettings);
    }

    public void SetLocalSetting(string key, object? value, string? basePath = null)
    {
        this.localSettings[key] = value;
        var configDir = basePath ?? this.localSettingsPath;
        SaveSettings(configDir, this.localSettings);
    }

    private static Dictionary<string, object?> LoadSettings(string basePath)
    {
        var configDir = basePath;
        var casaDir = Std.FsPath.Combine(configDir, "casa");
        if (!Fs.DirectoryExists(casaDir))
            return new(StringComparer.OrdinalIgnoreCase);

        var settingsFile = Std.FsPath.Combine(casaDir, "settings.json");
        if (!Fs.FileExists(settingsFile))
            return new(StringComparer.OrdinalIgnoreCase);

        var settingsJson = Std.Fs.ReadTextFile(settingsFile);
        return JsonSerializer.Deserialize<Dictionary<string, object?>>(settingsJson) ?? new(StringComparer.OrdinalIgnoreCase);
    }

    private static void SaveSettings(string basePath, Dictionary<string, object?> settings)
    {
        var configDir = basePath;
        var casaDir = Std.FsPath.Combine(configDir, "casa");
        if (!Fs.DirectoryExists(casaDir))
            Fs.MakeDirectory(casaDir);
        var settingsFile = Std.FsPath.Combine(casaDir, "settings.json");
        Std.Fs.WriteTextFile(settingsFile, JsonSerializer.Serialize(settings));
    }
}