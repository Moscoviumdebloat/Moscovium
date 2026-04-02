using System.Diagnostics;
using Velopack;
using Velopack.Sources;

namespace MoscoviumThree.Helpers;

/// <summary>
/// Centralized service for checking, downloading, and applying Moscovium updates via Velopack.
/// Uses GitHub Releases as the update source.
/// </summary>
public static class UpdateService
{
    // TODO: Update this to your actual GitHub repository URL
    private const string GitHubRepoUrl = "https://github.com/Moscoviumdebloat/Moscovium-V3";

    private static UpdateManager CreateManager()
    {
        var source = new GithubSource(GitHubRepoUrl, null, false);
        return new UpdateManager(source);
    }

    /// <summary>
    /// Checks if an update is available. Returns the UpdateInfo if found, null otherwise.
    /// </summary>
    public static async Task<UpdateInfo?> CheckForUpdateAsync()
    {
        try
        {
            var mgr = CreateManager();
            return await mgr.CheckForUpdatesAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateService] Check failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Downloads the update with progress reporting (0-100).
    /// </summary>
    public static async Task DownloadUpdateAsync(UpdateInfo updateInfo, Action<int>? progressCallback = null)
    {
        var mgr = CreateManager();
        await mgr.DownloadUpdatesAsync(updateInfo, progress =>
        {
            progressCallback?.Invoke(progress);
        });
    }

    /// <summary>
    /// Applies the downloaded update and restarts the application.
    /// </summary>
    public static void ApplyUpdateAndRestart(UpdateInfo updateInfo)
    {
        var mgr = CreateManager();
        mgr.ApplyUpdatesAndRestart(updateInfo);
    }

    /// <summary>
    /// Applies the downloaded update without restarting (user can restart later).
    /// </summary>
    public static void ApplyUpdateAndExit(UpdateInfo updateInfo)
    {
        var mgr = CreateManager();
        mgr.ApplyUpdatesAndExit(updateInfo);
    }

    /// <summary>
    /// One-call method: checks, downloads silently, and returns the UpdateInfo
    /// if an update was downloaded and is ready to install. Does NOT restart.
    /// Returns null if no update is available or if an error occurs.
    /// </summary>
    public static async Task<UpdateInfo?> CheckAndDownloadSilentlyAsync()
    {
        try
        {
            var mgr = CreateManager();
            var update = await mgr.CheckForUpdatesAsync();
            if (update == null)
            {
                Debug.WriteLine("[UpdateService] No update available.");
                return null;
            }

            Debug.WriteLine($"[UpdateService] Update found: {update.TargetFullRelease.Version}");
            await mgr.DownloadUpdatesAsync(update);
            Debug.WriteLine("[UpdateService] Update downloaded successfully.");
            return update;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateService] Silent update failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the current installed version string, or "dev" if not installed via Velopack.
    /// </summary>
    public static string GetCurrentVersion()
    {
        try
        {
            var mgr = CreateManager();
            if (mgr.IsInstalled)
            {
                return mgr.CurrentVersion?.ToString() ?? "Unknown";
            }
        }
        catch { }
        return "dev";
    }

    /// <summary>
    /// Returns true if the app was installed via Velopack (not running from dev/portable).
    /// </summary>
    public static bool IsInstalledViaVelopack()
    {
        try
        {
            var mgr = CreateManager();
            return mgr.IsInstalled;
        }
        catch { return false; }
    }
}
