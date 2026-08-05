using System.Diagnostics;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MoscoviumThree.Models;

namespace MoscoviumThree.Helpers;

public static class SetupAutomationHelper
{
    public static bool IsWingetAvailable()
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "--version",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (process == null) return false;
            process.WaitForExit(10000);
            return process.HasExited && process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Refresh winget sources so a fresh install sees the latest package metadata.
    /// </summary>
    public static Task<bool> UpdateWingetSourcesAsync(Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke("Updating winget sources...");
        return RunWingetAsync("source update --accept-source-agreements", progressCallback);
    }

    /// <summary>
    /// True if the package is already installed.
    /// </summary>
    public static Task<bool> IsAppInstalledAsync(SetupApp app)
    {
        if (app.IsDownload || string.IsNullOrEmpty(app.WingetId)) return Task.FromResult(false);
        return RunWingetAsync($"list --id {app.WingetId} --exact --accept-source-agreements --disable-interactivity", null);
    }

    /// <summary>
    /// Installs a catalog app: winget (optionally from a specific source), a direct download,
    /// or a ZIP that is extracted and its bundled installer run.
    /// Download apps resolve their latest link from the vendor page when configured.
    /// </summary>
    public static async Task<bool> InstallAppAsync(SetupApp app, Action<string>? progressCallback = null)
    {
        if (app.IsZip && !string.IsNullOrEmpty(app.ZipUrl))
        {
            progressCallback?.Invoke($"Downloading {app.Name}...");
            return await InstallFromZipAsync(app.ZipUrl, app.Name);
        }

        if (app.IsDownload && !string.IsNullOrEmpty(app.DownloadUrl))
        {
            var url = app.DownloadUrl;
            if (!string.IsNullOrEmpty(app.ResolvePageUrl) && !string.IsNullOrEmpty(app.ResolvePattern))
            {
                progressCallback?.Invoke($"Resolving latest {app.Name}...");
                var latest = await ResolveLatestUrlAsync(app.ResolvePageUrl, app.ResolvePattern);
                if (latest != null)
                {
                    url = latest;
                }
                else
                {
                    progressCallback?.Invoke($"Latest lookup failed, using stored {app.Name} link.");
                }
            }

            progressCallback?.Invoke($"Downloading {app.Name}...");
            return await InstallFromUrlAsync(url, app.Name);
        }

        var sourceArg = string.IsNullOrEmpty(app.Source) ? "" : $" --source {app.Source}";
        return await InstallWingetAppAsync(app.WingetId, sourceArg, progressCallback);
    }

    /// <summary>
    /// Scrapes a vendor page for the latest direct installer link.
    /// </summary>
    private static async Task<string?> ResolveLatestUrlAsync(string pageUrl, string pattern)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html");

            var html = await client.GetStringAsync(pageUrl);
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Value.Trim() : null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Latest URL resolution failed for {pageUrl}: {ex.Message}");
            return null;
        }
    }

    public static Task<bool> InstallWingetAppAsync(string wingetId, string sourceArg = "", Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke($"Installing {wingetId}...");
        return RunWingetAsync(
            $"install --id {wingetId} --exact --silent --accept-package-agreements --accept-source-agreements --disable-interactivity{sourceArg}",
            progressCallback);
    }

    /// <summary>
    /// Downloads a ZIP, extracts it to temp and runs its bundled installer (setup.exe / install*.exe).
    /// </summary>
    private static async Task<bool> InstallFromZipAsync(string zipUrl, string appName)
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Moscovium");
        Directory.CreateDirectory(tempRoot);

        var zipPath = Path.Combine(tempRoot, $"{appName}-{Guid.NewGuid():N}.zip");
        var extractPath = Path.Combine(tempRoot, $"{appName}-{Guid.NewGuid():N}");

        try
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) })
            {
                var response = await client.GetAsync(zipUrl);
                if (!response.IsSuccessStatusCode) return false;

                await using (var fileStream = File.Create(zipPath))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            Directory.CreateDirectory(extractPath);
            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);

            var installer = FindZipInstaller(extractPath);
            if (installer == null)
            {
                Debug.WriteLine($"No installer executable found in zip for {appName}");
                return false;
            }

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = installer,
                UseShellExecute = true,
                Verb = "runas"
            });
            if (process == null) return false;

            await process.WaitForExitAsync();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Zip install failed for {appName}: {ex.Message}");
            return false;
        }
        finally
        {
            try { if (File.Exists(zipPath)) File.Delete(zipPath); } catch { }
        }
    }

    /// <summary>
    /// Finds the setup executable inside an extracted app: prefers SETUP.EXE, then install*.exe,
    /// then any .exe directly under the root.
    /// </summary>
    private static string? FindZipInstaller(string root)
    {
        var preferred = new[] { "setup.exe", "install.exe", "installer.exe" };
        foreach (var name in preferred)
        {
            var candidate = Directory.EnumerateFiles(root, name, SearchOption.AllDirectories).FirstOrDefault();
            if (candidate != null) return candidate;
        }

        var installExe = Directory.EnumerateFiles(root, "install*.exe", SearchOption.AllDirectories).FirstOrDefault();
        if (installExe != null) return installExe;

        return Directory.EnumerateFiles(root, "*.exe", SearchOption.TopDirectoryOnly).FirstOrDefault();
    }

    /// <summary>
    /// Downloads a direct installer to temp and runs it elevated.
    /// </summary>
    private static async Task<bool> InstallFromUrlAsync(string url, string appName)
    {
        var fileName = Path.GetFileName(new Uri(url).LocalPath);
        if (string.IsNullOrEmpty(fileName)) fileName = "installer.exe";

        var destPath = Path.Combine(Path.GetTempPath(), "Moscovium", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return false;

            await using (var fileStream = File.Create(destPath))
            {
                await response.Content.CopyToAsync(fileStream);
            }

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = destPath,
                UseShellExecute = true,
                Verb = "runas"
            });
            if (process == null) return false;

            await process.WaitForExitAsync();
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Download/install failed for {appName}: {ex.Message}");
            return false;
        }
    }

    public static Task<bool> UpgradeAllAppsAsync(Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke("Upgrading all winget apps...");
        return RunWingetAsync(
            "upgrade --all --silent --accept-package-agreements --accept-source-agreements --disable-interactivity",
            progressCallback);
    }

    private static async Task<bool> RunWingetAsync(string arguments, Action<string>? progressCallback = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "winget",
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null) return false;

            var stderrTask = process.StandardError.ReadToEndAsync();
            string output = await process.StandardOutput.ReadToEndAsync();
            string stderr = await stderrTask;
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                Debug.WriteLine($"winget {arguments}\n{output}\n{stderr}");
            }
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"winget failed ({arguments}): {ex.Message}");
            return false;
        }
    }

    public static async Task<bool> RunWindowsUpdateAsync(Action<string>? progressCallback = null)
    {
        progressCallback?.Invoke("Running Windows Update...");

        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell",
            Arguments = "-Command \"Install-Module PSWindowsUpdate -Force -Confirm:$false; Get-WindowsUpdate -Install -AcceptAll -AutoReboot:$false\"",
            UseShellExecute = true,
            Verb = "runas", // Requires admin
            CreateNoWindow = false // Let user see the update window
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null) return false;

            await process.WaitForExitAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
