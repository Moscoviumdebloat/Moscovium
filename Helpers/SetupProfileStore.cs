using System.IO;
using System.Text.Json;
using MoscoviumThree.Models;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Persists and transfers <see cref="SetupProfile"/> checklists.
/// </summary>
public static class SetupProfileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string DefaultProfilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Moscovium", "setup-profile.json");

    public static SetupProfile? LoadDefault() => Load(DefaultProfilePath);

    public static void SaveDefault(SetupProfile profile) => Save(DefaultProfilePath, profile);

    public static SetupProfile? Load(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;

            var profile = JsonSerializer.Deserialize<SetupProfile>(File.ReadAllText(path), JsonOptions);
            if (profile == null) return null;

            // Only accept apps and tweaks we know about
            profile.WingetApps ??= new();
            profile.Tweaks ??= new();

            var knownApps = new HashSet<string>(SetupCatalog.Apps.ConvertAll(a => a.Id), StringComparer.OrdinalIgnoreCase);
            profile.WingetApps.RemoveAll(id => !knownApps.Contains(id));

            var knownTweaks = new HashSet<string>(TweakCatalog.Tweaks.ConvertAll(t => t.Name), StringComparer.OrdinalIgnoreCase);
            profile.Tweaks.RemoveAll(name => !knownTweaks.Contains(name));
            return profile;
        }
        catch
        {
            return null;
        }
    }

    public static void Save(string path, SetupProfile profile)
    {
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, JsonSerializer.Serialize(profile, JsonOptions));
        }
        catch
        {
            // Silently ignore profile save failures - never block the setup
        }
    }
}
