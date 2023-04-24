namespace Bearz.Casa.Settings;

public static class Config
{
    private static SettingsStore settingsStore = new();

    public static void Reload(string? localBasePath = null)
    {
        settingsStore.Reload(localBasePath);
    }

    public static object? Get(string key)
    {
        return settingsStore.GetSetting(key);
    }

    public static T Get<T>(string key)
    {
        return settingsStore.GetSetting<T>(key);
    }

    public static T Get<T>(string key, T defaultValue)
    {
        var value = settingsStore.GetSetting(key);
        if (value is null)
        {
            return defaultValue;
        }

        return (T)value;
    }

    public static T Get<T>(string key, Func<T> defaultValue)
    {
        var value = settingsStore.GetSetting(key);
        if (value is null)
        {
            return defaultValue();
        }

        return (T)value;
    }

    public static void Set(string key, object? value, StoreKind storeKind = StoreKind.User, bool reload = false)
    {
        switch (storeKind)
        {
            case StoreKind.Global:
                settingsStore.SetGlobalSetting(key, value);
                break;

            case StoreKind.User:
                settingsStore.SetUserSetting(key, value);
                break;

            case StoreKind.Local:
                settingsStore.SetLocalSetting(key, value);
                break;
        }

        if (reload)
            settingsStore.Reload();
    }
}