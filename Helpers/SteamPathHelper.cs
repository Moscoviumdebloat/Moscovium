namespace MoscoviumThree.Helpers;

/// <summary>
/// Detects CS2/CS:GO config folder locations across all drives.
/// </summary>
public static class SteamPathHelper
{
    private const string CS2_CFG_RELATIVE = @"steamapps\common\Counter-Strike Global Offensive\game\csgo\cfg";

    /// <summary>
    /// Find all CS2 cfg directories across all drives.
    /// </summary>
    public static List<string> FindCfgFolders()
    {
        var folders = new List<string>();

        // Check default Steam path
        var defaultPath = @"C:\Program Files (x86)\Steam\" + CS2_CFG_RELATIVE;
        if (Directory.Exists(defaultPath))
            folders.Add(defaultPath);

        // Search all drives for Steam installations
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
        {
            try
            {
                foreach (var steamDir in Directory.GetDirectories(drive.RootDirectory.FullName, "Steam", SearchOption.TopDirectoryOnly))
                {
                    var cfgPath = Path.Combine(steamDir, CS2_CFG_RELATIVE);
                    if (Directory.Exists(cfgPath) && !folders.Contains(cfgPath))
                        folders.Add(cfgPath);
                }
            }
            catch
            {
                // Skip drives/folders we can't access
            }
        }

        return folders;
    }
}
