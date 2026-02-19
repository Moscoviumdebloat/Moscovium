using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace MoscoviumThree.Helpers;

public static class SettingsHelper
{
    private static readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Moscovium", "settings.json");
    private static Dictionary<string, object> _settingsCache = new();

    static SettingsHelper()
    {
        LoadSettings();
    }

    private static void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                _settingsCache = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
            }
            else
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
            }
        }
        catch { _settingsCache = new(); }
    }

    private static void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settingsCache);
            File.WriteAllText(_settingsPath, json);
        }
        catch { }
    }

    public static string AppsInstallPath
    {
        get
        {
            if (_settingsCache.TryGetValue("AppsInstallPath", out var value) && value is JsonElement element && element.ValueKind == JsonValueKind.String)
            {
                return element.GetString() ?? GetDefaultPath();
            }
            // Fallback for direct string if not from JSON
            if (_settingsCache.TryGetValue("AppsInstallPath", out var valStr) && valStr is string s) return s;
            
            return GetDefaultPath();
        }
        set
        {
            _settingsCache["AppsInstallPath"] = value;
            SaveSettings();
        }
    }

    private static string GetDefaultPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Moscovium", "Apps");
    }

    public static bool AutoUpdateApps
    {
        get
        {
            if (_settingsCache.TryGetValue("AutoUpdateApps", out var value))
            {
                if (value is JsonElement element && (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False))
                {
                    return element.GetBoolean();
                }
                if (value is bool b) return b;
            }
            return false;
        }
        set
        {
            _settingsCache["AutoUpdateApps"] = value;
            SaveSettings();
        }
    }

    public static bool AutoUpdateMoscovium
    {
        get
        {
            if (_settingsCache.TryGetValue("AutoUpdateMoscovium", out var value))
            {
                if (value is JsonElement element && (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False))
                {
                    return element.GetBoolean();
                }
                if (value is bool b) return b;
            }
            return false;
        }
        set
        {
            _settingsCache["AutoUpdateMoscovium"] = value;
            SaveSettings();
        }
    }
    public static string? GitHubToken
    {
        get
        {
            if (_settingsCache.TryGetValue("GitHubToken", out var obj))
            {
                if (obj is JsonElement element && element.ValueKind == JsonValueKind.String)
                {
                    return element.GetString();
                }
                if (obj is string str)
                {
                    return str;
                }
            }
            return null;
        }
        set
        {
            _settingsCache["GitHubToken"] = value ?? "";
            SaveSettings();
        }
    }
}
